using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;


public class MapGenerator : MonoBehaviour
{
	private Random rand = new Random(0);

	[SerializeField] private int nodeCount = 100;
	[SerializeField] private int minDistance = 10;
	[SerializeField] private int pipeHeight = 5;
	[SerializeField] private Vector3 field = new Vector3(50, 10, 50);

	[SerializeField] private GameObject node;
	[SerializeField] private GameObject edge;

	private void Awake()
	{
		CreateMap();
	}

	private float RandomFloat(float min, float max)
	{
		return (float)(rand.NextDouble() * (max - min) + min);
	}

	private void CreateMap()
	{
		Vector3[] points = new Vector3[nodeCount];
		for (int i = 0; i < nodeCount; i++)
		{
			points[i] = new Vector3(RandomFloat(-field.x, field.x), RandomFloat(-field.y, field.y),
				RandomFloat(-field.z, field.z));

			var redo = true;
			while (redo)
			{
				redo = false;
				for (int j = 0; j < i; j++)
				{
					if (Vector3.Distance(points[i], points[j]) < minDistance)
					{
						points[i] = new Vector3(RandomFloat(-field.x, field.x), RandomFloat(-field.y, field.y),
							RandomFloat(-field.z, field.z));
						redo = true;
						break;
					}
				}
			}
		}

		int[] graph = new int[nodeCount];
		HashSet<int> found = new HashSet<int>();
		HashSet<int> notAdded = new HashSet<int>();
		for (int i = 0; i < nodeCount; i++)
		{
			graph[i] = -1;
			notAdded.Add(i);
		}

		found.Add(0);
		notAdded.Remove(0);

		while (notAdded.Count > 0)
		{
			var min = float.PositiveInfinity;
			var minI = -1;
			var minJ = -1;
			foreach (var i in notAdded)
			{
				foreach (var j in found)
				{
					var dist = Vector3.Distance(points[i], points[j]);
					if (dist < min)
					{
						min = dist;
						minI = i;
						minJ = j;
					}
				}
			}

			graph[minI] = minJ;
			notAdded.Remove(minI);
			found.Add(minI);
		}

		// for (int i = 1; i < 100; i++)
		// {
		// 	var min = float.PositiveInfinity;
		// 	var minJ = -1;
		// 	foreach (var j in found)
		// 	{
		// 		var dist = Vector3.Distance(points[i], points[j]);
		// 		if (dist < min)
		// 		{
		// 			min = dist;
		// 			minJ = j;
		// 		}
		// 	}
		//
		// 	graph[i] = minJ;
		// 	found.Add(i);
		// }

		Instantiate(node, points[0], Quaternion.identity, transform);

		for (int i = 1; i < nodeCount; i++)
		{
			int j = graph[i];
			Instantiate(node, points[i], Quaternion.identity, transform);
			var newEdge = Instantiate(edge, (points[i] + points[j]) / 2,
				Quaternion.LookRotation(points[i] - points[j]), transform);
			newEdge.transform.localScale =
				new Vector3(pipeHeight, pipeHeight, (points[i] - points[j]).magnitude - pipeHeight);
		}
	}
}
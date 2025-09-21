using Drone.Sensors.Noise;
using UnityEngine;

namespace Drone.Sensors
{
	public class Lidar : MonoBehaviour
	{
		[SerializeField] private float minDistance = 0.3f;
		[SerializeField] private float maxDistance = 6;
		[SerializeField] private LayerMask mask;

		[SerializeField] private GameObject debugLidarTarget;

		public float Distance => noise.Distance;
		public float DistanceNormalized => (float.IsInfinity(noise.Distance) || float.IsNaN(noise.Distance)) ? MaxDistance : noise.Distance;
		
		public float MaxDistance => maxDistance;
		public float MinDistance => minDistance;
		
		public bool IsValid => !float.IsInfinity(noise.Distance) && !float.IsNaN(noise.Distance);

		private LidarNoise noise;

		private void Awake()
		{
			noise = new LidarNoise(minDistance, maxDistance);
		}

		private void FixedUpdate()
		{
			if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, maxDistance, mask))
			{
				if (hitInfo.distance < 0)
				{
					noise.Set(LidarNoise.Inf);
					debugLidarTarget.SetActive(false);
					return;
				}
				noise.Set(hitInfo.distance);
				if (!float.IsPositiveInfinity(Distance))
				{
					debugLidarTarget.SetActive(true);
					debugLidarTarget.transform.position = transform.position + transform.forward * Distance;
				}
				else
				{
					debugLidarTarget.SetActive(false);
				}
			}
			else
			{
				noise.Set(LidarNoise.Inf);
				debugLidarTarget.SetActive(false);
			}
		}
	}
}
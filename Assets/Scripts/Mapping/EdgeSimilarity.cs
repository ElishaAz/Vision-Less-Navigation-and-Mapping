using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mapping
{
    public class EdgeSimilarity : MonoBehaviour
    {
        private float SimilarEdge(Edge a, Edge b)
        {
            // DTWDistance<Vector3> distance = Vector3.Distance;

            // return MyDTW.DTW(a.NormalizedPositions(), b.NormalizedPositions(), distance).Item1;
            List<Vector3> first = new List<Vector3>();

            foreach (var sample in a.Samples)
            {
                first.Add(sample.FrontRightPosition);
                first.Add(sample.FrontLeftPosition);
                first.Add(sample.BackRightPosition);
                first.Add(sample.BackLeftPosition);
            }

            List<Vector3> second = new List<Vector3>();
            foreach (var sample in b.Samples)
            {
                second.Add(sample.FrontRightPosition);
                second.Add(sample.FrontLeftPosition);
                second.Add(sample.BackRightPosition);
                second.Add(sample.BackLeftPosition);
            }

            return CloudPoint.ClosePoints(first, second, 0.1f);
        }


        private void Start()
        {
            Inconsistencies.Instance.OnNewEdge += OnNewEdge;
            Inconsistencies.Instance.OnNewNode += OnNewNode;
        }

        private void OnDestroy()
        {
            Inconsistencies.Instance.OnNewEdge -= OnNewEdge;
            Inconsistencies.Instance.OnNewNode -= OnNewNode;
        }

        private Color[] colors = new[]
        {
            Color.blue,
            Color.red,
            Color.green,
            Color.yellow,
            Color.cyan,
            Color.magenta
        };

        private int currentColor = 0;

        private readonly List<(Color, List<Edge>)> similarEdges = new();

        private void OnNewEdge(Edge edge)
        {
            Edge similarEdge = null;
            var ratio = 0f;
            var index = -1;
            for (int i = 0; i < Inconsistencies.Instance.Edges.Count - 1; i++)
            {
                var currentRatio = SimilarEdge(Inconsistencies.Instance.Edges[i], edge);
                if (currentRatio > ratio)
                {
                    similarEdge = Inconsistencies.Instance.Edges[i];
                    ratio = currentRatio;
                    index = i;
                }
            }

            Debug.Log($"New Edge. Ratio: {ratio}");

            if (index >= 0 && ratio > 0.5f)
            {
                bool found = false;
                foreach (var sim in similarEdges)
                {
                    foreach (var e in sim.Item2)
                    {
                        if (similarEdge == e)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        sim.Item2.Add(edge);
                        edge.SetColor(sim.Item1);
                        break;
                    }
                }

                if (!found)
                {
                    edge.SetColor(colors[currentColor]);
                    similarEdge.SetColor(colors[currentColor]);
                    currentColor = (currentColor + 1) % colors.Length;

                    similarEdges.Add((colors[currentColor],
                        new List<Edge> { edge, similarEdge }));
                }

                Debug.Log($"Similar edge: {index}, {ratio}, {similarEdge}");
            }
        }

        private void OnNewNode(Node node)
        {
        }
    }
}
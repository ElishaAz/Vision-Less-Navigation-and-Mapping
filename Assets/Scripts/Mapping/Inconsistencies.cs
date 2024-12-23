using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using UnityEngine;

namespace Mapping
{
    public class Inconsistencies : MonoBehaviour
    {
        [SerializeField] private DroneSensors sensors;
        [SerializeField] private float threshold = 1f;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject edgePrefab;
        [SerializeField] private int maxBackMerge = 5;
        [SerializeField] private float mergeDistance = 1f;
        [SerializeField] private float mergeTime = 10f;
        [SerializeField] private float interval = 0.1f;

        private readonly struct Sample
        {
            public readonly float Right;
            public readonly float Left;
            public readonly float Up;
            public readonly float Down;
            public readonly float Front;
            public readonly float Back;

            public readonly Vector3 Position;
            public readonly Vector3 Orientation;
            public readonly float Time;

            public Sample(
                float right, float left, float up, float down, float front, float back,
                Vector3 position, Vector3 orientation, float time
            )
            {
                Right = right;
                Left = left;
                Up = up;
                Down = down;
                Front = front;
                Back = back;

                Position = position;
                Orientation = orientation;
                Time = time;
            }
        }

        private class Node
        {
            private readonly List<(Sample, Sample)> samples = new List<(Sample, Sample)>();

            /// <summary>
            /// A list of samples, before and after the inconsistency.
            /// </summary>
            public IReadOnlyList<(Sample, Sample)> Samples => samples;

            public void AddSample(Sample before, Sample after)
            {
                samples.Add((before, after));
                Position = Samples.Aggregate(Vector3.zero,
                               (current, sample) => current + sample.Item2.Position)
                           / Samples.Count;

                prefab.transform.position = Position + DroneView.DroneView.Offset;
            }

            /// <summary>
            /// Average of all After positions
            /// </summary>
            public Vector3 Position { get; private set; }

            private GameObject prefab;

            public Node(GameObject prefab)
            {
                this.prefab = prefab;
            }
        }

        private class Edge
        {
            public Node From;
            public Node To;
            private readonly List<Sample> samples = new List<Sample>();
            public IReadOnlyList<Sample> Samples => samples;

            private GameObject prefab;

            public Edge(Node from, Node to, GameObject prefab)
            {
                From = from;
                To = to;
                this.prefab = prefab;
            }

            public void AddSample(Sample sample)
            {
                samples.Add(sample);
            }

            public void UpdatePosition()
            {
                prefab.transform.position = (From.Position + To.Position) / 2 + DroneView.DroneView.Offset;
                prefab.transform.rotation = Quaternion.LookRotation(To.Position - From.Position);
                prefab.transform.localScale = new Vector3(1, 1, Vector3.Distance(From.Position, To.Position));
            }
        }

        private readonly List<Node> nodes = new List<Node>();
        private readonly List<Edge> edges = new List<Edge>();
        private Node lastNode = null;
        private Edge currentEdge = null;

        private Sample lastSample;

        private void Start()
        {
            lastSample = new Sample(
                sensors.right.DistanceNormalized, sensors.left.DistanceNormalized,
                sensors.up.DistanceNormalized, sensors.down.DistanceNormalized,
                sensors.front.DistanceNormalized, sensors.back.DistanceNormalized,
                sensors.DronePosition, sensors.gyro.Orientation, Time.time);
        }


        private float nextUpdate;

        private void FixedUpdate()
        {
            nextUpdate -= Time.fixedDeltaTime;
            if (nextUpdate > 0) return;
            nextUpdate = interval;
            var sample = new Sample(
                sensors.right.DistanceNormalized, sensors.left.DistanceNormalized,
                sensors.up.DistanceNormalized, sensors.down.DistanceNormalized,
                sensors.front.DistanceNormalized, sensors.back.DistanceNormalized,
                sensors.DronePosition, sensors.gyro.Orientation, Time.time);

            currentEdge?.AddSample(sample);

            if (Mathf.Abs(sample.Right - lastSample.Right) > threshold ||
                Mathf.Abs(sample.Left - lastSample.Left) > threshold)
            {
                bool merged = false;
                for (int i = nodes.Count - 1; i >= Mathf.Max(nodes.Count - maxBackMerge, 0); i--)
                {
                    var lastIncon = nodes[i];
                    if (Vector3.Distance(lastIncon.Position, sample.Position) > mergeDistance
                        || sample.Time - lastIncon.Samples.Last().Item2.Time > mergeTime) continue;

                    lastIncon.AddSample(lastSample, sample);

                    foreach (var edge in edges.Where(edge => edge.From == lastIncon || edge.To == lastIncon))
                    {
                        edge.UpdatePosition();
                    }

                    if (lastNode != lastIncon)
                    {
                        CreateEdge(lastNode, lastIncon, new List<Sample> { lastSample, sample });
                    }

                    lastNode = lastIncon;
                    merged = true;
                    Debug.Log($"Merged {lastIncon.Position}");
                    break;
                }

                if (!merged)
                {
                    var node = new Node(Instantiate(nodePrefab, transform));
                    node.AddSample(lastSample, sample);

                    if (lastNode != null)
                    {
                        CreateEdge(lastNode, node, new List<Sample> { lastSample, sample });
                    }

                    nodes.Add(node);
                    lastNode = node;
                }
            }

            lastSample = sample;
        }

        private float SimilarEdge(Edge a, Edge b)
        {
            var diff = Vector3.Distance(a.Samples.First().Position, b.Samples.First().Position);
            DTWDistance<Sample> distance = (left, right) => Vector3.Distance(left.Position, right.Position) - diff;

            var dtw = new DTW<Sample>(a.Samples, b.Samples, distance);
            return dtw.ComputeDTW();
        }

        private bool SamplesSimilar(Sample left, Sample right)
        {
            if (Mathf.Abs(left.Time - right.Time) <= mergeTime)
            {
                // Samples are close in time. There probably isn't a lot of error in the OF
                if (Vector3.Distance(left.Position, right.Position) <= mergeDistance)
                {
                    return true;
                }
            }


            return false;
        }

        private void CreateEdge(Node from, Node to, List<Sample> samples)
        {
            var edge = new Edge(from, to, Instantiate(edgePrefab, transform));
            foreach (var sample in samples)
            {
                edge.AddSample(sample);
            }

            if (edges.Count >= 2)
            {
                Edge similarEdge = null;
                var dist = float.MaxValue;
                var index = -1;
                for (int i = 0; i < edges.Count - 1; i++)
                {
                    var currentDist = SimilarEdge(edges[i], currentEdge);
                    if (currentDist < dist)
                    {
                        similarEdge = edges[i];
                        dist = currentDist;
                        index = i;
                    }
                }

                Debug.Log($"Similar edge: {index}, {dist}, {similarEdge}");
            }

            edge.UpdatePosition();
            edges.Add(edge);
            currentEdge = edge;
        }
    }
}
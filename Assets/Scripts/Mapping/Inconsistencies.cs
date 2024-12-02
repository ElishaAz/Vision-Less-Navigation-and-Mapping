using System.Collections.Generic;
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
        [SerializeField] private float interval = 0.1f;

        private readonly struct Sample
        {
            public readonly float Right;
            public readonly float Left;
            public readonly float Up;
            public readonly float Down;
            public readonly float Forward;
            public readonly float Back;

            public Sample(float right, float left, float up, float down, float forward, float back)
            {
                Right = right;
                Left = left;
                Up = up;
                Down = down;
                Forward = forward;
                Back = back;
            }
        }

        private class Node
        {
            public Vector3 Position;
            public int Count;
            public GameObject Prefab;
            public Sample Before;
            public Sample After;
        }

        private class Edge
        {
            public Node From;
            public Node To;
            public GameObject Prefab;

            public void UpdatePosition()
            {
                Prefab.transform.position = (From.Position + To.Position) / 2 + DroneView.DroneView.Offset;
                Prefab.transform.rotation = Quaternion.LookRotation(To.Position - From.Position);
                Prefab.transform.localScale = new Vector3(1, 1, Vector3.Distance(From.Position, To.Position));
            }
        }

        private readonly List<Node> nodes = new List<Node>();
        private readonly List<Edge> edges = new List<Edge>();
        private Node lastNode = null;

        private Sample lastSample;

        private void Start()
        {
            lastSample = new Sample(sensors.right.DistanceNormalized, sensors.left.DistanceNormalized,
                sensors.up.DistanceNormalized, sensors.down.DistanceNormalized,
                sensors.front.DistanceNormalized, sensors.back.DistanceNormalized);
        }


        private float nextUpdate;

        private void FixedUpdate()
        {
            nextUpdate -= Time.fixedDeltaTime;
            if (nextUpdate > 0) return;
            nextUpdate = interval;
            var sample = new Sample(sensors.right.DistanceNormalized, sensors.left.DistanceNormalized,
                sensors.up.DistanceNormalized, sensors.down.DistanceNormalized,
                sensors.front.DistanceNormalized, sensors.back.DistanceNormalized);
            var position = sensors.DronePosition;

            if (Mathf.Abs(sample.Right - lastSample.Right) > threshold ||
                Mathf.Abs(sample.Left - lastSample.Left) > threshold)
            {
                bool merged = false;
                for (int i = nodes.Count - 1; i >= Mathf.Max(nodes.Count - maxBackMerge, 0); i--)
                {
                    var lastIncon = nodes[i];
                    if (Vector3.Distance(lastIncon.Position, position) < mergeDistance)
                    {
                        // TODO: add time check
                        // Weighted average
                        lastIncon.Position = (lastIncon.Position * lastIncon.Count + position) / (lastIncon.Count + 1);
                        lastIncon.Count++;
                        lastIncon.Prefab.transform.position = lastIncon.Position + DroneView.DroneView.Offset;

                        foreach (var edge in edges)
                        {
                            if (edge.From == lastIncon || edge.To == lastIncon)
                            {
                                edge.UpdatePosition();
                            }
                        }

                        if (lastNode != lastIncon)
                        {
                            // TODO add edge
                        }

                        lastNode = lastIncon;
                        merged = true;
                        Debug.Log($"Merged {lastIncon.Position}");
                        break;
                    }
                }

                if (!merged)
                {
                    var node = new Node
                    {
                        Position = position,
                        Count = 1,
                        Before = lastSample,
                        After = sample,
                        Prefab = Instantiate(nodePrefab, sensors.DronePosition + DroneView.DroneView.Offset,
                            Quaternion.identity, transform)
                    };
                    if (lastNode != null)
                    {
                        var edge = new Edge
                        {
                            From = (Node)lastNode,
                            To = node,
                            Prefab = Instantiate(edgePrefab, transform)
                        };
                        edge.UpdatePosition();
                        edges.Add(edge);
                    }

                    nodes.Add(node);
                    lastNode = node;
                }
            }

            lastSample = sample;
        }
    }
}
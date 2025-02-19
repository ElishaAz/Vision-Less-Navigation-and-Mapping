using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mapping
{
    public class Inconsistencies : MonoBehaviour
    {
        public static Inconsistencies Instance;

        [SerializeField] private DroneSensors sensors;
        [SerializeField] private float threshold = 1f;
        [SerializeField] private float maxDistance = 2f;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject edgePrefab;
        [SerializeField] private int maxBackMerge = 5;
        [SerializeField] private float mergeDistance = 1f;
        [SerializeField] private float mergeTime = 10f;
        [SerializeField] private float interval = 0.1f;

        [SerializeField] private RawImage currentEdgeImage;
        private Texture2D currentEdgeTexture;

        private readonly List<Node> nodes = new List<Node>();
        private readonly List<Edge> edges = new List<Edge>();
        public event Action<Edge> OnNewEdge;
        public event Action<Node> OnNewNode;

        public IReadOnlyList<Node> Nodes => nodes;
        public IReadOnlyList<Edge> Edges => edges;

        private Node lastNode = null;
        private readonly List<Sample> currentEdgeSamples = new List<Sample>();
        private readonly CloudPoint currentEdgeCloudPoint = new CloudPoint();

        private Sample lastSample;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            lastSample = new Sample(sensors, Time.time);
            currentEdgeTexture = new Texture2D(100, 100, TextureFormat.RGBA32, false);
            currentEdgeImage.texture = currentEdgeTexture;
        }


        private float nextUpdate;

        private void FixedUpdate()
        {
            nextUpdate -= Time.fixedDeltaTime;
            if (nextUpdate > 0) return;
            nextUpdate = interval;
            var sample = new Sample(sensors, Time.time);

            currentEdgeSamples.Add(sample);

            if (lastNode != null)
            {
                currentEdgeCloudPoint.Add(sample, -lastNode.Position);
                currentEdgeCloudPoint.ToTexture(currentEdgeTexture);
            }

            if (IsIncon(sample.FrontLeft, lastSample.FrontLeft) || IsIncon(sample.FrontRight, lastSample.FrontRight))
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
                        CreateEdge(lastNode, lastIncon);
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
                        CreateEdge(lastNode, node);
                    }

                    nodes.Add(node);
                    lastNode = node;
                }

                OnNewNode?.Invoke(lastNode);
            }

            lastSample = sample;
        }

        private bool IsIncon(float current, float last)
        {
            return Mathf.Abs(current - last) > threshold && (current < maxDistance || last < maxDistance);
        }

        private void CreateEdge(Node from, Node to)
        {
            var edge = new Edge(from, to, Instantiate(edgePrefab, transform));
            foreach (var sample in currentEdgeSamples)
            {
                edge.AddSample(sample);
            }
            currentEdgeSamples.Clear();
            currentEdgeCloudPoint.Clear();

            edge.UpdatePosition();
            OnNewEdge?.Invoke(edge);
            edges.Add(edge);
        }
    }
}
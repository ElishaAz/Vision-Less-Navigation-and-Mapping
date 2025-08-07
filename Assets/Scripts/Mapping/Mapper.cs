using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using Mapping.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Mapping
{
    public class Mapper : MonoBehaviour
    {
        public static Mapper Instance;

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

        [SerializeField] private bool save = false;
        private Texture2D currentEdgeTexture;

        public event Action<Edge, int> OnNewEdge;
        public event Action<Node> OnNewNode;

        public readonly Map Map = new Map();

        private Node lastNode = null;
        private readonly List<Sample> currentEdgeSamples = new List<Sample>();
        private readonly PointCloud currentEdgePointCloud = new PointCloud();

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
            save = false;
        }

        private float nextUpdate;

        private void FixedUpdate()
        {
            if (save)
            {
                save = false;
                MapLoader.Save(Map, "map.json");
            }

            nextUpdate -= Time.fixedDeltaTime;
            if (nextUpdate > 0) return;
            nextUpdate = interval;
            var sample = new Sample(sensors, Time.time);

            currentEdgeSamples.Add(sample);

            if (lastNode != null)
            {
                currentEdgePointCloud.Add(sample, -lastNode.Position);
                currentEdgePointCloud.ToTexture(currentEdgeTexture);
            }

            if (IsIncon(sample.FrontLeft, lastSample.FrontLeft) || IsIncon(sample.FrontRight, lastSample.FrontRight))
            {
                bool merged = false;
                for (int i = Map.Nodes.Count - 1; i >= Mathf.Max(Map.Nodes.Count - maxBackMerge, 0); i--)
                {
                    var lastIncon = Map.Nodes[i];
                    if (Vector3.Distance(lastIncon.Position, sample.Position) > mergeDistance
                        || sample.Time - lastIncon.Afters.Last().Time > mergeTime) continue;

                    lastIncon.AddSample(lastSample, sample);

                    foreach (var edge in Map.Edges.Where(edge => edge.From == lastIncon || edge.To == lastIncon))
                    {
                        EdgePosition.Of(edge)?.UpdatePosition();
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
                    var node = new Node();
                    node.AddSample(lastSample, sample);
                    NodePosition.Create(node, Instantiate(nodePrefab, transform));

                    if (lastNode != null)
                    {
                        CreateEdge(lastNode, node);
                    }

                    Map.AddNode(node);
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
            var edge = new Edge(from, to, currentEdgeSamples);
            EdgePosition.Create(edge, Instantiate(edgePrefab, transform));

            currentEdgeSamples.Clear();
            currentEdgePointCloud.Clear();

            Map.AddEdge(edge);
            var edgeIndex = Map.Edges.Count - 1;
            Algorithms.EdgeSimilarity.EdgeAdded(Map, edge, edgeIndex);
            OnNewEdge?.Invoke(edge, edgeIndex);
        }
    }
}
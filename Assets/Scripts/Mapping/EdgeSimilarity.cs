using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mapping.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Mapping
{
    public class EdgeSimilarity : MonoBehaviour
    {
        [SerializeField] private RawImage edge1Image;
        [SerializeField] private RawImage edge2Image;


        private void Start()
        {
            Mapper.Instance.OnNewEdge += OnNewEdge;
            Mapper.Instance.OnNewNode += OnNewNode;
        }

        private void OnDestroy()
        {
            Mapper.Instance.OnNewEdge -= OnNewEdge;
            Mapper.Instance.OnNewNode -= OnNewNode;
        }

        private readonly Color[] colors = new Color[]
        {
            Color.blue,
            Color.red,
            Color.green,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            new Color32(0x8a, 0x2b, 0xe2, 0xff),
            new Color32(0xff, 0x7f, 0x50, 0xff),
            new Color32(0x64, 0x95, 0xed, 0xff),
            new Color32(0x8f, 0xbc, 0x8f, 0xff),
            new Color32(0x00, 0xbf, 0xff, 0xff),
            new Color32(0xff, 0xd7, 0x00, 0xff),
            new Color32(0xf0, 0x80, 0x80, 0xff),
            new Color32(0xff, 0xa5, 0x00, 0xff),
            new Color32(0x98, 0xfb, 0x98, 0xff),
            new Color32(0x66, 0x33, 0x99, 0xff),
            new Color32(0x87, 0xce, 0xeb, 0xff),
            new Color32(0x46, 0x82, 0xb4, 0xff),
        };

        private int currentColor = 0;

        private readonly List<(Color, List<Edge>)> similarEdges = new();

        private void OnNewEdge(Edge edge, int currentEdgeIndex)
        {
            if (edge == null)
            {
                return;
            }

            PointCloud pointCloud = PointCloud.FromSamples(edge.Samples);

            edge1Image.texture = pointCloud.ToTexture(100, 100);
            Edge similarEdge = null;
            var ratio = 0f;
            var index = -1;
            for (int i = 0; i < currentEdgeIndex; i++)
            {
                var currentRatio = Mapper.Instance.Map.GetSimilarity(i, currentEdgeIndex);
                if (currentRatio > ratio)
                {
                    similarEdge = Mapper.Instance.Map.Edges[i];
                    ratio = currentRatio;
                    index = i;
                }
            }

            Mapper.Instance.Map.AddSimilarity(currentEdgeIndex, currentEdgeIndex, 1f);

            Debug.Log($"New Edge. Ratio: {ratio}");

            if (index >= 0 && ratio > 0.5f && similarEdge != null)
            {
                edge2Image.gameObject.SetActive(true);
                edge2Image.texture = similarEdge.PointCloud.ToTexture(100, 100);

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
                        EdgePosition.Of(edge)?.SetColor(sim.Item1);
                        break;
                    }
                }

                if (!found)
                {
                    EdgePosition.Of(edge)?.SetColor(colors[currentColor]);
                    EdgePosition.Of(similarEdge)?.SetColor(colors[currentColor]);
                    currentColor = (currentColor + 1) % colors.Length;

                    similarEdges.Add((colors[currentColor],
                        new List<Edge> { edge, similarEdge }));
                }

                Debug.Log($"Similar edge: {index}, {ratio}, {similarEdge}");
            }
            else
            {
                edge2Image.gameObject.SetActive(false);
            }
        }

        private void OnNewNode(Node node)
        {
        }
    }
}
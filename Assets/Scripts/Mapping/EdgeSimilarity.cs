using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Mapping
{
    public class EdgeSimilarity : MonoBehaviour
    {
        [SerializeField] private RawImage edge1Image;
        [SerializeField] private RawImage edge2Image;

        private static float AngleDifference(float angle1, float angle2)
        {
            float a = angle2 - angle1;
            if (a > 180)
                a -= 360;
            if (a < -180)
                a += 360;
            return a;
        }

        private static CloudPoint EdgeToCloudPoint(Edge edge)
        {
            CloudPoint cloud = new CloudPoint();

            foreach (var sample in edge.Samples)
            {
                cloud.Add(sample);
            }

            Vector3 average = cloud.Aggregate(Vector3.zero, (current, next) => current + next) / cloud.Count;
            float angle = edge.Samples.Select((s) => AngleDifference(s.Compass, s.Gyro.y)).Average();
            Quaternion rotation =
                Quaternion.AngleAxis(-angle, Vector3.up);
            cloud = new CloudPoint(cloud.Select((v) => rotation * (v - average)));
            return cloud;
        }

        private float SimilarCloudPoint(CloudPoint a, CloudPoint b)
        {
            if (a.Count() < b.Count() / 2 || a.Count() / 2 > b.Count())
            {
                return 0;
            }

            return a.ClosePoints(b, 0.1f);
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
            Color.magenta,
            new Color(0x8a, 0x2b, 0xe2),
            new Color(0xff, 0x7f, 0x50),
            new Color(0x64, 0x95, 0xed),
            new Color(0x8f, 0xbc, 0x8f),
            new Color(0x00, 0xbf, 0xff),
            new Color(0xff, 0xd7, 0x00),
            new Color(0xf0, 0x80, 0x80),
            new Color(0xff, 0xa5, 0x00),
            new Color(0x98, 0xfb, 0x98),
            new Color(0x66, 0x33, 0x99),
            new Color(0x87, 0xce, 0xeb),
            new Color(0x46, 0x82, 0xb4),
        };

        private int currentColor = 0;

        private readonly List<(Color, List<Edge>)> similarEdges = new();

        private void OnNewEdge(Edge edge)
        {
            if (edge == null)
            {
                return;
            }

            CloudPoint cloudPoint = EdgeToCloudPoint(edge);

            edge1Image.texture = cloudPoint.ToTexture(100, 100);
            Edge similarEdge = null;
            CloudPoint similarCloudPoint = null;
            var ratio = 0f;
            var index = -1;
            for (int i = 0; i < Inconsistencies.Instance.Edges.Count; i++)
            {
                CloudPoint otherCloudPoint = EdgeToCloudPoint(Inconsistencies.Instance.Edges[i]);
                var currentRatio = SimilarCloudPoint(otherCloudPoint, cloudPoint);
                if (currentRatio > ratio)
                {
                    similarEdge = Inconsistencies.Instance.Edges[i];
                    similarCloudPoint = otherCloudPoint;
                    ratio = currentRatio;
                    index = i;
                }
            }

            Debug.Log($"New Edge. Ratio: {ratio}");

            if (index >= 0 && ratio > 0.5f)
            {
                edge2Image.gameObject.SetActive(true);
                edge2Image.texture = similarCloudPoint.ToTexture(100, 100);

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
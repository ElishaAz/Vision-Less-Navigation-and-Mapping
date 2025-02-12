using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mapping
{
    public class EdgeSimilarity : MonoBehaviour
    {
        [SerializeField] private RawImage edge1Image;
        [SerializeField] private RawImage edge2Image;

        private static List<Vector3> EdgeToCloudPoint(Edge edge)
        {
            List<Vector3> cloud = new List<Vector3>();

            foreach (var sample in edge.Samples)
            {
                if (sample.FrontRight < 5)
                    cloud.Add(sample.FrontRightPosition);
                if (sample.FrontLeft < 5)
                    cloud.Add(sample.FrontLeftPosition);
                if (sample.BackRight < 5)
                    cloud.Add(sample.BackRightPosition);
                if (sample.BackLeft < 5)
                    cloud.Add(sample.BackLeftPosition);
            }

            Vector3 firstAverage = cloud.Aggregate(Vector3.zero, (current, next) => current + next) / cloud.Count;
            Quaternion firstRotation =
                Quaternion.AngleAxis(-edge.Samples.Select((s) => s.Compass).Average(), Vector3.up);
            cloud = cloud.Select((v) => firstRotation * (v - firstAverage)).ToList();
            return cloud;
        }

        private static Texture2D CloudPointToTexture(List<Vector3> cloudPoint, int width, int height)
        {
            var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var maxWidth = Mathf.Max(cloudPoint.Max(v => v.x), -cloudPoint.Min(v => v.x));
            var maxHeight = Mathf.Max(cloudPoint.Max(v => v.z), -cloudPoint.Min(v => v.z));

            var scale = Mathf.Max(width / maxWidth, height / maxHeight);

            var color = new Color(0xff, 0xff, 0xff, 0xff);

            Color32[] colors = tex.GetPixels32();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color32(0x00, 0x00, 0x00, 0xff);
            }

            tex.SetPixels32(colors);
            tex.Apply();

            foreach (var point in cloudPoint)
            {
                // var projected = Vector3.ProjectOnPlane(point, Vector3.up);

                // colors[width / 2 + (int)(projected.x * scale) + width * (height / 2 + (int)(projected.z * scale))] =
                // new Color32(0x00, 0xff, 0x00, 0xff);

                tex.SetPixel(width / 2 + (int)(point.x * scale), height / 2 + (int)(point.z * scale), color);
            }

            Debug.Log($"Width: {maxWidth} Height: {maxHeight}");

            Debug.Log(cloudPoint.Select(v => v.ToString()).Aggregate((a, b) => $"{a}, {b}"));

            tex.Apply();

            return tex;
        }

        private float SimilarEdge(Edge a, Edge b)
        {
            // DTWDistance<Vector3> distance = Vector3.Distance;

            // return MyDTW.DTW(a.NormalizedPositions(), b.NormalizedPositions(), distance).Item1;

            if (a.Samples.Count() < b.Samples.Count() / 2 || a.Samples.Count() / 2 > b.Samples.Count())
            {
                return 0;
            }


            List<Vector3> first = EdgeToCloudPoint(a);
            List<Vector3> second = EdgeToCloudPoint(b);

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

            edge1Image.texture = CloudPointToTexture(EdgeToCloudPoint(edge), 100, 100);
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
                edge2Image.texture = CloudPointToTexture(EdgeToCloudPoint(similarEdge), 100, 100);

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
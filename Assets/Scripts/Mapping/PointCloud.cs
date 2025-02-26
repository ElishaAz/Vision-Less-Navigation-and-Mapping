using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mapping
{
    public class PointCloud
    {
        private List<Vector3> points;

        public int Count => points.Count;

        public PointCloud()
        {
            points = new List<Vector3>();
        }

        public PointCloud(IEnumerable<Vector3> collection)
        {
            points = new List<Vector3>(collection);
        }

        public PointCloud(int capacity)
        {
            points = new List<Vector3>(capacity);
        }

        public static PointCloud FromSamples(IReadOnlyList<Sample> samples)
        {
            PointCloud cloud = new PointCloud();

            foreach (var sample in samples)
            {
                cloud.Add(sample);
            }

            Vector3 average = cloud.points.Aggregate(Vector3.zero, (current, next) => current + next) / cloud.Count;
            float angle = samples.Select((s) => AngleDifference(s.Compass, s.Gyro.y)).Average();
            Quaternion rotation =
                Quaternion.AngleAxis(-angle, Vector3.up);

            for (int i = 0; i < cloud.Count; i++)
            {
                cloud.points[i] = rotation * (cloud.points[i] - average);
            }

            return cloud;
        }

        private static float AngleDifference(float angle1, float angle2)
        {
            float a = angle2 - angle1;
            if (a > 180)
                a -= 360;
            if (a < -180)
                a += 360;
            return a;
        }

        public void AddPoint(Vector3 point)
        {
            points.Add(point);
        }

        public void Add(Sample sample, Vector3 offset = default)
        {
            if (sample.FrontRight < 5)
                AddPoint(sample.FrontRightPosition + offset);
            if (sample.FrontLeft < 5)
                AddPoint(sample.FrontLeftPosition + offset);
            if (sample.BackRight < 5)
                AddPoint(sample.BackRightPosition + offset);
            if (sample.BackLeft < 5)
                AddPoint(sample.BackLeftPosition + offset);
        }

        private float MinDistance(Vector3 point)
        {
            float minDistance = float.MaxValue;
            foreach (var p in points)
            {
                var d = Vector3.Distance(point, p);
                if (d < minDistance)
                    minDistance = d;
            }

            return minDistance;
        }


        public float ClosePoints(PointCloud second, float maxDistance)
        {
            float count = 0;

            foreach (var p in second.points)
            {
                if (MinDistance(p) < maxDistance)
                {
                    count++;
                }
            }

            return count / second.Count;
        }

        public Texture2D ToTexture(int width, int height)
        {
            var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            ToTexture(tex);
            return tex;
        }


        public void ToTexture(Texture2D tex)
        {
            var maxWidth = Mathf.Max(points.Max(v => v.x), -points.Min(v => v.x));
            var maxHeight = Mathf.Max(points.Max(v => v.z), -points.Min(v => v.z));

            var scale = Mathf.Min(tex.width / maxWidth, tex.height / maxHeight) / 2;

            var color = new Color(0xff, 0xff, 0xff, 0xff);

            Color32[] colors = tex.GetPixels32();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color32(0x00, 0x00, 0x00, 0xff);
            }

            tex.SetPixels32(colors);
            tex.Apply();

            foreach (var point in points)
            {
                tex.SetPixel(tex.width / 2 + (int)(point.x * scale), tex.height / 2 + (int)(point.z * scale), color);
            }

            tex.Apply();
        }

        public void Clear()
        {
            points.Clear();
        }
    }
}
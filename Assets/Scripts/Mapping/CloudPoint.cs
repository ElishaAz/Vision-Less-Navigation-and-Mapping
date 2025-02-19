using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mapping
{
    public class CloudPoint : List<Vector3>
    {
        public CloudPoint()
        {
        }

        public CloudPoint(IEnumerable<Vector3> collection) : base(collection)
        {
        }

        public CloudPoint(int capacity) : base(capacity)
        {
        }

        private void AddPoint(Vector3 point)
        {
            base.Add(point);
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
            foreach (var p in this)
            {
                var d = Vector3.Distance(point, p);
                if (d < minDistance)
                    minDistance = d;
            }

            return minDistance;
        }


        public float ClosePoints(CloudPoint second, float maxDistance)
        {
            float count = 0;

            foreach (var p in second)
            {
                if (this.MinDistance(p) < maxDistance)
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
            var maxWidth = Mathf.Max(this.Max(v => v.x), -this.Min(v => v.x));
            var maxHeight = Mathf.Max(this.Max(v => v.z), -this.Min(v => v.z));

            var scale = Mathf.Min(tex.width / maxWidth, tex.height / maxHeight) / 2;

            var color = new Color(0xff, 0xff, 0xff, 0xff);

            Color32[] colors = tex.GetPixels32();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color32(0x00, 0x00, 0x00, 0xff);
            }

            tex.SetPixels32(colors);
            tex.Apply();

            foreach (var point in this)
            {
                // var projected = Vector3.ProjectOnPlane(point, Vector3.up);

                // colors[width / 2 + (int)(projected.x * scale) + width * (height / 2 + (int)(projected.z * scale))] =
                // new Color32(0x00, 0xff, 0x00, 0xff);

                tex.SetPixel(tex.width / 2 + (int)(point.x * scale), tex.height / 2 + (int)(point.z * scale), color);
            }

            tex.Apply();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mapping
{
    public class Edge
    {
        public readonly Node From;
        public readonly Node To;
        private readonly List<Sample> samples = new List<Sample>();
        public IReadOnlyList<Sample> Samples => samples;

        private GameObject prefab;

        public readonly PointCloud PointCloud;

        public Edge(Node from, Node to, GameObject prefab, IReadOnlyList<Sample> samples)
        {
            From = from;
            To = to;
            this.prefab = prefab;
            this.samples.AddRange(samples);

            PointCloud = PointCloud.FromSamples(samples);

            UpdatePosition();
        }

        public void UpdatePosition()
        {
            prefab.transform.position = (From.Position + To.Position) / 2 + DroneView.DroneView.Offset;
            prefab.transform.rotation = Quaternion.LookRotation(To.Position - From.Position);
            prefab.transform.localScale = new Vector3(1, 1, Vector3.Distance(From.Position, To.Position));
        }

        public IReadOnlyList<Vector3> NormalizedPositions()
        {
            Vector3 average = Samples.Select((s) => s.Position).Aggregate((a, b) => a + b) / samples.Count;
            return Samples.Select((s) => s.Position - average).ToList();
        }

        public void SetColor(Color color)
        {
            prefab.GetComponentInChildren<MeshRenderer>().material.color = color;
        }
    }
}
using System;
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

        [NonSerialized] public readonly PointCloud PointCloud;

        public Edge(Node from, Node to, IReadOnlyList<Sample> samples)
        {
            From = from;
            To = to;
            this.samples.AddRange(samples);

            PointCloud = PointCloud.FromSamples(samples);
        }

        public IReadOnlyList<Vector3> NormalizedPositions()
        {
            Vector3 average = Samples.Select((s) => s.Position).Aggregate((a, b) => a + b) / samples.Count;
            return Samples.Select((s) => s.Position - average).ToList();
        }
    }
}
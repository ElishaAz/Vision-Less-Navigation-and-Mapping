using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mapping.Data
{
    [Serializable]
    public class Edge
    {
        [SerializeField] private Node from;
        [SerializeField] private Node to;

        public Node From => from;
        public Node To => to;

        [SerializeField] private List<Sample> samples = new List<Sample>();
        public IReadOnlyList<Sample> Samples => samples;

        [NonSerialized] public readonly PointCloud PointCloud;

        public Edge(Node from, Node to, IReadOnlyList<Sample> samples)
        {
            this.from = from;
            this.to = to;
            this.samples.AddRange(samples);

            PointCloud = PointCloud.FromSamples(samples);
        }

        public IReadOnlyList<Vector3> NormalizedPositions()
        {
            Vector3 average = Samples.Select((s) => s.Position).Aggregate((a, b) => a + b) / samples.Count;
            return Samples.Select((s) => s.Position - average).ToList();
        }

        public static bool operator ==(Edge left, Edge right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !(left == right);
        }

        protected bool Equals(Edge other)
        {
            return Equals(from, other.from) && Equals(to, other.to) && Equals(samples, other.samples);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Edge)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(from, to, samples);
        }
    }
}
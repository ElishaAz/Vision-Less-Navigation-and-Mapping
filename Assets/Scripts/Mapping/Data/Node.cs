using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mapping.Data
{
    [Serializable]
    public class Node
    {
        [SerializeField] private List<Sample> befores = new List<Sample>();
        [SerializeField] private List<Sample> afters = new List<Sample>();

        /// <summary>
        /// A list of samples, before the inconsistency.
        /// </summary>
        public IReadOnlyList<Sample> Befores => befores;

        /// <summary>
        /// A list of samples, after the inconsistency.
        /// </summary>
        public IReadOnlyList<Sample> Afters => afters;

        public void AddSample(Sample before, Sample after)
        {
            befores.Add(before);
            afters.Add(after);
            Position = Befores.Aggregate(Vector3.zero,
                           (current, sample) => current + sample.Position)
                       / Befores.Count;
        }

        /// <summary>
        /// Average of all After positions
        /// </summary>
        [field: NonSerialized]
        public Vector3 Position { get; private set; }

        public Node()
        {
        }

        public Node(IReadOnlyList<Sample> befores, IReadOnlyList<Sample> afters)
        {
            this.befores.AddRange(befores);
            this.afters.AddRange(afters);
            Position = Befores.Aggregate(Vector3.zero,
                           (current, sample) => current + sample.Position)
                       / Befores.Count;
        }

        public static bool operator ==(Node a, Node b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Node a, Node b)
        {
            return !(a == b);
        }

        protected bool Equals(Node other)
        {
            return Equals(befores, other.befores) && Equals(afters, other.afters);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(befores, afters);
        }
    }
}
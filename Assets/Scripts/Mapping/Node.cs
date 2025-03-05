using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mapping
{
    public class Node
    {
        private readonly List<(Sample, Sample)> samples = new List<(Sample, Sample)>();

        /// <summary>
        /// A list of samples, before and after the inconsistency.
        /// </summary>
        public IReadOnlyList<(Sample, Sample)> Samples => samples;

        public void AddSample(Sample before, Sample after)
        {
            samples.Add((before, after));
            Position = Samples.Aggregate(Vector3.zero,
                           (current, sample) => current + sample.Item2.Position)
                       / Samples.Count;
        }

        /// <summary>
        /// Average of all After positions
        /// </summary>
        [field: NonSerialized] public Vector3 Position { get; private set; }

        private readonly GameObject prefab;

        public Node()
        {
        }

        public Node(List<(Sample, Sample)> samples)
        {
            this.samples.AddRange(samples);
            Position = Samples.Aggregate(Vector3.zero,
                           (current, sample) => current + sample.Item2.Position)
                       / Samples.Count;
        }
    }
}
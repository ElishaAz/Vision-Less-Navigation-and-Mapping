using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mapping.Data
{
    [Serializable]
    public class Map
    {
        [SerializeField] private List<Node> nodes = new List<Node>();
        [SerializeField] private List<Edge> edges = new List<Edge>();


        [NonSerialized]
        private Dictionary<Tuple<int, int>, float> similarityMatrix = new Dictionary<Tuple<int, int>, float>();

        public IReadOnlyList<Node> Nodes => nodes;
        public IReadOnlyList<Edge> Edges => edges;

        public void AddEdge(Edge edge)
        {
            edges.Add(edge);
        }

        public void AddSimilarity(int index1, int index2, float similarity)
        {
            if (index2 < index1)
            {
                (index1, index2) = (index2, index1);
            }

            similarityMatrix.Add(new Tuple<int, int>(index1, index2), similarity);
        }

        public float GetSimilarity(int index1, int index2)
        {
            if (index2 < index1)
            {
                (index1, index2) = (index2, index1);
            }

            return similarityMatrix[new Tuple<int, int>(index1, index2)];
        }

        public float GetSimilarity(Edge a, Edge b)
        {
            var index1 = edges.IndexOf(a);
            var index2 = edges.IndexOf(b);

            return GetSimilarity(index1, index2);
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public static bool operator ==(Map left, Map right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Map left, Map right)
        {
            return !(left == right);
        }

        protected bool Equals(Map other)
        {
            return Equals(nodes, other.nodes) && Equals(edges, other.edges);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Map)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nodes, edges);
        }
    }
}
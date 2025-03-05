using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mapping
{
    [Serializable]
    public class Map
    {
        [SerializeField] private List<Node> nodes = new List<Node>();
        [SerializeField] private List<Edge> edges = new List<Edge>();

        public IReadOnlyList<Node> Nodes => nodes;
        public IReadOnlyList<Edge> Edges => edges;

        public void AddEdge(Edge edge)
        {
            edges.Add(edge);
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
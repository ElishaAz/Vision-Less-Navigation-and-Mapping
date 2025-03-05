using System.Collections.Generic;

namespace Mapping
{
    public class Map
    {
        private readonly List<Node> nodes = new List<Node>();
        private readonly List<Edge> edges = new List<Edge>();

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
    }
}
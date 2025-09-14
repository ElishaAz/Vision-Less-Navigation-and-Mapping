using System.Collections.Generic;
using Mapping.Data;

namespace Mapping.Algorithms
{
    public class PathPlanner
    {
        private const float SimilarityThreshold = 0.5f;

        private Map map;
        private Node target;
        private Edge current;

        private List<Node> path;

        public PathPlanner(Map map, Node target, Edge current)
        {
            this.map = map;
            this.target = target;
            this.current = current;
        }

        private void Plan()
        {
            path.Clear();
            path.Add(target);

            if (target.From != null)
            {
                var fromEdge = target.From;
            }
        }

        public Node GetNextNode()
        {
            return null;
        }
    }
}
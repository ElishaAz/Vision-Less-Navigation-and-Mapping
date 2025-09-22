using System.Linq;
using Mapping.Data;
using UnityEngine;

namespace Mapping.Algorithms
{
    public static class EdgeSimilarity
    {
        public static float Recall => found / (float)total;
        public static float Precision => found / (float)guessed;

        private static int total = 0;
        private static int found = 0;
        private static int guessed = 0;

        public static void EdgeAdded(Map map, Edge edge, int index)
        {
            PointCloud pointCloud = PointCloud.FromSamples(edge.Samples);

            Vector3 edgePosition =
                edge.Samples.Select(s => s.PositionGT).Aggregate((a, b) => a + b) / edge.Samples.Count;

            for (int i = 0; i < index; i++)
            {
                PointCloud otherPointCloud = map.Edges[i].PointCloud;
                var currentRatio = PointCloud.SimilarPointCloud(otherPointCloud, pointCloud);
                map.AddSimilarity(i, index, currentRatio);

                if (currentRatio > 0.7f)
                {
                    guessed++;
                }

                // Ground Truth
                Vector3 otherEdgePosition = map.Edges[i].Samples.Select(s => s.PositionGT).Aggregate((a, b) => a + b) /
                                            map.Edges[i].Samples.Count;

                if (Vector3.Distance(edgePosition, otherEdgePosition) < 0.5f)
                {
                    // Same edge
                    total++;

                    if (currentRatio > 0.7f)
                    {
                        found++;
                    }
                }
            }
        }
    }
}
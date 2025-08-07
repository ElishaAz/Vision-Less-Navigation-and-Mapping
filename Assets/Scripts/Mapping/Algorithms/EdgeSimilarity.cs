using Mapping.Data;

namespace Mapping.Algorithms
{
    public static class EdgeSimilarity
    {
        public static void EdgeAdded(Map map, Edge edge, int index)
        {
            PointCloud pointCloud = PointCloud.FromSamples(edge.Samples);

            for (int i = 0; i < index; i++)
            {
                PointCloud otherPointCloud = map.Edges[i].PointCloud;
                var currentRatio = PointCloud.SimilarPointCloud(otherPointCloud, pointCloud);
                map.AddSimilarity(i, index, currentRatio);
            }
        }
    }
}
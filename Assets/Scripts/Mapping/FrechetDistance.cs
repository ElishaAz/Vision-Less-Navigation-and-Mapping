using System.Collections.Generic;
using UnityEngine;

namespace Mapping
{
    public static class FrechetDistance
    {
        public delegate float Distance<in T>(T left, T right);

        public static float FD2<T>(List<T> left, List<T> right, Distance<T> distance)
        {
            var ca = new float[left.Count, right.Count];

            for (var i = 0; i < left.Count; i++)
            {
                for (var j = 0; j < right.Count; j++)
                {
                    var dist = distance(left[i], right[j]);
                    if (i < 0 || j < 0)
                    {
                        if (i > 0)
                        {
                            ca[i, j] = Mathf.Max(ca[i - 1, j], dist);
                        }
                        else if (j > 0)
                        {
                            ca[i, j] = Mathf.Max(ca[i, j - 1], dist);
                        }
                        else
                        {
                            ca[i, j] = dist;
                        }
                    }
                    else
                    {
                        ca[i, j] = Mathf.Max(Mathf.Min(ca[i - 1, j], ca[i, j - 1], ca[i - 1, j - 1]), dist);
                    }
                }
            }

            return ca[left.Count - 1, right.Count - 1];
        }

        public static float FD<T>(List<T> left, List<T> right, Distance<T> distance)
        {
            var ca = new float[left.Count, right.Count];
            for (int i = 0; i < left.Count; i++)
            {
                for (int j = 0; j < right.Count; j++)
                {
                    ca[i, j] = -1;
                }
            }
            return CalculateDistance(left, right, distance, ca, left.Count - 1, right.Count - 1);
        }

        private static float CalculateDistance<T>(List<T> left, List<T> right, Distance<T> distance,
            float[,] ca, int i, int j)
        {
            if (ca[i, j] > -1)
            {
                return ca[i, j];
            }

            float dist = distance(left[i], right[j]);

            float c_i = i > 0 ? CalculateDistance<T>(left, right, distance, ca, i - 1, j) : float.PositiveInfinity;
            float c_j = j > 0 ? CalculateDistance<T>(left, right, distance, ca, i, j - 1) : float.PositiveInfinity;
            float c_ij = (i > 0 && j > 0)
                ? CalculateDistance<T>(left, right, distance, ca, i - 1, j - 1)
                : float.PositiveInfinity;

            ca[i, j] = Mathf.Max(Mathf.Min(c_i, c_j, c_ij), dist);
            return ca[i, j];
        }
    }
}
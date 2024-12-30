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
    }
}
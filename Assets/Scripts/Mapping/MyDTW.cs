using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Mapping
{
    public static class MyDTW<T>
    {
        public static float DTW(IReadOnlyList<T> left, IReadOnlyList<T> right, DTWDistance<T> squareDistance)
        {
            var R = new float[right.Count, left.Count];

            for (var i = 0; i < left.Count; i++)
            {
                for (var j = 0; j < right.Count; j++)
                {
                    R[i, j] = squareDistance(left[i], right[j]);
                    if (i < 0 || j < 0)
                    {
                        if (i > 0)
                        {
                            R[i, j] += R[i - 1, j];
                        }
                        else if (j > 0)
                        {
                            R[i, j] += R[i, j - 1];
                        }
                    }
                    else
                    {
                        R[i, j] += Mathf.Min(R[i - 1, j], R[i, j - 1], R[i - 1, j - 1]);
                    }
                }
            }

            return Mathf.Sqrt(R[-1, -1]);
        }
        
        

        private static float Min(float a, float b, float c)
        {
            return Mathf.Min(a, b, c);
        }
    }
}
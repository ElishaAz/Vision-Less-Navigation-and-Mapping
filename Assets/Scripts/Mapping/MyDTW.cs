using System;
using System.Collections.Generic;

namespace Mapping
{
    public static class MyDTW
    {
        public static (float, List<(int, int)>) DTW<T>(IReadOnlyList<T> left, IReadOnlyList<T> right,
            DTWDistance<T> squareDistance)
        {
            var cost = new float[left.Count, right.Count];
            var path = new int[left.Count, right.Count]; // 0 - both, 1 - i, 2 - j

            for (var i = 0; i < left.Count; i++)
            {
                for (var j = 0; j < right.Count; j++)
                {
                    cost[i, j] = squareDistance(left[i], right[j]);
                    if (i <= 0 || j <= 0)
                    {
                        if (i > 0)
                        {
                            cost[i, j] += cost[i - 1, j];
                            path[i, j] = 1;
                        }
                        else if (j > 0)
                        {
                            cost[i, j] += cost[i, j - 1];
                            path[i, j] = 2;
                        }
                    }
                    else
                    {
                        var (min, index) = MinIndex(cost[i - 1, j - 1], cost[i - 1, j], cost[i, j - 1]);

                        cost[i, j] += min;
                        path[i, j] = index;
                    }
                }
            }

            var bestPath = new List<(int, int)>();
            {
                var i = left.Count - 1;
                var j = right.Count - 1;
                bestPath.Add((i, j));
                while (true)
                {
                    if (i == 0 && j == 0)
                        break;
                    if (i < 0 || j < 0)
                    {
                        // error
                    }

                    switch (path[i, j])
                    {
                        case 0:
                            bestPath.Add((i - 1, j - 1));
                            i -= 1;
                            j -= 1;
                            break;
                        case 1:
                            bestPath.Add((i - 1, j));
                            i -= 1;
                            break;
                        case 2:
                            bestPath.Add((i, j - 1));
                            j -= 1;
                            break;
                    }
                }
            }
            bestPath.Reverse();
            return ((float)Math.Sqrt(cost[left.Count - 1, right.Count - 1]), bestPath);
        }

        private static (float, int) MinIndex(params float[] array)
        {
            var minValue = float.MaxValue;
            var index = -1;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < minValue)
                {
                    minValue = array[i];
                    index = i;
                }
            }

            return (minValue, index);
        }
    }
}
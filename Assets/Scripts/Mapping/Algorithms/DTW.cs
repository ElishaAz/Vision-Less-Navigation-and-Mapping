using System.Collections.Generic;

// Based on https://gist.github.com/socrateslee/1966342

namespace Mapping.Algorithms
{
    public delegate float DTWDistance<in T>(T left, T right);

    public class DTW<T>
    {
        private IReadOnlyList<T> x;
        private IReadOnlyList<T> y;
        private float[,] distance;
        private float[,] f;
        private List<T> pathX;
        private List<T> pathY;
        private List<float> distanceList;
        private float sum;

        private DTWDistance<T> dist;

        public DTW(IReadOnlyList<T> x, IReadOnlyList<T> y, DTWDistance<T> dist)
        {
            this.x = x;
            this.y = y;
            this.dist = dist;

            distance = new float[x.Count, y.Count];
            f = new float[x.Count + 1, y.Count + 1];

            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < y.Count; j++)
                {
                    distance[i, j] = dist(x[i], y[j]);
                }
            }

            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < y.Count; j++)
                {
                    f[i, j] = -1f;
                }
            }

            for (int i = 0; i < x.Count; i++)
            {
                f[i, 0] = float.PositiveInfinity;
            }

            for (int j = 0; j < y.Count; j++)
            {
                f[0, j] = float.PositiveInfinity;
            }

            f[0, 0] = 0f;
            sum = 0f;

            pathX = new List<T>();
            pathY = new List<T>();
            distanceList = new List<float>();
        }

        public float ComputeDTW()
        {
            sum = ComputeFBackward(x.Count, y.Count);
            //sum = computeFForward();
            return sum;
        }

        public float ComputeFForward()
        {
            for (int i = 1; i <= x.Count; ++i)
            {
                for (int j = 1; j <= y.Count; ++j)
                {
                    if (f[i - 1, j] <= f[i - 1, j - 1] && f[i - 1, j] <= f[i, j - 1])
                    {
                        f[i, j] = distance[i - 1, j - 1] + f[i - 1, j];
                    }
                    else if (f[i, j - 1] <= f[i - 1, j - 1] && f[i, j - 1] <= f[i - 1, j])
                    {
                        f[i, j] = distance[i - 1, j - 1] + f[i, j - 1];
                    }
                    else if (f[i - 1, j - 1] <= f[i, j - 1] && f[i - 1, j - 1] <= f[i - 1, j])
                    {
                        f[i, j] = distance[i - 1, j - 1] + f[i - 1, j - 1];
                    }
                }
            }

            return f[x.Count, y.Count];
        }

        public float ComputeFBackward(int i, int j)
        {
            if (!(f[i, j] < 0.0))
            {
                return f[i, j];
            }
            else
            {
                if (ComputeFBackward(i - 1, j) <= ComputeFBackward(i, j - 1)
                    && ComputeFBackward(i - 1, j) <= ComputeFBackward(i - 1, j - 1)
                    && ComputeFBackward(i - 1, j) < float.PositiveInfinity)
                {
                    f[i, j] = distance[i - 1, j - 1] + ComputeFBackward(i - 1, j);
                }
                else if (ComputeFBackward(i, j - 1) <= ComputeFBackward(i - 1, j)
                         && ComputeFBackward(i, j - 1) <= ComputeFBackward(i - 1, j - 1)
                         && ComputeFBackward(i, j - 1) < float.PositiveInfinity)
                {
                    f[i, j] = distance[i - 1, j - 1] + ComputeFBackward(i, j - 1);
                }
                else if (ComputeFBackward(i - 1, j - 1) <= ComputeFBackward(i - 1, j)
                         && ComputeFBackward(i - 1, j - 1) <= ComputeFBackward(i, j - 1)
                         && ComputeFBackward(i - 1, j - 1) < float.PositiveInfinity)
                {
                    f[i, j] = distance[i - 1, j - 1] + ComputeFBackward(i - 1, j - 1);
                }
            }

            return f[i, j];
        }
    }
}
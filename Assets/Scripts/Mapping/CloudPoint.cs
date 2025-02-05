using System.Collections.Generic;
using System.Linq;
using Mono.Collections.Generic;
using UnityEngine;

namespace Mapping
{
    public static class CloudPoint
    {
        private static float MinDistance(Vector3 point, ICollection<Vector3> cloud)
        {
            float minDistance = float.MaxValue;
            foreach (var p in cloud)
            {
                var d = Vector3.Distance(point, p);
                if (d < minDistance)
                    minDistance = d;
            }

            return minDistance;
        }


        public static float ClosePoints(ICollection<Vector3> first, ICollection<Vector3> second, float maxDistance)
        {
            float count = 0;

            foreach (var p in second)
            {
                if (MinDistance(p, first) < maxDistance)
                {
                    count++;
                }
            }

            return count / second.Count;
        }
    }
}
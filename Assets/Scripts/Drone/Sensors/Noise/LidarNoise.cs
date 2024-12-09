using UnityEngine;

namespace Drone.Sensors.Noise
{
    public class LidarNoise
    {
        private readonly float minDistance;
        private readonly float maxDistance;
        public const float Inf = float.PositiveInfinity;

        public float Distance { get; private set; }

        public LidarNoise(float minDistance, float maxDistance)
        {
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            Distance = Inf;
        }

        public void Set(float distance)
        {
            if (float.IsInfinity(distance) || distance > maxDistance)
            {
                Distance = Inf;
            } else if (distance < minDistance)
            {
                Distance = -2;
            }
            else
            {
                if (NoiseParams.Instance.noiseEnabled)
                {
                    distance =
                        (distance + Random.Range(-NoiseParams.Instance.addLidar, NoiseParams.Instance.addLidar)) *
                        Random.Range(1 - NoiseParams.Instance.multLidar, 1 + NoiseParams.Instance.multLidar);
                }

                if (distance > maxDistance)
                {
                    Distance = Inf;
                } else if (distance < minDistance)
                {
                    Distance = -1;
                }
                else
                {
                    Distance = distance;
                }
            }
        }
    }
}
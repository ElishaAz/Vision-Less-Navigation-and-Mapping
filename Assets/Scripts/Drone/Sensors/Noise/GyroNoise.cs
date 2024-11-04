using UnityEngine;

namespace Drone.Sensors.Noise
{
    public class GyroNoise
    {
        private readonly float min;
        private readonly float max;
        private readonly bool wrapAround;
        private readonly float bias;

        public GyroNoise(float min = float.NegativeInfinity, float max = float.PositiveInfinity,
            bool wrapAround = false)
        {
            if (max <= min)
                throw new System.ArgumentException("max must be greater than min");
            this.min = min;
            this.max = max;
            this.wrapAround = wrapAround;
            float biasDirection = (Random.value > 0.5f) ? 1.0f : -1.0f;
            bias = biasDirection * Random.Range(NoiseParams.Instance.biasMinGyro, NoiseParams.Instance.biasMaxGyro);
        }

        public float Value { get; private set; }

        public void Set(float speed)
        {
            if (NoiseParams.Instance.noiseEnabled)
            {
                Value += (speed +
                          (GaussianNoise.NextGaussianFloat() * NoiseParams.Instance.gyroStd) + bias) *
                         Time.fixedDeltaTime;
            }
            else
            {
                Value += speed * Time.fixedDeltaTime;
            }

            while (Value < min)
            {
                if (wrapAround)
                {
                    float diff = Value - min;
                    Value = max + diff;
                }
                else
                {
                    Value = min;
                }
            }

            while (Value > max)
            {
                if (wrapAround)
                {
                    float diff = max - Value;
                    Value = min + diff;
                }
                else
                {
                    Value = max;
                }
            }
        }
    }
}
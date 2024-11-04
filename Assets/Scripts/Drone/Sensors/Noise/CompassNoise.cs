namespace Drone.Sensors.Noise
{
	public class CompassNoise
	{
		public float Value { get; private set; }

		public void Set(float value)
		{
			if (NoiseParams.Instance.noiseEnabled)
			{
				Value = value + GaussianNoise.NextGaussianFloat() * NoiseParams.Instance.compassStdDev;
			}
			else
			{
				Value = value;
			}
		}
	}
}
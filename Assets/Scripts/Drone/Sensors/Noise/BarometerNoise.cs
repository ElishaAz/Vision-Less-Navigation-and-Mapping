namespace Drone.Sensors.Noise
{
	public class BarometerNoise
	{
		public float Value { get; private set; }

		public void Set(float value)
		{
			Value = value;
		}
	}
}
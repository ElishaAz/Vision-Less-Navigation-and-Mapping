using UnityEngine;

namespace Drone.Sensors.Noise
{
	public class OpticalFlowNoise
	{
		public Vector2 Speed { get; private set; } = Vector2.zero;

		public OpticalFlowNoise()
		{
		}

		public void Set(Vector2 speed)
		{
			if (NoiseParams.Instance.noiseEnabled)
			{
				Speed = new Vector2(AddNoise(speed.x), AddNoise(speed.y));
			}
			else
			{
				Speed = speed;
			}
		}

		private float AddNoise(float value)
		{
			return (value + Random.Range(-NoiseParams.Instance.addOF, NoiseParams.Instance.addOF)) *
			       Random.Range(1 - NoiseParams.Instance.multOF, 1 + NoiseParams.Instance.multOF);
		}
	}
}
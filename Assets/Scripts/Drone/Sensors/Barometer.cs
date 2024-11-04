using Drone.Sensors.Noise;
using UnityEngine;

namespace Drone.Sensors
{
	public class Barometer : MonoBehaviour
	{
		private BarometerNoise noise;
		public float Value => noise.Value;

		private float startHeight;

		private void Awake()
		{
			noise = new BarometerNoise();
			startHeight = transform.position.y;
		}

		private void FixedUpdate()
		{
			noise.Set(transform.position.y - startHeight);
		}
	}
}
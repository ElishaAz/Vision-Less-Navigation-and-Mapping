using Drone.Sensors.Noise;
using UnityEngine;

namespace Drone.Sensors
{
	public class Compass : MonoBehaviour
	{
		private CompassNoise noise;
		public float Value => noise.Value;

		private void Awake()
		{
			noise = new CompassNoise();
		}

		private void FixedUpdate()
		{
			noise.Set(transform.eulerAngles.y);
		}
	}
}
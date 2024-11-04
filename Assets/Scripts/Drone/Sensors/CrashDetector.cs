using UnityEditor;
using UnityEngine;

namespace Drone.Sensors
{
	public class CrashDetector : MonoBehaviour
	{
		public bool InCrash { get; private set; }
		public int Crashes { get; private set; }

		private void OnCollisionEnter(Collision other)
		{
			if (Crashes == 0)
			{
				EditorApplication.isPaused = true;
			}
			InCrash = true;
			Crashes++;
		}

		private void OnCollisionExit(Collision other)
		{
			InCrash = false;
		}
	}
}
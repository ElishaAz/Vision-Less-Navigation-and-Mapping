using UnityEngine;
using UnityEngine.Serialization;

namespace Drone.Sensors.Noise
{
    public class NoiseParams : MonoBehaviour
    {
        public static NoiseParams Instance { get; private set; }

        public float addLidar = 0.01f;
        public float multLidar = 0.01f;

        public float addOF = 0.1f;
        public float multOF = 0.1f;

        public float gyro = 0.0001f;
        public float biasMinGyro = 0.5f;
        public float biasMaxGyro = 1.5f;
        public float gyroStd = 0.1f;

        public float compassStdDev = 1f; // degrees

        public bool noiseEnabled = false;

        public float deltaTime = 0.02f;

        private void Awake()
        {
            if (isActiveAndEnabled)
                Instance = this;
        }
    }
}
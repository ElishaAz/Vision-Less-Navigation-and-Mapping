using Drone.Sensors.Noise;
using UnityEngine;

namespace Drone.Sensors
{
    [RequireComponent(typeof(Rigidbody))]
    public class Gyro : MonoBehaviour
    {
        public float Roll { get; private set; }

        public float Pitch { get; private set; }

        private GyroNoise yawNoise;
        public float Yaw => NoiseParams.Instance.noiseEnabled ? yawNoise.Value : trueYaw;

        /// <summary>
        /// Orientation, Pitch Yaw Roll (rotation in X Y Z)
        /// </summary>
        public Vector3 Orientation => new Vector3(Pitch, Yaw, Roll);

        private Rigidbody rb;

        private float trueYaw;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            yawNoise = new GyroNoise(-180, 180, true);
        }


        private void FixedUpdate()
        {
            var orientation = transform.eulerAngles;
            Roll = orientation.z;
            if (Roll > 180)
            {
                Roll -= 360;
            }

            Pitch = orientation.x;

            if (Pitch > 180)
            {
                Pitch -= 360;
            }

            trueYaw = orientation.y;

            yawNoise.Set(Mathf.Rad2Deg * rb.angularVelocity.y);
        }
    }
}
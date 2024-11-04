using Drone.Sensors.Noise;
using UnityEngine;

namespace Drone
{
    [RequireComponent(typeof(Rigidbody))]
    public class Drone : MonoBehaviour
    {
        [SerializeField] private float maxRollAngle = 15;
        [SerializeField] private float rollSpeed = 1; // m/s

        [SerializeField] private float maxPitchAngle = 15;
        [SerializeField] private float pitchSpeed = 1; // m/s

        [SerializeField] private float rollPitchRate = 1; // multiplier per second

        [SerializeField] private float yawRate = 45; // d/s

        [SerializeField] private float thrustSpeed = 1; // m/s

        private Rigidbody rb;
        private float rollRC = 0;
        private float pitchRC = 0;
        private float yawRC = 0;
        private float thrustRC = 0;

        private float rollActual;
        private float pitchActual;

        private Vector3 wind = new Vector3(0.1f, 0, 0.01f);

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            rollActual = Mathf.MoveTowards(rollActual, rollRC, rollPitchRate * Time.fixedDeltaTime);
            pitchActual = Mathf.MoveTowards(pitchActual, pitchRC, rollPitchRate * Time.fixedDeltaTime);

            var velocity = new Vector3(rollActual * rollSpeed, thrustRC * thrustSpeed, pitchActual * pitchSpeed);
            var eulerAngles = transform.eulerAngles;
            rb.linearVelocity = Quaternion.AngleAxis(eulerAngles.y, Vector3.up) * velocity + wind;
            rb.MoveRotation(Quaternion.Euler(pitchActual * maxPitchAngle, eulerAngles.y, -rollActual * maxRollAngle));
            var vector3 = rb.angularVelocity;
            vector3.y = yawRC * yawRate * Mathf.Deg2Rad;
            rb.angularVelocity = vector3;
        }

        public void RC(float roll, float pitch, float yaw, float thrust)
        {
            rollRC = Mathf.Clamp(roll, -1, 1);
            pitchRC = Mathf.Clamp(pitch, -1, 1);
            yawRC = Mathf.Clamp(yaw, -1, 1);
            thrustRC = Mathf.Clamp(thrust, -1, 1);
        }
    }
}
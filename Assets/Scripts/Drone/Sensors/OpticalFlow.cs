using Drone.Sensors.Noise;
using UnityEngine;

namespace Drone.Sensors
{
    [RequireComponent(typeof(Rigidbody))]
    public class OpticalFlow : MonoBehaviour
    {
        private Rigidbody rb;
        private OpticalFlowNoise noise;

        public Vector2 Speed => noise.Speed;
        public Vector3 Speed3D => new Vector3(Speed.x, 0, Speed.y);
        public Vector2 Position { get; private set; }

        public Vector3 Position3D => new(Position.x, 0, Position.y);

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            noise = new OpticalFlowNoise();
        }

        private void FixedUpdate()
        {
            var speed3d = Quaternion.Inverse(transform.rotation) * rb.linearVelocity;
            var speed = new Vector2(speed3d.x, speed3d.z);
            noise.Set(speed);
            Position += Speed * Time.fixedDeltaTime;
        }
    }
}
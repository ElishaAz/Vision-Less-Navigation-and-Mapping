using System;
using Drone.Sensors;
using UnityEngine;
using Compass = Drone.Sensors.Compass;

namespace Drone
{
    public class DroneSensors : MonoBehaviour
    {
        [SerializeField] public Lidar right;
        [SerializeField] public Lidar left;

        [SerializeField] public Lidar front;

        [SerializeField] public Lidar back;
        [SerializeField] public Lidar up;
        [SerializeField] public Lidar down;

        [SerializeField] public OpticalFlow opticalFlow;

        [SerializeField] public Gyro gyro;
        [SerializeField] public Compass compass;
        [SerializeField] public Barometer barometer;

        [SerializeField] public CrashDetector crashDetector;

        public Quaternion DroneRotation { get; private set; } = Quaternion.identity;
        public Vector3 DronePosition => dronePosition;
        private Vector3 dronePosition = Vector3.zero;

        public Lidar[] Lidars { get; private set; }

        private void Awake()
        {
            Lidars = new[] { right, left, front, back, up, down };
        }

        private void FixedUpdate()
        {
            DroneRotation = Quaternion.Euler(gyro.Pitch, gyro.Yaw, gyro.Roll);
            var globalPosDif = DroneRotation * (opticalFlow.Speed3D * Time.fixedDeltaTime);
            globalPosDif.y = 0;
            dronePosition += globalPosDif;
            dronePosition.y = barometer.Value;
        }
    }
}
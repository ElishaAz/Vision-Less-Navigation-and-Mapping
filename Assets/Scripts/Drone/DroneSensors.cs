using System;
using Drone.Sensors;
using UnityEngine;
using UnityEngine.Serialization;
using Compass = Drone.Sensors.Compass;

namespace Drone
{
    public class DroneSensors : MonoBehaviour
    {
        [SerializeField] public Lidar frontRight;
        [SerializeField] public Lidar frontLeft;

        [SerializeField] public Lidar backRight;

        [SerializeField] public Lidar backLeft;

        [SerializeField] public Lidar up;
        [SerializeField] public Lidar down;

        [SerializeField] public OpticalFlow opticalFlow;

        [SerializeField] public Gyro gyro;
        [SerializeField] public Compass compass;
        [SerializeField] public Barometer barometer;

        [SerializeField] public CrashDetector crashDetector;

        // For the old algorithm:
        [SerializeField] public Lidar front;
        [SerializeField] public Lidar right;
        [SerializeField] public Lidar left;

        public Quaternion DroneRotation { get; private set; } = Quaternion.identity;
        public Vector3 DronePosition => dronePosition;
        private Vector3 dronePosition = Vector3.zero;

        public Lidar[] Lidars { get; private set; }

        private void Awake()
        {
            Lidars = new[] { frontRight, frontLeft, backRight, backLeft, up, down };
        }

        private void FixedUpdate()
        {
            DroneRotation = Quaternion.Euler(gyro.Pitch, gyro.Yaw, gyro.Roll);
            var globalPosDif = DroneRotation * (opticalFlow.Speed3D * Time.fixedDeltaTime);
            globalPosDif.y = 0;
            dronePosition += globalPosDif;
            dronePosition.y = barometer.Value;
        }

        public Vector3 PositionForLidar(Lidar lidar)
        {
            var lidarRotation = lidar.transform.localRotation;
            var lidarPosition = DronePosition +
                                (DroneRotation *
                                 (lidarRotation * Vector3.forward * lidar.Distance +
                                  lidar.transform.localPosition));
            return lidarPosition;
        }
    }
}
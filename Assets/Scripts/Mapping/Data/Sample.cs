using System;
using Drone;
using Drone.Sensors;
using UnityEngine;

namespace Mapping.Data
{
    [Serializable]
    public struct Sample
    {
        [SerializeField] private float frontRight;
        public float FrontRight => frontRight;
        [SerializeField] private Vector3 frontRightPosition;
        public Vector3 FrontRightPosition => frontRightPosition;

        [SerializeField] private float frontLeft;
        public readonly float FrontLeft => frontLeft;
        [SerializeField] private Vector3 frontLeftPosition;
        public readonly Vector3 FrontLeftPosition => frontLeftPosition;
        [SerializeField] private float up;
        public readonly float Up => up;
        [SerializeField] private Vector3 upPosition;
        public readonly Vector3 UpPosition => upPosition;
        [SerializeField] private float down;
        public readonly float Down => down;
        [SerializeField] private Vector3 downPosition;
        public readonly Vector3 DownPosition => downPosition;
        [SerializeField] private float backRight;
        public readonly float BackRight => backRight;
        [SerializeField] private Vector3 backRightPosition;
        public readonly Vector3 BackRightPosition => backRightPosition;
        [SerializeField] private float backLeft;
        public readonly float BackLeft => backLeft;
        [SerializeField] private Vector3 backLeftPosition;
        public readonly Vector3 BackLeftPosition => backLeftPosition;

        [SerializeField] private Vector3 position;
        public readonly Vector3 Position => position;
        [SerializeField] private Vector3 gyro;
        public readonly Vector3 Gyro => gyro;
        [SerializeField] private float compass;
        public readonly float Compass => compass;
        [SerializeField] private float time;
        public readonly float Time => time;

        private static Vector3 GetLidarPosition(Lidar lidar, DroneSensors droneSensors)
        {
            if (float.IsNaN(lidar.Distance) || float.IsInfinity(lidar.Distance))
            {
                return Vector3.zero;
            }

            return droneSensors.PositionForLidar(lidar);
        }

        public Sample(DroneSensors sensors, float time)
        {
            this.time = time;

            position = sensors.DronePosition;
            gyro = sensors.gyro.Orientation;
            compass = sensors.compass.Value;

            frontRight = sensors.frontRight.DistanceNormalized;
            frontRightPosition = GetLidarPosition(sensors.frontRight, sensors);
            frontLeft = sensors.frontLeft.DistanceNormalized;
            frontLeftPosition = GetLidarPosition(sensors.frontLeft, sensors);
            up = sensors.up.DistanceNormalized;
            upPosition = GetLidarPosition(sensors.up, sensors);
            down = sensors.down.DistanceNormalized;
            downPosition = GetLidarPosition(sensors.down, sensors);
            backRight = sensors.backRight.DistanceNormalized;
            backRightPosition = GetLidarPosition(sensors.backRight, sensors);
            backLeft = sensors.backLeft.DistanceNormalized;
            backLeftPosition = GetLidarPosition(sensors.backLeft, sensors);
        }
    }
}
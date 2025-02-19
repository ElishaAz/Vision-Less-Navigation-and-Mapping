using Drone;
using Drone.Sensors;
using UnityEngine;

namespace Mapping
{
    public readonly struct Sample
    {
        public readonly float FrontRight;
        public readonly Vector3 FrontRightPosition;

        public readonly float FrontLeft;
        public readonly Vector3 FrontLeftPosition;
        public readonly float Up;
        public readonly Vector3 UpPosition;
        public readonly float Down;
        public readonly Vector3 DownPosition;
        public readonly float BackRight;
        public readonly Vector3 BackRightPosition;
        public readonly float BackLeft;
        public readonly Vector3 BackLeftPosition;

        public readonly Vector3 Position;
        public readonly Vector3 Gyro;
        public readonly float Compass;
        public readonly float Time;

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
            Time = time;

            Position = sensors.DronePosition;
            Gyro = sensors.gyro.Orientation;
            Compass = sensors.compass.Value;

            FrontRight = sensors.frontRight.DistanceNormalized;
            FrontRightPosition = GetLidarPosition(sensors.frontRight, sensors);
            FrontLeft = sensors.frontLeft.DistanceNormalized;
            FrontLeftPosition = GetLidarPosition(sensors.frontLeft, sensors);
            Up = sensors.up.DistanceNormalized;
            UpPosition = GetLidarPosition(sensors.up, sensors);
            Down = sensors.down.DistanceNormalized;
            DownPosition = GetLidarPosition(sensors.down, sensors);
            BackRight = sensors.backRight.DistanceNormalized;
            BackRightPosition = GetLidarPosition(sensors.backRight, sensors);
            BackLeft = sensors.backLeft.DistanceNormalized;
            BackLeftPosition = GetLidarPosition(sensors.backLeft, sensors);
        }
    }
}
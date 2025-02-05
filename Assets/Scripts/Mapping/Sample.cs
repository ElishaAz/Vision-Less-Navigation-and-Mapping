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

        private static Vector3 GetLidarPosition(Lidar lidar, Vector3 dronePosition, Quaternion droneRotation)
        {
            var lidarRotation = lidar.transform.localRotation;
            var lidarPosition = dronePosition +
                                (droneRotation *
                                 (lidarRotation * Vector3.forward * lidar.DistanceNormalized +
                                  lidar.transform.localPosition));
            return lidarPosition;
        }

        public Sample(DroneSensors sensors, float time)
        {
            Time = time;

            Position = sensors.DronePosition;
            Gyro = sensors.gyro.Orientation;
            Compass = sensors.compass.Value;

            FrontRight = sensors.frontRight.DistanceNormalized;
            FrontRightPosition = GetLidarPosition(sensors.frontRight, Position, sensors.DroneRotation);
            FrontLeft = sensors.frontLeft.DistanceNormalized;
            FrontLeftPosition = GetLidarPosition(sensors.frontLeft, Position, sensors.DroneRotation);
            Up = sensors.up.DistanceNormalized;
            UpPosition = GetLidarPosition(sensors.up, Position, sensors.DroneRotation);
            Down = sensors.down.DistanceNormalized;
            DownPosition = GetLidarPosition(sensors.down, Position, sensors.DroneRotation);
            BackRight = sensors.backRight.DistanceNormalized;
            BackRightPosition = GetLidarPosition(sensors.backRight, Position, sensors.DroneRotation);
            BackLeft = sensors.backLeft.DistanceNormalized;
            BackLeftPosition = GetLidarPosition(sensors.backLeft, Position, sensors.DroneRotation);
        }
    }
}
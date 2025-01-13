using System.Collections.Generic;
using Drone;
using UnityEngine;

namespace Mapping
{
    public class DataCollector : MonoBehaviour
    {
        public static DataCollector Instance { get; private set; }

        [SerializeField] private Drone.Drone drone;
        [SerializeField] private DroneSensors sensors;

        // [SerializeField] private RawImage rawImage;
        //
        // private int textureWidth = 128;
        // private int textureHeight = 128;
        // private Texture2D texture;

        [SerializeField] private float interval = 0.1f;


        private float startTime;
        private readonly List<DataPoint> history = new List<DataPoint>();

        public IReadOnlyList<DataPoint> History => history;
        
        public DataPoint CurrentPoint { get; private set; }

        private void Awake()
        {
            if (isActiveAndEnabled)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            // texture = new Texture2D(textureWidth, textureHeight);
            //
            // for (int x = 0; x < textureWidth; x++)
            // for (int y = 0; y < textureHeight; y++)
            // {
            //     texture.SetPixel(x, y, Color.white);
            // }
            //
            // texture.Apply();
            //
            // rawImage.texture = texture;

            startTime = Time.time;
        }

        private float nextUpdate;

        private void FixedUpdate()
        {

            nextUpdate -= Time.fixedDeltaTime;

            if (nextUpdate <= 0)
            {
                AddPoint();
                nextUpdate = interval;
            }

            // // Vector2 position = sensors.opticalFlow.Position;
            // Vector2 position2D = new Vector2(position.x, position.z);
            // // float yaw = drone.transform.eulerAngles.y;
            // // float yaw = sensors.gyro.Yaw;
            // float yaw = sensors.gyro.Yaw;
            //
            // Vector2 rightWall = position2D + GetLidar(sensors.right.Distance, yaw - 90);
            // Vector2 leftWall = position2D + GetLidar(sensors.left.Distance, yaw + 90);
            //
            // SetPixel(rightWall, Color.black);
            // SetPixel(leftWall, Color.blue);
        }

        private void AddPoint()
        {
            CurrentPoint = new DataPoint(Time.time - startTime, sensors.DronePosition, sensors.DroneRotation,
                left: sensors.frontLeft.Distance, right: sensors.frontRight.Distance, front: sensors.backRight.Distance,
                back: sensors.backLeft.Distance,
                up: sensors.up.Distance, down: sensors.down.Distance);

            history.Add(CurrentPoint);
        }
        //
        // private Vector2 GetLidar(float distance, float angle)
        // {
        //     return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad),
        //         Mathf.Cos(angle * Mathf.Deg2Rad) * distance);
        // }
        //
        // private void SetPixel(Vector2 pixel, Color color)
        // {
        //     int x = (int)(pixel.x * 2 + textureWidth / 2);
        //     int y = (int)(pixel.y * 2 + textureHeight / 2);
        //     if (x >= 0 && y >= 0 && x < textureWidth && y < textureHeight)
        //     {
        //         texture.SetPixel(x, y, color);
        //         texture.Apply();
        //     }
        // }
    }
}
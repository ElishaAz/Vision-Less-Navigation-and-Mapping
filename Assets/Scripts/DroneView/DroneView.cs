using System;
using System.Collections;
using Drone;
using UnityEngine;

namespace DroneView
{
    public class DroneView : MonoBehaviour
    {
        public static Vector3 Offset { get; private set; }

        [SerializeField] private Drone.Drone drone;
        [SerializeField] private DroneSensors sensors;

        [SerializeField] private Vector3 offset;
        [SerializeField] private GameObject[] lidarPrefs;
        [SerializeField] private GameObject[] lidarSticks;
        [SerializeField] private Transform holder;

        [SerializeField] private float interval = 0.1f;

        private void Awake()
        {
            Offset = offset;
        }

        private void Start()
        {
            transform.position = offset + sensors.opticalFlow.Position3D;
            transform.eulerAngles = new Vector3(sensors.gyro.Pitch, sensors.gyro.Yaw, sensors.gyro.Roll);
            StartCoroutine(DrawLidars());
        }

        private void FixedUpdate()
        {
            transform.position = offset + sensors.DronePosition;
            transform.rotation = sensors.DroneRotation;
        }

        private IEnumerator DrawLidars()
        {
            while (true)
            {
                var dronePosition = offset + sensors.DronePosition;
                var droneRotation = sensors.DroneRotation;

                for (var i = 0; i < sensors.Lidars.Length; i++)
                {
                    var lidar = sensors.Lidars[i];
                    if (float.IsNaN(lidar.Distance) || float.IsInfinity(lidar.Distance))
                    {
                        if (i < lidarSticks.Length)
                            lidarSticks[i]?.SetActive(false);
                        continue;
                    }

                    var spawnPosition = sensors.PositionForLidar(lidar) + offset;

                    if (lidarPrefs[i] != null)
                    {
                        var lidarPos = Instantiate(lidarPrefs[i], spawnPosition, Quaternion.identity);
                        lidarPos.transform.SetParent(holder);
                    }

                    if (i < lidarSticks.Length && lidarSticks[i] != null)
                    {
                        var stick = lidarSticks[i];
                        stick.SetActive(true);

                        var diff = spawnPosition - dronePosition;
                        stick.transform.position = (dronePosition + spawnPosition) / 2;
                        stick.transform.rotation = Quaternion.LookRotation(diff);
                        stick.transform.localScale = new Vector3(1, 1, Vector3.Distance(dronePosition, spawnPosition));
                    }
                }

                yield return new WaitForSeconds(interval);
            }
        }
    }
}
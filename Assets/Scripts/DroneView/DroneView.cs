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
                    if (lidarPrefs[i] == null) continue;
                    
                    var lidar = sensors.Lidars[i];
                    if (float.IsNaN(lidar.Distance)) continue;
                    if (float.IsInfinity(lidar.Distance)) continue;
                    var lidarRotation = lidar.transform.localRotation;

                    var spawnPosition = dronePosition +
                                        (droneRotation *
                                         (lidarRotation * Vector3.forward * lidar.Distance +
                                          lidar.transform.localPosition));
                    var lidarPos = Instantiate(lidarPrefs[i], spawnPosition, lidarRotation * droneRotation);
                    lidarPos.transform.SetParent(holder);
                }
                yield return new WaitForSeconds(interval);
            }
        }
    }
}

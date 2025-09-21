using System;
using Drone;
using UnityEngine;

namespace Mapping
{
    public class Coverage : MonoBehaviour
    {
        [SerializeField] private Drone.Drone drone;
        [SerializeField] private DroneSensors sensors;
        [SerializeField] private float scale;
        [SerializeField] private float yScale;
        [SerializeField] private Transform area;

        private bool[,,] collected;

        public int TotalCollected { get; private set; }

        public float Collected => TotalCollected /
                                  ((float)area.lossyScale.x / scale * area.lossyScale.y / yScale * area.lossyScale.z /
                                   scale);

        private void Awake()
        {
            collected = new bool[(int)(area.lossyScale.x / scale), (int)(area.lossyScale.y / yScale),
                (int)(area.lossyScale.z / scale)];
        }

        private void FixedUpdate()
        {
            foreach (var lidar in sensors.Lidars)
            {
                if (!lidar.IsValid) continue;

                // True position for Lidar
                var lidarRotation = lidar.transform.localRotation;
                var lidarPosition = drone.transform.position +
                                    (drone.transform.rotation *
                                     (lidarRotation * Vector3.forward * lidar.Distance +
                                      lidar.transform.localPosition));

                var collectPosition = drone.transform.position;
                while (Vector3.Distance(lidarPosition, collectPosition) >= scale)
                {
                    Collect(collectPosition);
                    collectPosition = Vector3.MoveTowards(collectPosition, lidarPosition, scale);
                }
            }
        }

        private void Collect(Vector3 position)
        {
            int x = (int)Mathf.Clamp(position.x / scale + area.lossyScale.x / scale / 2, 0, collected.GetLength(0) - 1);
            int y = (int)Mathf.Clamp(position.y / scale + area.lossyScale.y / yScale / 2, 0, collected.GetLength(1) - 1);
            int z = (int)Mathf.Clamp(position.z / scale + area.lossyScale.z / scale / 2, 0, collected.GetLength(2) - 1);
            

            if (collected[x, y, z]) return;
            collected[x, y, z] = true;
            TotalCollected++;
        }
    }
}
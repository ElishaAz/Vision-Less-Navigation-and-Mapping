using System;
using System.Collections;
using System.Linq;
using Drone;
using UnityEngine;

namespace Mapping
{
    public class Coverage : MonoBehaviour
    {
        [SerializeField] private Drone.Drone drone;
        [SerializeField] private DroneSensors sensors;
        [Range(0.01f, 10f)] [SerializeField] private float scale;
        [Range(0.01f, 20f)] [SerializeField] private float yScale;
        [SerializeField] private float interval = 0.1f;

        [SerializeField] private bool useOldLidars;


        public int TotalCollected => CoverageArea.Areas.Select(a => a.TotalCollected).Sum();
        public int TotalPoints => CoverageArea.Areas.Select(a => a.TotalPoints).Sum();

        public float Collected => TotalCollected / (float)TotalPoints;

        private void Start()
        {
            foreach (var area in CoverageArea.Areas)
            {
                area.Create(scale, yScale);
            }
        }

        private float lastUpdate;

        private void FixedUpdate()
        {
            if (Time.timeSinceLevelLoad - lastUpdate < interval) return;
            lastUpdate = Time.timeSinceLevelLoad;

            foreach (var lidar in (useOldLidars ? sensors.OldLidars : sensors.Lidars))
            {
                if (float.IsNaN(lidar.Distance)) continue;

                // True position for Lidar
                var lidarRotation = lidar.transform.localRotation;
                var lidarPosition = drone.transform.position +
                                    (drone.transform.rotation *
                                     (lidarRotation * Vector3.forward * lidar.DistanceNormalized +
                                      lidar.transform.localPosition));

                var collectPosition = drone.transform.position;
                while (Vector3.Distance(lidarPosition, collectPosition) >= scale)
                {
                    Collect(collectPosition);
                    collectPosition =
                        Vector3.MoveTowards(collectPosition, lidarPosition, scale);
                }
            }
        }

        private void Collect(Vector3 position)
        {
            foreach (var area in CoverageArea.Areas)
            {
                area.Collect(position);
            }
        }

        public void SetUseOldLidars(bool use)
        {
            useOldLidars = use;
        }
    }
}
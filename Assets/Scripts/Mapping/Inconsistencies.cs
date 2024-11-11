using System;
using Drone;
using UnityEngine;

namespace Mapping
{
    public class Inconsistencies : MonoBehaviour
    {
        [SerializeField] private DroneSensors sensors;
        [SerializeField] private float threshold = 1f;
        [SerializeField] private GameObject prefab;

        private float lastRight;
        private float lastLeft;

        private void Start()
        {
            lastRight = sensors.right.DistanceNormalized;
            lastLeft = sensors.left.DistanceNormalized;
        }

        private void FixedUpdate()
        {
            float right = sensors.right.DistanceNormalized;
            float left = sensors.left.DistanceNormalized;

            if (Mathf.Abs(right - lastRight) > threshold || Mathf.Abs(left - lastLeft) > threshold)
            {
                Instantiate(prefab, sensors.DronePosition + DroneView.DroneView.Offset, Quaternion.identity, transform);
            }
            lastRight = right;
            lastLeft = left;
        }
    }
}
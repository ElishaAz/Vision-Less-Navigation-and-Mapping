using System;
using System.Collections.Generic;
using Drone;
using UnityEngine;

namespace Mapping
{
    public class Inconsistencies : MonoBehaviour
    {
        [SerializeField] private DroneSensors sensors;
        [SerializeField] private float threshold = 1f;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int maxBackMerge = 5;
        [SerializeField] private float mergeDistance = 1f;

        private struct Inconsistency
        {
            public Vector3 Position;
            public int Count;
            public GameObject Prefab;
        }

        private readonly List<Inconsistency> inconsistencies = new List<Inconsistency>();

        private float lastRight;
        private float lastLeft;

        private void Start()
        {
            lastRight = sensors.right.DistanceNormalized;
            lastLeft = sensors.left.DistanceNormalized;
        }

        private void FixedUpdate()
        {
            var right = sensors.right.DistanceNormalized;
            var left = sensors.left.DistanceNormalized;
            var position = sensors.DronePosition;

            if (Mathf.Abs(right - lastRight) > threshold || Mathf.Abs(left - lastLeft) > threshold)
            {
                bool merged = false;
                for (int i = inconsistencies.Count - 1; i >= Mathf.Max(inconsistencies.Count - maxBackMerge, 0); i--)
                {
                    var lastIncon = inconsistencies[i];
                    if (Vector3.Distance(lastIncon.Position, position) < mergeDistance)
                    {
                        // TODO: add time check
                        // Weighted average
                        lastIncon.Position = (lastIncon.Position * lastIncon.Count + position) / (lastIncon.Count + 1);
                        lastIncon.Count++;
                        lastIncon.Prefab.transform.position = lastIncon.Position + DroneView.DroneView.Offset;
                        merged = true;
                        Debug.Log($"Merged {lastIncon.Position}");
                        break;
                    }
                }

                if (!merged)
                {
                    var incon = new Inconsistency
                    {
                        Position = position,
                        Count = 1,
                        Prefab = Instantiate(prefab, sensors.DronePosition + DroneView.DroneView.Offset,
                            Quaternion.identity, transform)
                    };
                    inconsistencies.Add(incon);
                }
            }

            lastRight = right;
            lastLeft = left;
        }
    }
}
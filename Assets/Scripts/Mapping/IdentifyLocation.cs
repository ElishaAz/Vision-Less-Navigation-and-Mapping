using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mapping
{
    public class IdentifyLocation : MonoBehaviour
    {
        private readonly float timeBack = 10;
        private readonly float lidarAccuracy = 0.1f;
        private readonly float yawAccuracy = 1f;

        [SerializeField] private Vector3 offset = new Vector3(200, 0, 200);
        [SerializeField] private GameObject prefab;

        private List<GameObject> prefabs = new List<GameObject>();

        private float nextUpdate;

        private void Awake()
        {
        }

        private void FixedUpdate()
        {
            nextUpdate -= Time.fixedDeltaTime;

            if (nextUpdate > 0)
            {
                return;
            }

            nextUpdate = 0.5f;

            var current = DataCollector.Instance.CurrentPoint;

            foreach (var obj in prefabs)
            {
                obj.transform.position = Vector3.zero;
            }

            List<DataPoint> closePoints = new List<DataPoint>();

            foreach (var dp in DataCollector.Instance.History)
            {
                if (current.Time - dp.Time < timeBack) continue;

                if (!CompareLidars(current.Front, current.Back, dp.Front, dp.Back)
                    || !CompareLidars(current.Right, current.Left, dp.Right, dp.Left)) continue;

                if (!(dp.Rotation.eulerAngles.y - current.Rotation.eulerAngles.y < yawAccuracy)) continue;
                closePoints.Add(dp);
            }

            var points = GroupPoints(closePoints);
            ShowPoints(points);
        }

        private List<Vector3> GroupPoints(List<DataPoint> points)
        {
            List<List<DataPoint>> pointGroups = new List<List<DataPoint>>();
            float maxDistance = 0.5f;

            foreach (var dp in points)
            {
                if (pointGroups.Count == 0)
                {
                    var list = new List<DataPoint>();
                    list.Add(dp);
                    pointGroups.Add(list);
                    continue;
                }

                bool added = false;
                foreach (var group in pointGroups)
                {
                    bool close = true;
                    foreach (var point in group)
                    {
                        if (Vector3.Distance(dp.Position, point.Position) > maxDistance)
                        {
                            close = false;
                            break;
                        }
                    }

                    if (close)
                    {
                        group.Add(dp);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    var list = new List<DataPoint>();
                    list.Add(dp);
                    pointGroups.Add(list);
                }
            }

            List<DataPoint> biggest = null;
            foreach (var group in pointGroups)
            {
                if (biggest == null || group.Count > biggest.Count)
                {
                    biggest = group;
                }
            }

            pointGroups.Sort((g1, g2) => g2.Count.CompareTo(g1.Count));

            return pointGroups.Select(group => VecAverage(group.Select(dp => dp.Position).ToList())).ToList();
        }

        private static Vector3 VecAverage(List<Vector3> points)
        {
            var average = points.Aggregate(Vector3.zero, (current, point) => current + point);

            average /= points.Count;
            return average;
        }

        private void ShowPoints(List<Vector3> points)
        {
            if (points.Count > 0)
                Debug.Log(points.Select(p => p.ToString()).Aggregate("", (s, p) => s + ", " + p.ToString()));

            for (int i = 0; i < points.Count; i++)
            {
                if (i >= prefabs.Count)
                {
                    prefabs.Add(Instantiate(prefab, transform));
                    if (i == 0)
                    {
                        if (prefabs[0].TryGetComponent<MeshRenderer>(out var renderer))
                        {
                            renderer.material.color = Color.red;
                        }
                    }
                }

                var p = prefabs[i];
                p.transform.position = points[i] + offset;
            }
        }

        private bool CompareLidars(float current0, float current1, float history0, float history1)
        {
            if (float.IsInfinity(current0)) current0 = 10;
            if (float.IsInfinity(current1)) current1 = 10;
            if (float.IsInfinity(history0)) history0 = 10;
            if (float.IsInfinity(history1)) history1 = 10;

            return Math.Abs(current0 - history0) < lidarAccuracy && Math.Abs(current1 - history1) < lidarAccuracy;
        }
    }
}
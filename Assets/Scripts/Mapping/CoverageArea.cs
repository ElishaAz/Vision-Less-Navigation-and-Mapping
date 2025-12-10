using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mapping
{
    public class CoverageArea : MonoBehaviour
    {
        private static List<CoverageArea> areas = new List<CoverageArea>();
        public static IReadOnlyList<CoverageArea> Areas => areas;

        private bool[,,] collected;

        public int TotalPoints { get; private set; }

        public int TotalCollected { get; private set; } = 0;


        private void Awake()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            areas.Add(this);
        }

        private float scale;
        private float yScale;

        public void Create(float scale, float yScale)
        {
            this.scale = scale;
            this.yScale = yScale;

            collected = new bool[(int)(transform.lossyScale.x / scale),
                (int)(transform.lossyScale.y / yScale),
                (int)(transform.lossyScale.z / scale)];

            TotalPoints = collected.GetLength(0) * collected.GetLength(1) * collected.GetLength(2);
        }

        public void Collect(Vector3 point)
        {
            point -= transform.position;
            var x = (int)(point.x / scale + transform.lossyScale.x / scale / 2);
            var y = (int)(point.y / scale + transform.lossyScale.y / yScale / 2);
            var z = (int)(point.z / scale + transform.lossyScale.z / scale / 2);

            if (x < 0 || x >= collected.GetLength(0)
                      || y < 0 || y >= collected.GetLength(1)
                      || z < 0 || z >= collected.GetLength(2))
            {
                return;
            }

            if (collected[x, y, z]) return;
            collected[x, y, z] = true;
            TotalCollected++;
        }
    }
}
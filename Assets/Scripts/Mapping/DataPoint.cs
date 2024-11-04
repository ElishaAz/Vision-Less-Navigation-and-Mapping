using UnityEngine;

namespace Mapping
{
    public readonly struct DataPoint
    {
        public readonly float Time;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly float Front, Back, Up, Down, Right, Left;

        public DataPoint(float time, Vector3 position, Quaternion rotation, float front, float back, float up,
            float down, float right, float left)
        {
            this.Time = time;
            this.Position = position;
            this.Rotation = rotation;
            this.Front = front;
            this.Back = back;
            this.Up = up;
            this.Down = down;
            this.Right = right;
            this.Left = left;
        }

        public float RightLeft => Right + Left;
        public float UpDown => Up + Down;
        public float FrontBack => Front + Back;
    }
}
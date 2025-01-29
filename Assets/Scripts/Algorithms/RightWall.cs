using System;
using Drone;
using UnityEngine;

namespace Algorithms
{
    public class RightWall : MonoBehaviour
    {
        [SerializeField] private DroneSensors sensors;
        [SerializeField] private Drone.Drone drone;

        enum State
        {
            RightWall,
            TurnLeft
        }

        private State state;

        private SimplePID yawPID = new SimplePID(1, 0, 0, -1, 1);
        private SimplePID rollPID = new SimplePID(1, 0, 0, -0.1f, 0.1f);

        private void Awake()
        {
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {
            float roll = 0;
            float pitch = 0;
            float yaw = 0;

            float frontRight = sensors.frontRight.DistanceNormalized;
            float frontLeft = sensors.frontLeft.DistanceNormalized;
            float backRight = sensors.backRight.DistanceNormalized;

            float right = (frontRight + backRight) / 2;

            switch (state)
            {
                case State.RightWall:
                    yaw = yawPID.Get(frontRight, backRight, Time.fixedDeltaTime);
                    roll = -rollPID.Get(1, right, Time.fixedDeltaTime);

                    pitch = 0.2f;

                    if (frontRight < 1 && frontLeft < 1)
                    {
                        state = State.TurnLeft;
                    }

                    break;
                case State.TurnLeft:
                    yaw = -1;
                    pitch = 0;
                    if (frontRight > 1.2)
                    {
                        state = State.RightWall;
                        yawPID.Reset();
                    }

                    roll = -rollPID.Get(1, right, Time.fixedDeltaTime);

                    break;
            }

            HUD.AlgoLog = $"State: {state}. Pitch: {pitch}. Yaw: {yaw}. Roll: {roll}";

            drone.RC(roll, pitch, yaw, 0);
        }
    }
}
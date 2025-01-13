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
                    if (frontRight > backRight)
                    {
                        yaw = 1;
                    }
                    else
                    {
                        yaw = -1;
                    }

                    if (right > 1)
                    {
                        roll = 0.1f;
                    }
                    else
                    {
                        roll = -0.1f;
                    }

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
                    }

                    if (right > 1)
                    {
                        roll = 0.1f;
                    }
                    else
                    {
                        roll = -0.1f;
                    }

                    break;
            }

            HUD.AlgoLog = $"State: {state}. Pitch: {pitch}. Yaw: {yaw}. Roll: {roll}";

            drone.RC(roll, pitch, yaw, 0);
        }
    }
}
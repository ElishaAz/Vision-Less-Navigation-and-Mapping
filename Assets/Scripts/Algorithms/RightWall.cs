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
            TurnLeft,
            TurnRight
        }

        private State state;

        private SimplePID yawPID = new SimplePID(1, 0, 0, -1, 1);
        private SimplePID rollPID = new SimplePID(1, 0, 0, -0.1f, 0.1f);
        
        private SimplePID yawTurnPID = new SimplePID(1, 0, 0, -1f, 1f);

        private void Awake()
        {
        }

        private void Start()
        {
        }

        private float lastFrontRight;
        private Vector3 turnRightPivot;
        private Vector3 turnRightStart;
        private float turnRightDistance;

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

                    if (lastFrontRight < 1.2 && frontRight - lastFrontRight > 1)
                    {
                        state = State.TurnRight;
                        turnRightStart = sensors.DronePosition;
                        turnRightPivot = sensors.DronePosition + (sensors.DroneRotation *
                                                                     (sensors.frontRight.transform.localRotation *
                                                                      Vector3.forward * lastFrontRight +
                                                                      sensors.frontRight.transform.localPosition));
                        turnRightDistance = lastFrontRight;
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
                case State.TurnRight:
                    yaw = yawTurnPID.Get(1, Vector3.Distance(sensors.DronePosition, turnRightPivot), Time.fixedDeltaTime);
                    roll = 0;
                    pitch = 0.2f;

                    if (frontRight < 1.2 || Vector3.Distance(sensors.DronePosition, turnRightStart)> 1.8 * turnRightDistance)
                    {
                        state = State.RightWall;
                        yawPID.Reset();
                        rollPID.Reset();
                    }

                    break;
            }

            lastFrontRight = frontRight;

            HUD.AlgoLog = $"State: {state}.\nPitch: {pitch:F}. Yaw: {yaw:F}. Roll: {roll:F}";

            drone.RC(roll, pitch, yaw, 0);
        }
    }
}
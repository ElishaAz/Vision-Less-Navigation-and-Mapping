using System;
using Drone;
using UnityEngine;

namespace Algorithms
{
    public class RightWallOld : MonoBehaviour
    {
        [SerializeField] private Drone.Drone drone;
        [SerializeField] private DroneSensors sensors;

        [SerializeField] private float frontClose = 1f;
        [SerializeField] private float frontEmergency = 0.3f;
        [SerializeField] private float rightFar = 2.5f;
        [SerializeField] private float rightLeftClose = 0.25f;

        private SimplePID pitchPid = new SimplePID(0.5f, 0, 0, 0.1f, 1);
        private SimplePID tunnelPid = new SimplePID(1, 0, 0, -1, 1);
        private SimplePID ffPid = new SimplePID(1, 0, 0, -0.2f, 0.2f);

        private bool isTurnRight;
        private float targetYaw;
        private float moveForwardEnd;
        private bool firstMoveForward;

        private float lastStateSet;
        private State state;

        enum State
        {
            Emergency,
            RotateCCW,
            Tunnel,
            MoveForward,
            TurnCW,
            FlyForward,
        }

        private void SetState()
        {

            float front = sensors.front.DistanceNormalized;
            float right = sensors.right.DistanceNormalized;
            float left = sensors.left.DistanceNormalized;

            if (front < frontEmergency)
            {
                // Emergency
                state = State.Emergency;
                return;
            }
            
            
            if (Time.time - lastStateSet < 0.3f) return; // Only set the state once a second (other than emergency)
            
            if (front < frontClose)
            {
                // Turn C.C.W
                state = State.RotateCCW;
            }
            else if (left + right < rightLeftClose)
            {
                // Tunnel
                state = State.Tunnel;
            }
            else if (right > rightFar)
            {
                // Turn C.W.
                state = State.MoveForward;
                moveForwardEnd = Time.time + 0.1f;
                firstMoveForward = true;
            }
            else
            {
                // Fly Forward
                state = State.FlyForward;
            }

            lastStateSet = Time.time;
        }

        private void FixedUpdate()
        {
            bool wasTurnRight = false;

            float front = sensors.front.DistanceNormalized;
            float right = sensors.right.DistanceNormalized;
            float left = sensors.left.DistanceNormalized;

            float roll = 0;
            float pitch = 0;
            float yaw = 0;
            float thrust = 0;

            switch (state)
            {
                case State.Emergency:
                    pitch = -0.2f;
                    SetState();
                    break;
                case State.RotateCCW:
                    yaw = -1f;
                    pitch = 0.1f;
                    SetState();
                    break;
                case State.Tunnel:
                    roll = tunnelPid.Get(0, left - right, Time.fixedDeltaTime);
                    pitch = pitchPid.Get(-frontClose, -front, Time.fixedDeltaTime);
                    SetState();
                    break;
                case State.MoveForward:
                    pitch = 0.2f;
                    if (Time.time >= moveForwardEnd)
                    {
                        if (firstMoveForward)
                        {
                            state = State.TurnCW;
                            targetYaw = sensors.gyro.Yaw + 90;
                            if (targetYaw > 360)
                            {
                                targetYaw -= 360;
                            }

                            if (targetYaw < 0)
                            {
                                targetYaw += 360;
                            }

                            firstMoveForward = false;
                        }
                        else
                        {
                            SetState();
                        }
                    }

                    break;
                case State.TurnCW:
                    yaw = 1;

                    float currentYaw = sensors.gyro.Yaw;
                    if (currentYaw > 360)
                    {
                        currentYaw -= 360;
                    }

                    if (currentYaw < 0)
                    {
                        currentYaw += 360;
                    }
                    
                    float diff = targetYaw - currentYaw;

                    if (diff > 360)
                    {
                        diff -= 360;
                    }

                    if (diff < 0)
                    {
                        diff += 360;
                    }

                    if (diff > 350)
                    {
                        state = State.MoveForward;
                        moveForwardEnd = Time.time + 1.5f;
                    }

                    break;
                case State.FlyForward:
                    yaw = -ffPid.Get(0.5f, right, Time.fixedDeltaTime);
                    pitch = pitchPid.Get(-frontClose, -front, Time.fixedDeltaTime);
                    SetState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            HUD.AlgoLog =
                $"State: {state}. ({targetYaw})\nPitch: {pitch:F}. Yaw: {yaw:F}. Roll: {roll:F}. Thurst: {thrust:F}";

            drone.RC(roll, pitch, yaw, thrust);
        }
    }
}
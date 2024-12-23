using System;
using System.Collections;
using Drone;
using UnityEngine;

namespace Algorithms
{
    public class RightWallV2 : MonoBehaviour
    {
        [SerializeField] private DroneSensors sensors;
        [SerializeField] private Drone.Drone drone;

        private class SimplePID
        {
            private float Kp, Ki, Kd;
            private float min, max;
            private float previousError;
            private float integral;

            public SimplePID(float kp, float ki, float kd, float min, float max)
            {
                Kp = kp;
                Ki = ki;
                Kd = kd;
                this.min = min;
                this.max = max;
            }

            public float Get(float target, float current, float dt)
            {
                var error = target - current;
                var p = error;
                integral += error * dt;
                var d = (error - previousError) / dt;
                var output = Kp * p + Ki * integral + Kd * d;
                previousError = error;
                return Mathf.Clamp(output, min, max);
            }

            public void Reset()
            {
                integral = 0;
                previousError = 0;
            }
        }

        // private readonly PID pitchPID = new PID(1f, 0, 0, 0, 0.5f, -0.1f);
        // private readonly PID yawPID = new PID(1f, 0, 0, 0, 1f, -1f);

        // private readonly SimplePID pitchPID = new SimplePID(1f, 0, 0, -0.1f, 0.5f);
        // private readonly SimplePID yawRightPID = new SimplePID(1f, 0, 0, -0.3f, 0.3f);
        // private readonly SimplePID yawFrontPID = new SimplePID(10f, 0, 0, 0, 0.5f);
        // private readonly SimplePID rollFrontPID = new SimplePID(1f, 0, 0, 0f, 0.1f);

        private readonly SimplePID pitchPID = new SimplePID(-1f, 0, 0, 0.2f, 1f);
        private readonly SimplePID pitchTunnelPID = new SimplePID(-1f, 0, 0, 0.1f, 1f);
        private readonly SimplePID rollRightPID = new SimplePID(-1f, 0, -1f, -1f, 1f);
        private readonly SimplePID rollTunnelPID = new SimplePID(-1f, 0, 0, -1f, 1f);
        private readonly SimplePID yawPID = new SimplePID(-1f, 0, 0, -1f, 0);


        private readonly SimplePID pitchWallYaw = new SimplePID(-1f, 0, 0, -1f, 1f);
        private readonly SimplePID yawWallYaw = new SimplePID(-1f, 0, -1f, -1f, 1f);
        private readonly SimplePID yawTurnRightPID = new SimplePID(-1f, 0, 0, 0f, 1f);
        private readonly SimplePID rollTurnRightPID = new SimplePID(-1f, 0, 0, 0f, 0.2f);

        private readonly State defaultState = State.RightWall;


        private enum State
        {
            RightWall,
            RightWallYaw,
            RightWallYawTurnLeft,
            RightWallYawFixedPitch,
            Tunnel,
            LeftWall,
            ForwardFast,
            EmergencyStop,
            TurnLeft,
            TurnRight,
            TurnRightPart2,
            NoSensorsForward,
        }

        private State state;
        private float startTurnRightTime;
        private float lastStateChangeTime;
        private float lastYawCloseTime;
        private float turnRightDistanceBeforeTurn;

        private void Awake()
        {
            state = defaultState;
        }

        private void SetState(State newState)
        {
            pitchPID.Reset();
            pitchTunnelPID.Reset();
            rollRightPID.Reset();
            yawPID.Reset();
            yawTurnRightPID.Reset();
            rollTurnRightPID.Reset();
            pitchWallYaw.Reset();
            yawWallYaw.Reset();

            switch (newState)
            {
                case State.RightWall:
                    break;
                case State.RightWallYawFixedPitch:
                    break;
                case State.Tunnel:
                    break;
                case State.LeftWall:
                    break;
                case State.ForwardFast:
                    break;
                case State.EmergencyStop:
                    break;
                case State.TurnLeft:
                    break;
                case State.TurnRight:
                    startTurnRightTime = Time.time;
                    break;
                case State.TurnRightPart2:
                    break;
            }

            state = newState;
            lastStateChangeTime = Time.time;
        }

        private void FixedUpdate()
        {
            float roll = 0;
            float pitch = 0;
            float yaw = 0;
            float throttle = 0;

            float front = sensors.front.DistanceNormalized;
            float right = sensors.right.DistanceNormalized;
            float left = sensors.left.DistanceNormalized;

            // yaw = yawPID.PID_iterate(1, right, Time.fixedDeltaTime);
            // pitch = pitchPID.PID_iterate(1, front, Time.fixedDeltaTime);
            // yaw = Mathf.Clamp(right - 1 + Mathf.Min(front - 0.9f, 0) / 5, -0.9f, 0.9f);
            // pitch = (Mathf.Clamp(front - 1, 0.05f, 0.3f)) * (1 - Mathf.Abs(yaw)/2);

            // float yawFront = -yawFrontPID.Get(1.4f, front, Time.fixedDeltaTime);
            // float rollFront = -rollFrontPID.Get(1.4f, front, Time.fixedDeltaTime);
            // float yawRight = -yawRightPID.Get(1f, right, Time.fixedDeltaTime);
            // float pitchFront = -pitchPID.Get(1.3f, front, Time.fixedDeltaTime) * (1 - Mathf.Abs(yaw) / 2) + 0.05f;
            //
            // Debug.Log($"YawRight: {yawRight}. YawFront: {yawFront}. Pitch: {pitch}");
            //
            // if (right < 3)
            // {
            //     pitch += pitchFront;
            // }
            //
            // yaw += yawRight;
            // yaw += yawFront;
            //
            // if (right < 1)
            // {
            //     roll += rollFront;
            // }


            // if (front > 1)
            // {
            //     pitch = 0.5f;
            // }
            //
            // if (front < 1 || right < 1)
            // {
            //     yaw = -0.5f;
            //     pitch /= 5;
            // }

            // if (front > 2 && right > 2)
            // {
            //     pitch = 0.5f;
            //     yaw = 0f;
            // }

            // if (front > 5 && right > 5 && left > 5)
            // {
            //     pitch = 1f;
            //     drone.RC(roll, pitch, yaw, throttle);
            //     return;
            // }

            HUD.AlgoLog = $"State: {state}";
            Debug.Log($"State: {state}");

            switch (state)
            {
                case State.RightWall:
                    pitch = pitchPID.Get(2, front, Time.fixedDeltaTime);
                    roll = rollRightPID.Get(1.2f, right, Time.fixedDeltaTime);
                    yaw = yawPID.Get(5, front, Time.fixedDeltaTime);

                    if (right > 3.5f)
                    {
                        SetState(State.TurnRight);
                    }
                    else
                    {
                        turnRightDistanceBeforeTurn = right;

                        if (right + left < 3)
                        {
                            SetState(State.Tunnel);
                        }
                        else if (front < 1)
                        {
                            SetState(State.RightWallYawTurnLeft);
                        }
                    }

                    break;
                case State.Tunnel:
                    pitch = pitchTunnelPID.Get(0.9f, front, Time.fixedDeltaTime);
                    roll = rollTunnelPID.Get(0, right - left, Time.fixedDeltaTime);

                    if (pitch < 0.2f)
                    {
                        yaw = roll / 2;
                    }
                    // yaw = yawPID.Get(5, front, Time.fixedDeltaTime);

                    if (right > 3.5f)
                    {
                        SetState(State.TurnRight);
                    }
                    else
                    {
                        turnRightDistanceBeforeTurn = right;
                    }

                    if (right + left > 4)
                    {
                        if (right > 2 * left)
                        {
                            yaw = 1f;
                        }
                        else if (left > 2 * right)
                        {
                            yaw = -1f;
                        }
                        else
                        {
                            SetState(defaultState);
                        }
                    }
                    // else if (front < 1)
                    // {
                    //     SetState(State.RightWallYawTurnLeft);
                    // }

                    break;
                case State.ForwardFast:
                    break;
                case State.EmergencyStop:
                    break;
                case State.RightWallYaw:
                    pitch = pitchWallYaw.Get(1f, front, Time.fixedDeltaTime);
                    if (front < 1.5f)
                    {
                        SetState(State.RightWallYawTurnLeft);
                    }
                    else
                    {
                        yaw = yawWallYaw.Get(1f, right, Time.fixedDeltaTime);
                    }

                    if (right > 3)
                    {
                        SetState(State.TurnRight);
                    }
                    else if (right + left < 3)
                    {
                        SetState(State.Tunnel);
                    }
                    else if (front < 1)
                    {
                        SetState(State.TurnLeft);
                    }

                    // roll = rollTurnRightPID.Get(0.9f, right, Time.fixedDeltaTime);
                    break;
                case State.RightWallYawTurnLeft:
                    yaw = -1;
                    pitch = 0;
                    if (front < 1.5f)
                    {
                        lastYawCloseTime = Time.time;
                    }
                    else if (Time.time - lastYawCloseTime > 0.3f)
                    {
                        SetState(defaultState);
                    }

                    break;
                case State.RightWallYawFixedPitch:
                    pitch = 0.2f;
                    if (right > 1.5f)
                    {
                        roll = 1f;
                    }
                    else
                    {
                        roll = rollTurnRightPID.Get(0, right - left, Time.fixedDeltaTime);
                    }

                    yaw = yawTurnRightPID.Get(0.8f, right, Time.fixedDeltaTime);
                    if (front > 5)
                    {
                        SetState(defaultState);
                    }

                    if (left > 2 && front < 0.5f || front < 0.3f)
                    {
                        SetState(State.TurnLeft);
                    }

                    // if (front < 0.5f && right < 1f && left < 1f)
                    // {
                    //     SetState(defaultState);
                    // }

                    break;
                case State.TurnLeft:
                    pitch = 0;
                    roll = rollRightPID.Get(1, right, Time.fixedDeltaTime);
                    yaw = -1f;
                    if (front > 1.5)
                    {
                        SetState(defaultState);
                    }

                    if (front < 0.2f)
                    {
                        pitch = -0.1f;
                    }

                    break;
                case State.TurnRight:
                    pitch = 0f;
                    roll = 0.2f;
                    yaw = 1f;
                    if (right < Mathf.Max(turnRightDistanceBeforeTurn - 0.05f, 1f))
                    {
                        SetState(State.RightWallYawFixedPitch);
                    }

                    if (Time.time - lastStateChangeTime > 3f)
                    {
                        SetState(State.NoSensorsForward);
                    }

                    break;
                case State.NoSensorsForward:
                    pitch = 1;
                    if (front < 3f || right < 3f || left < 3f)
                    {
                        SetState(defaultState);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            drone.RC(roll, pitch, yaw, throttle);
        }
    }
}
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
        }

        // private readonly PID pitchPID = new PID(1f, 0, 0, 0, 0.5f, -0.1f);
        // private readonly PID yawPID = new PID(1f, 0, 0, 0, 1f, -1f);

        // private readonly SimplePID pitchPID = new SimplePID(1f, 0, 0, -0.1f, 0.5f);
        // private readonly SimplePID yawRightPID = new SimplePID(1f, 0, 0, -0.3f, 0.3f);
        // private readonly SimplePID yawFrontPID = new SimplePID(10f, 0, 0, 0, 0.5f);
        // private readonly SimplePID rollFrontPID = new SimplePID(1f, 0, 0, 0f, 0.1f);

        private readonly SimplePID pitchPID = new SimplePID(-1f, 0, 0, 0.2f, 1f);
        private readonly SimplePID rollRightPID = new SimplePID(-1f, 0, 0, -1f, 1f);
        private readonly SimplePID rollTunnelPID = new SimplePID(-1f, 0, 0, -1f, 1f);
        private readonly SimplePID yawPID = new SimplePID(-1f, 0, 0, -1f, 0);


        private readonly SimplePID pitchTurnRightPID = new SimplePID(-1f, 0, 0, 0f, 0.1f);
        private readonly SimplePID yawTurnRightPID = new SimplePID(-1f, 0, 0, 0f, 1f);
        private readonly SimplePID rollTurnRightPID = new SimplePID(-1f, 0, 0, 0f, 0.2f);


        private enum State
        {
            RightWall,
            RightWallYaw,
            Tunnel,
            LeftWall,
            ForwardFast,
            EmergencyStop,
            TurnLeft,
            TurnRight,
            TurnRightPart2,
        }

        private State state;
        private float startTurnRightTime;

        private void Awake()
        {
            state = State.RightWall;
        }

        private void SetState(State newState)
        {
            switch (newState)
            {
                case State.RightWall:
                    break;
                case State.RightWallYaw:
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

            if (front > 5 && right > 5 && left > 5)
            {
                pitch = 1f;
                drone.RC(roll, pitch, yaw, throttle);
                return;
            }

            Debug.Log($"State: {state}. Right: {right}, Left: {left}, Front: {front}");

            switch (state)
            {
                case State.RightWall:
                    pitch = pitchPID.Get(2, front, Time.fixedDeltaTime);
                    roll = rollRightPID.Get(1, right, Time.fixedDeltaTime);
                    yaw = yawPID.Get(5, front, Time.fixedDeltaTime);
                    if (right > 3)
                    {
                        SetState(State.TurnRight);
                    } else if (right + left < 3)
                    {
                        SetState(State.Tunnel);
                    } else if (front < 1)
                    {
                        SetState(State.TurnLeft);
                    }

                    break;
                case State.Tunnel:
                    pitch = 1f;
                    roll = rollTunnelPID.Get(0, right - left, Time.fixedDeltaTime);
                    yaw = yawPID.Get(5, front, Time.fixedDeltaTime);

                    if (right > 3)
                    {
                        SetState(State.TurnRight);
                    } else if (right + left > 3)
                    {
                        SetState(State.RightWall);
                    } else if (front < 1)
                    {
                        SetState(State.TurnLeft);
                    }

                    break;
                case State.ForwardFast:
                    break;
                case State.EmergencyStop:
                    break;
                case State.RightWallYaw:
                    pitch = 0.2f;
                    yaw = yawTurnRightPID.Get(0.8f, right, Time.fixedDeltaTime);
                    roll = rollTurnRightPID.Get(0.9f, right, Time.fixedDeltaTime);
                    if (front > 5)
                    {
                        SetState(State.RightWall);
                    }

                    if (left > 2 && front < 0.5f)
                    {
                        SetState(State.TurnLeft);
                    }
                    break;
                case State.TurnLeft:
                    pitch = 0;
                    roll = rollRightPID.Get(1, right, Time.fixedDeltaTime);
                    yaw = -1f;
                    if (front > 1.5)
                    {
                        SetState(State.RightWall);
                    }

                    break;
                case State.TurnRight:
                    pitch = 0f;
                    roll = 0.2f;
                    yaw = 1f;
                    if (right < 1.2f)
                    {
                        SetState(State.RightWallYaw);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            drone.RC(roll, pitch, yaw, throttle);
        }
    }
}
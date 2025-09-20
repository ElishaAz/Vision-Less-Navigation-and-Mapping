using System;
using Drone;
using UnityEngine;

namespace Algorithms
{
    public class RightWall : MonoBehaviour
    {
        [SerializeField] private DroneSensors sensors;
        [SerializeField] private Drone.Drone drone;

        [SerializeField] private float emergencyThresh = 0.2f;
        [SerializeField] private float turnRightThresh = 1.2f;

        enum State
        {
            RightWall,
            TurnLeft,
            TurnRight,
            Emergency,
        }

        private State state = State.RightWall;

        private SimplePID yawPID = new SimplePID(1, 0, 0, -1, 1);
        private SimplePID rollPID = new SimplePID(1, 0, 0, -0.1f, 0.1f);
        private SimplePID pitchPID = new SimplePID(1, 0, 0.1f, 0.1f, 1f);

        private SimplePID rollTurnPID = new SimplePID(1, 0, 0, -0.1f, 0.1f);

        private SimplePID thrustPID = new SimplePID(1, 0, 0, -1f, 1f);

        private void Awake()
        {
        }

        private void Start()
        {
            firstRun = true;
            time = 0;
        }

        private bool firstRun = true;
        private float lastFrontRight;
        private Vector3 turnRightPivot;
        private Vector3 turnRightStart;
        private float turnRightDistance;

        private float turnRightDecFrames = 0;
        private float lastStateChange;
        private float time;

        private void FixedUpdate()
        {
            float roll = 0;
            float pitch = 0;
            float yaw = 0;
            float thrust = 0;

            time += Time.fixedDeltaTime;

            float frontRight = sensors.frontRight.DistanceNormalized;
            float frontLeft = sensors.frontLeft.DistanceNormalized;
            float backRight = sensors.backRight.DistanceNormalized;
            float top = sensors.up.DistanceNormalized;
            float bottom = sensors.down.DistanceNormalized;

            if (firstRun)
            {
                firstRun = false;
                lastFrontRight = frontRight;
                lastStateChange = 0;
            }

            float right = (frontRight + backRight) / 2;

            float throttlePIDCurrent;

            if (float.IsInfinity(sensors.up.Distance) && float.IsInfinity(sensors.down.Distance))
            {
                throttlePIDCurrent = 0;
            }
            else if (float.IsInfinity(sensors.up.Distance))
            {
                throttlePIDCurrent = sensors.up.MaxDistance;
            }
            else if (float.IsInfinity(sensors.down.Distance))
            {
                throttlePIDCurrent = sensors.down.MaxDistance;
            }
            else
            {
                throttlePIDCurrent = top - bottom;
            }

            thrust = thrustPID.Get(0, throttlePIDCurrent, Time.fixedDeltaTime);

            switch (state)
            {
                case State.RightWall:
                    yaw = yawPID.Get(frontRight, backRight, Time.fixedDeltaTime);
                    roll = -rollPID.Get(1, right, Time.fixedDeltaTime);

                    pitch = pitchPID.Get(-0.5f, -Mathf.Min(frontRight, frontLeft), Time.fixedDeltaTime);

                    if (time - lastStateChange < 1)
                    {
                        pitch = Mathf.Min(pitch, 0.2f); // Do not fly at more than 0.2 pitch for the first second
                    }

                    if (frontRight < 1 && frontLeft < 1)
                    {
                        state = State.TurnLeft;
                        lastStateChange = time;
                    }

                    Debug.Log($"{lastFrontRight}, {frontRight}");

                    if (lastFrontRight < turnRightThresh && frontRight - lastFrontRight > 1 && lastFrontRight < 3)
                    {
                        state = State.TurnRight;
                        rollTurnPID.Reset();
                        turnRightStart = sensors.DronePosition;
                        turnRightPivot = sensors.DronePosition + (sensors.DroneRotation *
                                                                  (sensors.frontRight.transform.localRotation *
                                                                   Vector3.forward * lastFrontRight +
                                                                   sensors.frontRight.transform.localPosition));
                        turnRightDistance = lastFrontRight;
                        turnRightDecFrames = 0;
                        lastStateChange = time;
                    }

                    break;
                case State.TurnLeft:
                    yaw = -1;
                    pitch = 0;
                    if (frontRight > 1.2)
                    {
                        state = State.RightWall;
                        yawPID.Reset();
                        pitchPID.Reset();
                        lastStateChange = time;
                    }

                    roll = -rollPID.Get(1, right, Time.fixedDeltaTime);

                    break;
                case State.TurnRight:
                    yaw = 1f;
                    roll = -rollTurnPID.Get(1f, Vector3.Distance(sensors.DronePosition, turnRightPivot),
                        Time.fixedDeltaTime);
                    pitch = 0.2f;

                    if (lastFrontRight > frontRight && frontRight < turnRightDistance * 0.8f)
                    {
                        turnRightDecFrames++;
                    }
                    else
                    {
                        turnRightDecFrames--;
                        if (turnRightDecFrames < 0) turnRightDecFrames = 0;
                    }

                    // If the front-right is going down steadily (or is too close anyways)
                    if (turnRightDecFrames > 15 || frontRight < 0.5f)
                    {
                        state = State.RightWall;
                        yawPID.Reset();
                        rollPID.Reset();
                        pitchPID.Reset();
                        lastStateChange = time;
                    }

                    break;
            }

            // Add buoyancy (should be with a PID too). This helps it not get stuck in walls.

            if (frontRight < emergencyThresh)
            {
                if (frontLeft < backRight)
                {
                    pitch = -0.1f;
                }
                else
                {
                    roll = -0.1f;
                }
            }

            if (frontLeft < emergencyThresh)
            {
                pitch = -0.1f;
            }

            if (frontRight < emergencyThresh)
            {
                roll = -0.1f;
            }

            lastFrontRight = frontRight;

            HUD.AlgoLog = $"State: {state}.\nPitch: {pitch:F}. Yaw: {yaw:F}. Roll: {roll:F}. Thurst: {thrust:F}";

            drone.RC(roll, pitch, yaw, thrust);
        }
    }
}
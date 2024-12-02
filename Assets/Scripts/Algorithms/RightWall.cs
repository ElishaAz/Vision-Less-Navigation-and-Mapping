using System;
using Drone;
using UnityEngine;

namespace Algorithms
{
	public class RightWall : MonoBehaviour
	{
		[SerializeField] private DroneSensors sensors;
		[SerializeField] private Drone.Drone drone;

		[SerializeField] private float frontEmergency = 1f;
		[SerializeField] private float targetFront = 2f;
		[SerializeField] private float frontVeryFar = 5f;

		[SerializeField] private float tunnel = 3f;
		[SerializeField] private float rightFar = 3f;
		[SerializeField] private float rightOk = 2f;
		[SerializeField] private float rightClose = 1f;
		[SerializeField] private float frontClose = 2f;

		private float roll, pitch, yaw, thrust;

		private float front = float.PositiveInfinity, right = float.PositiveInfinity, left = float.PositiveInfinity;

		private PID tunnelPID = new PID(0.5f, 0, 0, 0, 1, -1);
		private PID rightPID = new PID(0.1f, 0, 0, 0, 0.1f, -0.1f);

		private enum LidarMode
		{
			Emergency,
			Close,
			Standard,
			Far,
			VeryFar
		}

		private LidarMode frontMode, rightMode;

		private enum PitchYawMode
		{
			Emergency,
			RightFar,
			RightFarAndFrontClose,
			FrontClose,
			FrontWasClose,
			FrontVeryFar,
			FrontFar,
			Standard
		}

		private PitchYawMode pitchYawMode = PitchYawMode.Standard;

		private enum RollMode
		{
			Tunnel,
			RightEmergency,
			LeftEmergency,
			Standard
		}
		
		private RollMode rollMode = RollMode.Standard;

		private float InferInfinitySign(float lastValue, float currentValue)
		{
			if (float.IsPositiveInfinity(currentValue))
			{
				if (front < 2)
				{
					return float.NegativeInfinity;
				}
				else
				{
					return float.PositiveInfinity;
				}
			}

			return currentValue;
		}

		private float LimitLidar(float value, float max)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				return max;
			}

			return value;
		}

		private void HandlePitchYaw()
		{
			if (front <= frontEmergency)
			{
				pitchYawMode = PitchYawMode.Emergency;
			}
			else
			{
				if (right > rightFar)
				{
					if (front < frontClose)
					{
						pitchYawMode = PitchYawMode.RightFarAndFrontClose;
					}
					else
					{
						pitchYawMode = PitchYawMode.RightFar;
					}
				} else if (front < frontClose)
				{
					pitchYawMode = PitchYawMode.FrontClose;
				}
				else
				{
					if (front > frontVeryFar)
					{
						pitchYawMode = PitchYawMode.FrontVeryFar;
					}
					else if (front > targetFront)
					{
						pitchYawMode = PitchYawMode.FrontFar;
					}
					else
					{
						pitchYawMode = PitchYawMode.Standard;
					}
				}
			}

			switch (pitchYawMode)
			{
				case PitchYawMode.Emergency:
					pitch = -0.1f;
					yaw = -0.1f;
					break;
				case PitchYawMode.RightFar:
					pitch = 0.1f;
					yaw = 1;
					break;
				case PitchYawMode.RightFarAndFrontClose:
					pitch = 0.05f;
					yaw = 1;
					break;
				case PitchYawMode.FrontClose:
					pitch = 0;
					yaw = -0.5f;
					break;
				case PitchYawMode.FrontVeryFar:
					yaw = 0;
					pitch = 1;
					break;
				case PitchYawMode.FrontFar:
					yaw = 0;
					pitch = 0.5f;
					break;
				case PitchYawMode.Standard:
					yaw = 0;
					pitch = 0.1f;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void HandleRoll()
		{
			if (right < rightClose)
			{
				rollMode = RollMode.RightEmergency;
			} else if (left < rightClose)
			{
				rollMode = RollMode.LeftEmergency;
			}
			else if (right + left < tunnel)
			{
				rollMode = RollMode.Tunnel;
			}
			else
			{
				rollMode = RollMode.Standard;
			}

			switch (rollMode)
			{
				case RollMode.Tunnel:
					rightPID.ResetController();
					roll = tunnelPID.PID_iterate(0, left - right, Time.fixedDeltaTime);
					break;
				case RollMode.Standard:
					tunnelPID.ResetController();
					roll = -rightPID.PID_iterate(rightOk, right, Time.fixedDeltaTime);
					break;
				case RollMode.RightEmergency:
					roll = -0.2f;
					break;
				case RollMode.LeftEmergency:
					roll = 0.2f;
					break;
			}
		}

		private void FixedUpdate()
		{
			// front = InferInfinitySign(front, sensors.front.Distance);
			// right = InferInfinitySign(right, sensors.right.Distance);
			// left = InferInfinitySign(left, sensors.left.Distance);
			front = LimitLidar(sensors.front.Distance, sensors.front.MaxDistance);
			right = LimitLidar(sensors.right.Distance, sensors.right.MaxDistance);
			left = LimitLidar(sensors.left.Distance, sensors.left.MaxDistance);

			// if (front < frontEmergency)
			// {
			// 	frontMode = LidarMode.Emergency;
			// }
			// else if (front < frontClose)
			// {
			// 	frontMode = LidarMode.Close;
			// } else if (front > targetFront * 2)
			// {
			// 	frontMode = LidarMode.VeryFar;
			// } else if (front > targetFront)
			// {
			// 	frontMode = LidarMode.Far;
			// }
			// else
			// {
			// 	frontMode = LidarMode.Standard;
			// }
			//
			// if (right < rightOk / 2)
			// {
			// 	rightMode = LidarMode.Close;
			// } else if (right > rightFar)
			// {
			// 	rightMode = LidarMode.Far;
			// }
			// else
			// {
			// 	rightMode = LidarMode.Standard;
			// }
			//
			// if (frontMode == LidarMode.Emergency)
			// {
			// 	pitch = -0.1f;
			// 	yaw = -0.5f;
			// }
			// else
			// {
			// 	
			// }

			HandlePitchYaw();

			HandleRoll();
			
			Debug.Log($"Modes: {pitchYawMode}, {rollMode}, front: {front}, right: {right}, left: {left}");

			drone.RC(roll, pitch, yaw, thrust);
		}
	}
}
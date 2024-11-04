using UnityEngine;

public static class Utils
{
	public static float NormalizeAngle(float angle)
	{
		while (angle > 180)
		{
			angle -= 360;
		}

		while (angle < -180)
		{
			angle += 360;
		}

		return angle;
	}

	public static Vector3 NormalizeAngles(Vector3 eulerAngles)
	{
		return new Vector3(NormalizeAngle(eulerAngles.x), NormalizeAngle(eulerAngles.y), NormalizeAngle(eulerAngles.z));
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using Drone;
using UnityEngine;

public class HUD : MonoBehaviour
{
	[SerializeField] private DroneSensors sensors;

	public static string AlgoLog;

	private void OnGUI()
	{
		GUI.color = Color.black;
		GUILayout.Label(
			$"Gyro: roll={sensors.gyro.Roll,8:0.00}, pitch={sensors.gyro.Pitch,8:0.00}, yaw={sensors.gyro.Yaw,8:0.00}");
		GUILayout.Label($"Optical Flow: {sensors.opticalFlow.Position,8:0.00}");
		GUILayout.Label($"Compass: {sensors.compass.Value,8:0.00}");
		GUILayout.Label($"Barometer: {sensors.barometer.Value,8:0.00}");
		GUILayout.Space(10);
		GUILayout.Label("Lidars:");
		GUILayout.BeginVertical();
		GUILayout.Label($"Front: {sensors.front.Distance,8:0.00}");
		GUILayout.Label($"Back: {sensors.back.Distance,8:0.00}");
		GUILayout.Label($"Right: {sensors.right.Distance,8:0.00}");
		GUILayout.Label($"Left: {sensors.left.Distance,8:0.00}");
		GUILayout.Label($"Up: {sensors.up.Distance,8:0.00}");
		GUILayout.Label($"Down: {sensors.down.Distance,8:0.00}");
		GUILayout.Label($"Crash count: {sensors.crashDetector.Crashes}");
		GUILayout.Label(AlgoLog);
		if (sensors.crashDetector.InCrash)
		{
			GUI.color = Color.red;
			GUILayout.Label("Crashed!");
		}
		GUILayout.EndVertical();
	}
}
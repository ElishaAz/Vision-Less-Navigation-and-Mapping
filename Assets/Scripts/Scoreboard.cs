using System;
using System.Collections.Generic;
using Algorithms;
using Drone;
using Mapping;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private float[] intervals;
    [SerializeField] private float timeScale = 1;
    [SerializeField] private GameObject[] maps;

    private static int currentInterval = 0;
    private static bool currentNew = true;
    private static int currentMap = 0;

    private static int iterations = 0;

    private readonly struct Score
    {
        public readonly string map;
        public readonly float interval;
        public readonly bool isNew;
        public readonly int collected;
        public readonly int crashes;

        public readonly float recall;
        public readonly float precision;
        public readonly float f1;

        public Score(string map, float interval, bool isNew, int collected, int crashes, float recall, float precision)
        {
            this.map = map;
            this.interval = interval;
            this.isNew = isNew;
            this.collected = collected;
            this.crashes = crashes;
            this.recall = recall;
            this.precision = precision;

            f1 = 2 * (recall * precision) / (recall + precision);
        }
    }

    private static List<Score> scores = new List<Score>();

    private bool finished = false;

    private RightWall rightWall;
    private RightWallOld rightWallOld;
    private Coverage coverage;
    private DroneSensors sensors;

    private void Awake()
    {
#if UNITY_EDITOR
        if (timeScale > 100) timeScale = 100;
#endif

        FindAnyObjectByType<TimeScale>()?.gameObject.SetActive(false);

        var drone = FindAnyObjectByType<Drone.Drone>();
        sensors = FindAnyObjectByType<DroneSensors>();
        coverage = FindAnyObjectByType<Coverage>();

        rightWall = drone.GetComponent<RightWall>();
        rightWallOld = drone.GetComponent<RightWallOld>();

        if (currentNew)
        {
            rightWall.enabled = true;
            rightWallOld.enabled = false;
        }
        else
        {
            rightWall.enabled = false;
            rightWallOld.enabled = true;
        }

        if (currentInterval >= intervals.Length)
        {
            currentMap++;
            currentInterval = 0;
        }

        if (currentMap >= maps.Length)
        {
            finished = true;
            Time.timeScale = 0;
        }

        coverage.SetUseOldLidars(!currentNew);

        FindAnyObjectByType<DroneView.DroneView>()?.SetUseOldLidars(!currentNew);

        for (var i = 0; i < maps.Length; i++)
        {
            if (i == currentMap) continue;
            maps[i].SetActive(false);
        }


        if (currentMap < maps.Length)
        {
            maps[currentMap].SetActive(true);
        }
    }

    private void Update()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 15;

        Time.timeScale = finished ? 0 : timeScale;
    }

    private Score CurrentScore => new Score(
        currentMap >= maps.Length ? "null" : maps[currentMap].name,
        intervals[currentInterval],
        currentNew,
        coverage.TotalCollected,
        sensors.crashDetector.Crashes,
        Mapping.Algorithms.EdgeSimilarity.Recall,
        Mapping.Algorithms.EdgeSimilarity.Precision
    );

    private void FinishedInterval()
    {
        scores.Add(CurrentScore);

        if (currentNew)
        {
            currentNew = false;
        }
        else
        {
            currentNew = true;
            currentInterval++;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        finished = true;
        iterations++;
    }

    private void FixedUpdate()
    {
        if (finished) return;

        if (Time.timeSinceLevelLoad >= intervals[currentInterval])
        {
            FinishedInterval();
        }
    }

    private static string IntervalToString(float interval)
    {
        int seconds = Mathf.FloorToInt(interval);

        int minutes = seconds % 60;
        seconds -= minutes * 60;

        if (minutes > 0)
        {
            if (seconds > 0)
            {
                return $"{minutes}m {seconds}s";
            }
            else
            {
                return $"{minutes}m";
            }
        }
        else
        {
            return $"{seconds}s";
        }
    }

    private const float w0 = 80;
    private const float w1 = 80;
    private const float w2 = 80;
    private const float w3 = 80;
    private const float w4 = 80;
    private const float w5 = 80;
    private const float w6 = 80;
    private const float w7 = 80;

    private void DisplayScore(Score score)
    {
        var origColor = GUI.color;
        GUILayout.BeginHorizontal();
        GUI.color = score.isNew ? new Color(0, 100, 0) : Color.blue;
        GUILayout.Label(score.map.ToString(), GUILayout.Width(w0));
        GUILayout.Label(IntervalToString(score.interval), GUILayout.Width(w1));
        GUILayout.Label(score.isNew.ToString(), GUILayout.Width(w2));
        GUILayout.Label(score.collected.ToString(), GUILayout.Width(w3));
        GUILayout.Label(score.crashes.ToString(), GUILayout.Width(w4));
        GUILayout.Label(score.recall.ToString(), GUILayout.Width(w5));
        GUILayout.Label(score.precision.ToString(), GUILayout.Width(w6));
        GUILayout.Label(score.f1.ToString(), GUILayout.Width(w7));
        GUI.color = origColor;
        GUILayout.EndHorizontal();
    }

    // Keep scroll position between scene loads
    private static Vector2 scrollPosition;

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(300);
        GUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Map", GUILayout.Width(w0));
        GUILayout.Label("Time", GUILayout.Width(w1));
        GUILayout.Label("New", GUILayout.Width(w2));
        GUILayout.Label("Score", GUILayout.Width(w3));
        GUILayout.Label("Crashes", GUILayout.Width(w4));
        GUILayout.Label("Recall", GUILayout.Width(w5));
        GUILayout.Label("Precision", GUILayout.Width(w6));
        GUILayout.Label("F1", GUILayout.Width(w7));
        GUILayout.EndHorizontal();

        GUILayout.Space(3);

        scrollPosition =
            GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(w0 + w1 + w2 + w3 + w4 + w5 + w6 + w7 + 70));

        foreach (var score in scores)
        {
            DisplayScore(score);
        }

        if (!finished)
        {
            DisplayScore(CurrentScore);
        }
        else
        {
            GUILayout.Space(10);
            GUILayout.Label("Finished");
        }

        GUILayout.Space(10);
        GUILayout.EndScrollView();

        GUILayout.Space(10);
        GUILayout.Label($"Iterations: {iterations}");

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}
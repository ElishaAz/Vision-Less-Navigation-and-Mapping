#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TimedPause : MonoBehaviour
{
    [SerializeField] private float duration;

    private float targetTime;

    private void Awake()
    {
        targetTime = Time.timeSinceLevelLoad + duration;
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad >= targetTime)
        {
            targetTime = Time.timeSinceLevelLoad + duration;
#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
        }
    }
}
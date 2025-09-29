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
        targetTime = Time.time + duration;
    }

    private void Update()
    {
        if (Time.time >= targetTime)
        {
            targetTime = Time.time + duration;
#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
        }
    }
}
using System;
using UnityEngine;

public class TimeScale : MonoBehaviour
{
    [SerializeField] private float timeScale = 1f;

    private void Update()
    {
        Time.timeScale = timeScale;
    }
}
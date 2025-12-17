using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ColorOverTime : MonoBehaviour
{
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private float time;

    private new MeshRenderer renderer;
    private float startTime;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        startTime = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        renderer.material.color = Color.Lerp(startColor, endColor, (Time.timeSinceLevelLoad - startTime) / time);
    }
}
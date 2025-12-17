using System;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    private void Awake()
    {
        if (!isActiveAndEnabled) return;

        var drone = FindAnyObjectByType<Drone.Drone>().gameObject.transform;
        drone.position = transform.position;
        drone.rotation = transform.rotation;
    }
}
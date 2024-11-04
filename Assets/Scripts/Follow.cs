using System;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool followYaw = false;

    private void Awake()
    {
        transform.position = target.position + offset;
        if (followYaw)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, target.eulerAngles.y, transform.eulerAngles.z);
    }

    private void Update()
    {
        transform.position = target.position + offset;
        if (followYaw)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, target.eulerAngles.y, transform.eulerAngles.z);
    }
}
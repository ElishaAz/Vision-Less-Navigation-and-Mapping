using System;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnRate = 1f;


    private float lastSpawnTime;

    private void Start()
    {
        lastSpawnTime = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad - lastSpawnTime < spawnRate) return;
        Instantiate(prefab, transform.position, Quaternion.identity);
        lastSpawnTime = Time.timeSinceLevelLoad;
    }
}
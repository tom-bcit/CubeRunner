using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnerShootingLeft : MonoBehaviour
{
    public GameObject obstaclePrefab; // Assign the cube prefab in the Inspector
    public float minSpawnTime = 1f; // Minimum time between spawns
    public float maxSpawnTime = 4f; // Maximum time between spawns
    public float spawnX = 10f; // X position of spawning
    public float spawnY = 30f; // Y position of spawning

    void Start()
    {
        StartCoroutine(SpawnObstacle());
    }

    IEnumerator SpawnObstacle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
            Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        }
    }
}

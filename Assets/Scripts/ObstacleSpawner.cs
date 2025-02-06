using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefabLeft; // Assign the cube prefab in the Inspector
    public GameObject obstaclePrefabRight; // Assign the cube prefab in the Inspector
    public float minSpawnTime = 1f; // Minimum time between spawns
    public float maxSpawnTime = 4f; // Maximum time between spawns
    public GameObject MyObstacleSpawner;
    public float spawnX;
    public float spawnY = 30f; // Y position of spawning
    public string direction;
    public GameObject GameManager;

    void Start()
    {
        spawnX = MyObstacleSpawner.transform.position.x;
        if (GameManager.GetComponent<PlayerSelection>().PlayerRole == "Host")
        {
            StartCoroutine(SpawnObstacle());
        }
    }

    IEnumerator SpawnObstacle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
            
            if (direction == "left")
            {
                Instantiate(obstaclePrefabLeft, spawnPosition, Quaternion.identity);
            }
            else
            {
                Instantiate(obstaclePrefabRight, spawnPosition, Quaternion.identity);
            }
        }
    }
}

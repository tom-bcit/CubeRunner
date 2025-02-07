using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    NetworkAPISocket.Messaging messaging = new NetworkAPISocket.Messaging();

    void Start()
    {
        messaging.Log += processMessage;
        (new Thread(new ThreadStart(messaging.ReceiveMessages))).Start();
        spawnX = MyObstacleSpawner.transform.position.x;
        if (GameManager.GetComponent<PlayerSelection>().PlayerRole == "Host")
        {
            StartCoroutine(SpawnObstacles());
        }
    }

    IEnumerator SpawnObstacles()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            Vector3 spawnPosition = new Vector3(15f, 0.52f + Random.Range(0, 3), 1f);
            SpawnObstacle(direction, spawnPosition);

        }
    }

    public void SpawnObstacle(string Direction, Vector3 SpawnPosition)
    {   
        if (direction == "left")
        {
            messaging.sendMessage($"dir = {Direction} x = {SpawnPosition.x}, y = {SpawnPosition.y}, z = {SpawnPosition.z}");
            Instantiate(obstaclePrefabLeft, SpawnPosition, Quaternion.identity);
        }
        else
        {
            SpawnPosition.x = -SpawnPosition.x;
            messaging.sendMessage($"dir = {Direction} x = {SpawnPosition.x}, y = {SpawnPosition.y}, z = {SpawnPosition.z}");
            Instantiate(obstaclePrefabRight, SpawnPosition, Quaternion.identity);
        }
    }

    public void processMessage(string message)
    {
        Debug.Log("Received message: " + message);
        if (GameManager.GetComponent<PlayerSelection>().PlayerRole == "Host")
        {
            // Handle message here
        }
    }

}

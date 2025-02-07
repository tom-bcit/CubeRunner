using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefabLeft; // Assign the cube prefab in the Inspector
    public GameObject obstaclePrefabRight; // Assign the cube prefab in the Inspector
    public float minSpawnTime = 1f; // Minimum time between spawns
    public float maxSpawnTime = 4f; // Maximum time between spawns
    public GameObject MyObstacleSpawner;
    public int ID;
    public float spawnX;
    public float spawnY = 30f; // Y position of spawning
    public string direction;
    public GameObject GameManager;
    public string role;
    NetworkAPISocket.Messaging messaging = new NetworkAPISocket.Messaging();

    void Start()
    {
        messaging.setId(ID);
        messaging.Log += processMessage;
        (new Thread(new ThreadStart(messaging.ReceiveMessages))).Start();
        spawnX = MyObstacleSpawner.transform.position.x;
        role = GameManager.GetComponent<PlayerSelection>().PlayerRole;
        if (role == "Host")
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
            messaging.sendMessage($"dir = {Direction}, x = {SpawnPosition.x}, y = {SpawnPosition.y}, z = {SpawnPosition.z}");
            Instantiate(obstaclePrefabLeft, SpawnPosition, Quaternion.identity);
        }
        else
        {
            SpawnPosition.x = -SpawnPosition.x;
            messaging.sendMessage($"dir = {Direction}, x = {SpawnPosition.x}, y = {SpawnPosition.y}, z = {SpawnPosition.z}");
            Instantiate(obstaclePrefabRight, SpawnPosition, Quaternion.identity);
        }
    }

    public void processMessage(string message)
    {
        Debug.Log("Spawner Received message: " + message + ", ROLE: " + role);
        if (role != "Host")
        {
            //Debug.Log("Message Received: " + message);
            string[] words = message.Split(' ');
            string[] parts = message.Split(new string[] { ", " }, StringSplitOptions.None);

            if (parts.Length != 4)
            {
                Debug.LogError("Malformed message. Expected 4 components (dir, x, y, z).");
                return;
            }

            // Attempt to parse each value, and validate structure
            string dirPart = parts[0];
            string[] dirParts = message.Split('=');
            //if (dirParts[0] == "dir")
            //{
            //    string dir = dirParts[1];
            //}
            float x = ParseValue(parts[1], "x");
            float y = ParseValue(parts[2], "y");
            float z = ParseValue(parts[3], "z");

            // Update the remote cube's position if all values are valid
            if (dirParts[0] == "dir" && !float.IsNaN(x) && !float.IsNaN(y) && !float.IsNaN(z))
            {
                SpawnObstacle(dirParts[1], new Vector3(x, y, z));
            }
            else
            {
                Debug.LogError("Failed to parse one or more values.");
            }
        }
    }

    private float ParseValue(string part, string expectedKey)
    {
        try
        {
            // Split by '=' and validate the key
            string[] keyValue = part.Split('=');
            if (keyValue.Length != 2 || keyValue[0].Trim() != expectedKey)
            {
                Debug.LogError($"Malformed component: {part}. Expected format '{expectedKey} = value'.");
                return float.NaN;
            }

            // Parse the numeric value
            return float.Parse(keyValue[1].Trim(), CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing value for {expectedKey}: {ex}");
            return float.NaN;
        }
    }

}

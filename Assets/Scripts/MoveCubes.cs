using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System;

public class MoveCubes : MonoBehaviour
{
    private float CUBE_HEIGHT = 1.0f;
    public GameObject localCube, remoteCube;
    public Vector3 localCubePos = new Vector3();
    public Vector3 remoteCubePos = new Vector3();
    NetworkAPI.Messaging messaging = new NetworkAPI.Messaging();
    void Awake() { }
    void OnEnable() { }
    void Start()
    {
        Debug.Log("Start");
        messaging.Log += processMessage;
        (new Thread(new ThreadStart(messaging.ReceiveMessages))).Start();
        localCubePos.x = 1.0f;
        localCubePos.y = CUBE_HEIGHT/2;
        localCubePos.z = 1.0f;
        remoteCubePos.x = -1.0f;
        remoteCubePos.y = CUBE_HEIGHT/2;
        remoteCubePos.z = 2.5f;
        localCube = GameObject.Find("CubeA");
        remoteCube = GameObject.Find("CubeB"); 
        remoteCube.transform.position = remoteCubePos;
        localCube.transform.position = localCubePos;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.RightArrow)) { localCubePos.x += 0.1f; }
            if (Input.GetKey(KeyCode.LeftArrow)) { localCubePos.x -= 0.1f; }
            if (Input.GetKey(KeyCode.UpArrow)) { localCubePos.y += 0.1f; }
            if (Input.GetKey(KeyCode.DownArrow)) { 
                float res = localCubePos.y -= 0.1f;
                if (res < CUBE_HEIGHT/2) localCubePos.y = CUBE_HEIGHT/2; }

            localCube.transform.position = localCubePos;
            if (Vector3.Distance(localCubePos, remoteCubePos) < 2.0f)
                // Debug.Log("Collision Detected");
                messaging.sendMessage($"x = {localCubePos.x}, y = {localCubePos.y}, z = {localCubePos.z}");
        }
        remoteCube.transform.position = remoteCubePos;
    }
    void OnDisable() { Debug.Log("OnDisable Called"); }

    public void processMessage(string message)
    {
        Debug.Log("Message Received: " + message);
        string[] words = message.Split(' ');
        string[] parts = message.Split(new string[] { ", " }, StringSplitOptions.None);

        if (parts.Length != 3)
        {
            Debug.LogError("Malformed message. Expected 3 components (x, y, z).");
            return;
        }

        // Attempt to parse each value, and validate structure
        float x = ParseValue(parts[0], "x");
        float y = ParseValue(parts[1], "y");
        float z = ParseValue(parts[2], "z");

        // Update the remote cube's position if all values are valid
        if (!float.IsNaN(x) && !float.IsNaN(y) && !float.IsNaN(z))
        {
            remoteCubePos.x = x;
            remoteCubePos.y = y;
            remoteCubePos.z = z;
        }
        else
        {
            Debug.LogError("Failed to parse one or more values.");
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

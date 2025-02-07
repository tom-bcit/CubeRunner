using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
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
    // NetworkAPISocket.Messaging messaging = new NetworkAPISocket.Messaging();
    void Awake() { }
    void OnEnable() { }
    void Start()
    {
        Debug.Log("Start");
        messaging.Log += processMessage;
        (new Thread(new ThreadStart(messaging.ReceiveMessages))).Start();
        localCubePos.x = 1.0f;
        localCubePos.y = CUBE_HEIGHT / 2;
        localCubePos.z = 1.0f;
        remoteCubePos.x = -1.0f;
        remoteCubePos.y = CUBE_HEIGHT / 2;
        remoteCubePos.z = 1.0f;

        remoteCube.transform.position = remoteCubePos;
        localCube.transform.position = localCubePos;
    }
    // Update is called once per frame
    void Update()
    {
        bool isGrounded = localCube.transform.position.y <= CUBE_HEIGHT / 2;
        localCubePos = localCube.transform.position;
        if (isGrounded) RemoveVerticalVelocity();
        else
            localCubePos.y = localCube.transform.position.y;
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.RightArrow)) { localCubePos.x += 0.1f; }
            else if (Input.GetKey(KeyCode.LeftArrow)) { localCubePos.x -= 0.1f; }
            else if (Input.GetKey(KeyCode.UpArrow)) { Jump(isGrounded); }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                RemoveVerticalAcceration();
                float res = localCubePos.y -= 0.1f;
                if (res < CUBE_HEIGHT / 2) localCubePos.y = CUBE_HEIGHT / 2;
            }

            localCube.transform.position = localCubePos;
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

    private void Jump(bool isGrounded)
    {
        float jumpForce = 1.5f;
        if (isGrounded)
        {
            StartCoroutine(DisableGravityForRise());
            Rigidbody localCubeRb = localCube.GetComponent<Rigidbody>();
            localCubeRb.velocity = new Vector3(localCubeRb.velocity.x, 6f, localCubeRb.velocity.z);
            localCubeRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }
    }

    private void RemoveVerticalVelocity()
    {
        Rigidbody localCubeRb = localCube.GetComponent<Rigidbody>();
        localCubeRb.velocity = new Vector3(localCubeRb.velocity.x, 0f, localCubeRb.velocity.z);
    }

    private void RemoveVerticalAcceration()
    {
        Rigidbody localCubeRb = localCube.GetComponent<Rigidbody>();
        if (localCubeRb.velocity.y > 0)
            localCubeRb.velocity = new Vector3(localCubeRb.velocity.x, 0f, localCubeRb.velocity.z);
    }

    private IEnumerator DisableGravityForRise()
    {
        Rigidbody localCubeRb = localCube.GetComponent<Rigidbody>();

        // Temporarily disable gravity for a brief period during the jump rise
        localCubeRb.useGravity = false;

        yield return new WaitForSeconds(0.1f); // Let the cube rise fast for 0.1 seconds

        // Re-enable gravity for the normal fall
        localCubeRb.useGravity = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) // Check if the colliding object is an obstacle
        {
            Debug.Log("An obstacle was hit!");

            // Determine which player was hit
            if (gameObject.CompareTag("CubeA"))
            {
                Debug.Log("Local player hit by obstacle!");
                PlayerScoreManager.instance.AddPoint();
            }
            else if (gameObject.CompareTag("CubeB"))
            {
                Debug.Log("Remote player hit by obstacle!");
                EnemyScoreManager.instance.AddPoint();
                // Handle remote player getting hit (e.g., send a network message)
            }
            PlayerScoreManager.instance.CheckColor(); // Update local player's score
            EnemyScoreManager.instance.CheckColor(); // Update remote player's score
        }
    }

    private void OnApplicationQuit()
    {
        messaging.Close();
    }
}

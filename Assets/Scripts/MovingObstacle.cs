using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    // Update is called once per frame automatically by Unity
    public float speed = 5f;
    public float lifetime = 10f; // Time before the obstacle is destroyed

    void Start()
    {
        transform.position = new Vector3(15f, 0.52f + Random.Range(0, 3), 1f); // Set the spawn position
        Destroy(gameObject, lifetime); // Destroy after 'lifetime' seconds
    }


    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}

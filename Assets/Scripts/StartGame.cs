using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject obstacleSpawner;
    public GameObject obstacleSpawner2;
    public GameObject startButton;

    public void OnStartButtonClick()
    {
        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetActive(true);
        }
        if (obstacleSpawner2 != null)
        {
            obstacleSpawner2.SetActive(true);
        }
        if (startButton != null)
        {
            startButton.SetActive(false);
        }
    }
}

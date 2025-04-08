
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject obstacleSpawner;
    public GameObject obstacleSpawner2;
    public GameObject player;
    public GameObject playerScore;
    public GameObject enemy;
    public GameObject enemyScore;
    public GameObject startButton;

    void Start() {
        obstacleSpawner.SetActive(false);
        obstacleSpawner2.SetActive(false);
        player.SetActive(false);
        enemy.SetActive(false);
        playerScore.SetActive(false);
        enemyScore.SetActive(false);
    }

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
        player.SetActive(player != null);
        enemy.SetActive(enemy != null);

        playerScore.SetActive(playerScore != null);
        enemyScore.SetActive(enemyScore != null);
        
        if (startButton != null)
        {
            startButton.SetActive(false);
        }
    }
}

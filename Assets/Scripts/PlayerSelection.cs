
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public GameObject CubeA;
    public GameObject CubeB;
    public GameObject HostButton;
    public GameObject Player1Button;
    public GameObject StartMenu;
    public GameObject SelectionMenu;
    public GameObject obstacleSpawner;
    public GameObject obstacleSpawner2;
    public string PlayerRole;

    public void OnHostButtonClick()
    {
        CubeA.GetComponent<MoveCubes>().ID = 1;
        CubeA.GetComponent<MoveCubes>().localCube = CubeA;
        CubeA.GetComponent<MoveCubes>().remoteCube = CubeB;
        CubeB.GetComponent<MoveCubes>().ID = 1;
        CubeB.GetComponent<MoveCubes>().localCube = CubeA;
        CubeB.GetComponent<MoveCubes>().remoteCube = CubeB;
        obstacleSpawner.GetComponent<ObstacleSpawner>().ID = 2;
        obstacleSpawner2.GetComponent<ObstacleSpawner>().ID = 3;
        SelectionMenu.SetActive(false);
        StartMenu.SetActive(true);
        PlayerRole = "Host";
    }

    public void OnPlayer1ButtonClick()
    {
        CubeA.GetComponent<MoveCubes>().ID = 4;
        CubeA.GetComponent<MoveCubes>().localCube = CubeB;
        CubeA.GetComponent<MoveCubes>().remoteCube = CubeA;
        CubeB.GetComponent<MoveCubes>().ID = 4;
        CubeB.GetComponent<MoveCubes>().localCube = CubeB;
        CubeB.GetComponent<MoveCubes>().remoteCube = CubeA;
        obstacleSpawner.GetComponent<ObstacleSpawner>().ID = 5;
        obstacleSpawner2.GetComponent<ObstacleSpawner>().ID = 6;
        SelectionMenu.SetActive(false);
        StartMenu.SetActive(true);
        PlayerRole = "Player1";
    }

}

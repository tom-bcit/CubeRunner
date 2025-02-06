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
    public string PlayerRole;

    public void OnHostButtonClick()
    {
        CubeA.GetComponent<MoveCubes>().localCube = CubeA;
        CubeA.GetComponent<MoveCubes>().remoteCube = CubeB;
        CubeB.GetComponent<MoveCubes>().localCube = CubeA;
        CubeB.GetComponent<MoveCubes>().remoteCube = CubeB;
        SelectionMenu.SetActive(false);
        StartMenu.SetActive(true);
        PlayerRole = "Host";
    }

    public void OnPlayer1ButtonClick()
    {
        CubeA.GetComponent<MoveCubes>().localCube = CubeB;
        CubeA.GetComponent<MoveCubes>().remoteCube = CubeA;
        CubeB.GetComponent<MoveCubes>().localCube = CubeB;
        CubeB.GetComponent<MoveCubes>().remoteCube = CubeA;
        SelectionMenu.SetActive(false);
        StartMenu.SetActive(true);
        PlayerRole = "Player1";
    }

}

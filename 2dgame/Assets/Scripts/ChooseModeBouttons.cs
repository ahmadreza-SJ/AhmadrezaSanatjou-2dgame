using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Entities;



/// <summary>
/// this class has the functions of mode (single-player and multi-player) buttons
/// </summary>
public class ChooseModeBouttons : MonoBehaviour
{
    public GameObject SinglePlayerBtn;
    public GameObject MultiPlayerBtn;
    public GameObject JoinMatchBtn;
    public GameObject PhotonLobby;
    public GameObject RoomController;
    public GameObject Loading;
    public GameObject BackBtn;
    public GameObject ExitBtn;

    public void OnSinglePlayerBtnClick()
    {
        SceneManager.LoadScene("SinglePlayerLevel1");
        Debug.Log("Level 1");
    }

    public void OnMultiPlayerBtnClick()
    {
        ExitBtn.SetActive(false);
        SinglePlayerBtn.SetActive(false);
        MultiPlayerBtn.SetActive(false);
        PhotonLobby.SetActive(true);
        RoomController.SetActive(true);
        Loading.SetActive(true);
        BackBtn.SetActive(true);
    }

    public void OnBackBtnClick()
    {
        ExitBtn.SetActive(true);
        SinglePlayerBtn.SetActive(true);
        MultiPlayerBtn.SetActive(true);
        BackBtn.SetActive(false);
        RoomController.SetActive(false);
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}



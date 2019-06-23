using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


/// <summary>
/// this script handles the connection to the master Photon server 
/// and lets the player to join a room to begin a match
/// </summary>
public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;
    RoomInfo[] rooms;

    public GameObject JoinMatchBtn;
    public GameObject CancelBtn;

    private void Awake()
    {
        lobby = this; //creates singletone
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // connects to master photon server
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master photon server");
        PhotonNetwork.AutomaticallySyncScene = true;
        JoinMatchBtn.SetActive(true); // by clicking this button, the pleyer will try to join a room
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected!");
    }

    public void OnJoinMatchBtnClick()
    {
        JoinMatchBtn.SetActive(false);
        CancelBtn.SetActive(true);
        PhotonNetwork.JoinRandomRoom(); // trying to join a random room
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room, maybe there are no random room available");
        CreateRoom();
    }

    void CreateRoom() // creates a new random room to join
    {
        Debug.Log("Creating a new random room");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom("Room " + randomRoomName, roomOps); // try to create a random room with specified values

    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a new random room, maybe a random room with the same name already exists");
        Debug.Log("trying to create another room");
        CreateRoom();// try to create another room
    }

    public void OnCancelBtnClick() // by clicking the cancel button, the player will leave the room
    {
        CancelBtn.SetActive(false);
        JoinMatchBtn.SetActive(true);
        PhotonNetwork.LeaveRoom();
        Debug.Log("Left the room");
    }
}

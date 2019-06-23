using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    public static PhotonRoom room; //singleton holder
    private PhotonView PV;

    public int CurrentScene;
    public int MultiplayerScene;

    public Player[] PlayerList;

    private void Awake()
    {
        // set up singleton
        if (room == null)
        {
            room = this;
        }
        else
        {
            if (room != this)
            {
                Destroy(room.gameObject);
                room = this;
            }
        }
        DontDestroyOnLoad(gameObject);
        PV = GetComponent<PhotonView>();
    }


    public override void OnEnable()
    {
        //subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishLoading; //register our function to action event of loading a new scene
    }


    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishLoading; //remove our function from action event of loading a new scene 
    }


    public override void OnJoinedRoom() // executes when the player've joined the room
    {
        base.OnJoinedRoom();
        Debug.Log("Joined the Room SuccessFully");

        StartGame();
    }

    void StartGame()
    {
        //load the multiplayerscene for all players
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        Debug.Log("Loading level");
        PhotonNetwork.LoadLevel(MultiplayerScene);
    }

    void OnSceneFinishLoading(Scene scene, LoadSceneMode mode)
    {
        CurrentScene = scene.buildIndex;
        if (CurrentScene == MultiplayerScene)
        {
            CreatePlayer();
        }
    }

    void CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

}

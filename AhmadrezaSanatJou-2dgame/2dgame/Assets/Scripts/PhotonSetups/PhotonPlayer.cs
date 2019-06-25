using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Entities;

public class PhotonPlayer : MonoBehaviour
{
    PhotonView PV;
    public GameObject myAvatar;
    public Player[] PlayerList;

    string characterPicker;
    int spawnPicker;

    // Start is called before the first frame update
    void Start()
    {
        PlayerList = PhotonNetwork.PlayerList;
        
        PV = GetComponent<PhotonView>();

        if (PlayerList.Length == 1) // the first player who joins the room will be the Cooper on the right side
        {
            spawnPicker = 0;
            characterPicker = "CooperAvatar";
        }
        else // the second player who joins the room will be the Spider on the left side
        {
            spawnPicker = 1;
            characterPicker = "SpiderAvatar";
        }

        if (PV.IsMine)
        {

            gameObject.tag = "LocalClient";
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", characterPicker),
            GameDetails.GD.SpawnPoints[spawnPicker].position, GameDetails.GD.SpawnPoints[spawnPicker].rotation, 0);
            ClientAvatarsStates.State.LocalClientAvatar = myAvatar;
        }
        else
        {
            gameObject.tag = "RemoteClient";
        }
    }

    private void OnDisable()
    {
        if(characterPicker == "CooperAvatar")
        {
            World.Active.EntityManager.DestroyEntity(AvatarEntityManager.Manager.CooperEntity);
        }
        else
        {
            World.Active.EntityManager.DestroyEntity(AvatarEntityManager.Manager.SpiderEntity);
        }
    }
}

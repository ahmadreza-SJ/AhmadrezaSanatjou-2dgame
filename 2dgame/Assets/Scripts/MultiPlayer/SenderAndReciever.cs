using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


//this script assigns the Avatar to local or remote avatar in ClientAvatarsStates
public class SenderAndReciever : MonoBehaviourPunCallbacks, IPunObservable
{


    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }


    void Start()
    {
        if(gameObject.GetComponent<PhotonView>().IsMine)
        {

            ClientAvatarsStates.State.LocalClientAvatar = gameObject;
        }
        else
        {
            ClientAvatarsStates.State.RemoteClientAvatar = gameObject;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(ClientAvatarsStates.State == null)
        {
            return;
        }
        if (stream.IsWriting)
        {
            stream.SendNext(ClientAvatarsStates.State.LocalIsShooting);
            ClientAvatarsStates.State.LocalIsShooting = false;

            stream.SendNext(ClientAvatarsStates.State.LocalIsDamaged);
            ClientAvatarsStates.State.LocalIsDamaged = false;
        }
        else
        {
            ClientAvatarsStates.State.RemoteIsShooting = (bool)stream.ReceiveNext();

            bool RID = (bool)stream.ReceiveNext();
            if (!ClientAvatarsStates.State.RemoteIsDamaged && RID)
            {
                ClientAvatarsStates.State.RemoteIsDamaged = RID;
            }
           
        }
    }


    private void OnDestroy()
    {
        GameDetails.GD.DestroyAll();
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
            

    }
}

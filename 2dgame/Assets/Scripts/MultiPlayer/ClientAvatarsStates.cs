using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAvatarsStates : MonoBehaviour
{
    public static ClientAvatarsStates State;

    public  Vector3 Position;


    public GameObject LocalClientAvatar;
    public GameObject RemoteClientAvatar;

    public bool LocalIsShooting;
    public bool RemoteIsShooting;

    public bool LocalIsDamaged;
    public bool RemoteIsDamaged;

    private void Awake()
    {
        if(State == null)
        {
            State = this;

            LocalIsShooting = false;
            RemoteIsShooting = false;

            LocalIsDamaged = false;
            RemoteIsDamaged = false;
        }
    }

    private void Update()
    {
        LocalClientAvatar.transform.position = Position;
    }

}

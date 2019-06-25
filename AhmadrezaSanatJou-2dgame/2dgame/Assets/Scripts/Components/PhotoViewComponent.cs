using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Photon.Pun;



public struct PhotonViewData : IComponentData
{
    public bool isMine;

}

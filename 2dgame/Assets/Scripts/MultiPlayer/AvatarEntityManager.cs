using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class AvatarEntityManager : MonoBehaviour
{
    public static AvatarEntityManager Manager;
    
        
    public Entity CooperEntity;
    public Entity SpiderEntity;

    private void OnEnable()
    {
        if(Manager == null )
        {
            Manager = this;
        }

    }

}

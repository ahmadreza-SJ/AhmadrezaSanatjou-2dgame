using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderGamePlayController : MonoBehaviour
{
    public static SpiderGamePlayController Controller;

    
    public float Speed = 2;
    public int Heart = 1;
    public float ColliderRadius = 0.5f;
    public float ShootingDelay = 1;
    public float ShootingChance;
    
    private void Awake()
    {
        if (Controller == null || Controller != this)
        {
            Controller = this;
        }
    }

}

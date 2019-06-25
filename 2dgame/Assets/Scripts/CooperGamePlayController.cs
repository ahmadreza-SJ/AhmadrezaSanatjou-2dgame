using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooperGamePlayController : MonoBehaviour
{
    public static CooperGamePlayController Controller;

    public float VerticalSpeed;
    public float HorizontalSpeed = 3;
    public int TotalShotsAtStart = 300;
    public int Heart = 5;
    public int ShotCountPerLoad = 5;
    public float TimeBetweenShoots = 0.5f;
    public float ReloadingTime = 2;
    public float ColliderRadius = 0.5f;

    private void Awake()
    {
        if(Controller == null || Controller != this)
        {
            Controller = this;
        }
    }

}

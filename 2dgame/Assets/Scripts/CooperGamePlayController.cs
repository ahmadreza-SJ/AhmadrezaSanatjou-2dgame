using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooperGamePlayController : MonoBehaviour
{
    public static CooperGamePlayController Controller;

    public float VerticalSpeed;
    public float HorizontalSpeed;

    private void Awake()
    {
        if(Controller == null || Controller != this)
        {
            Controller = this;
        }
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderGamePlayController : MonoBehaviour
{
    public static SpiderGamePlayController Controller;

    public float VerticalSpeed;
    public float HorizontalSpeed;

    private void Awake()
    {
        if (Controller == null || Controller != this)
        {
            Controller = this;
        }
    }

}

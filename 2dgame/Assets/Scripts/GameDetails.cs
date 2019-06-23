using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDetails : MonoBehaviour
{
    //saves the view width and height for spawning and moving boundaries

    public Camera Camera;
    public float CameraHalfHeight;
    public float CameraHalfWidth;

    public float VerticalBoundary;
    public float HorizontalBoundary;

    public static GameDetails GD;

    private void Awake()
    {
        if(GD == null || GD != null)
        {
            GD = this;
        }

        //initializing the view width and height
        Camera = Camera.main;
        CameraHalfHeight = Camera.orthographicSize;
        CameraHalfWidth = Camera.aspect * CameraHalfHeight;

    }

}

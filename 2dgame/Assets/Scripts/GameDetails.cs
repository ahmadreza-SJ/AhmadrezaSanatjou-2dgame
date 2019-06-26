using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.UI;
using Photon.Pun;

public class GameDetails : MonoBehaviour
{
    //saves the view width and height for spawning and moving boundaries

    //mesh and material referances to be rendered


    public int Level;

    public bool IsMultiPlayer = false;

    // count of spiders can be entered depend on the level
    public int SpiderCount;

    

    public Camera Camera;
    public float CameraHalfHeight;
    public float CameraHalfWidth;

    public float VerticalBoundary = 1;
    public float HorizontalBoundary = 1;

    public Transform[] SpawnPoints; // for multi player

    public Mesh Mesh;
    public Material CooperMaterial;
    public Material SpiderMaterial;
    public Material CooperShotMaterial;
    public Material SpiderShotMaterial;
    public Material CoinMaterial;

    public Text HeartText;
    public Text TotalShotText;
    public Text ScoreText;


    public float ShotSpeed = 4;
    public float CoinSpeed = 3;
    public float CoinSpriteRadius = 0.5f;
    public int EachCoinScoreValue = 10;
    public int Score = 0;


    public int CurrentHealth;
    public int CurrentShots;
    public int KilledSpidersCount;

    // this set is used to find the shot which collides a spider. because it's inaccessible from the inside of shotCollidingJob
    public HashSet<int> DestroyedShotsIndexes = new HashSet<int>()  ;

    public static GameDetails GD;

    public PhotonView PV;



    private void Awake()
    {
        if(GD == null || GD != null)
        {
            GD = this;
        }
        
        if (IsMultiPlayer)
        {
            PV = gameObject.AddComponent<PhotonView>();
        }

        //initializing the view width and height
        Camera = Camera.main;
        CameraHalfHeight = Camera.orthographicSize;
        CameraHalfWidth = Camera.aspect * CameraHalfHeight;

        KilledSpidersCount = 0;
        
    }

    private void Start()
    {
        CooperGamePlayController cgpc = CooperGamePlayController.Controller;
        CurrentHealth = cgpc.Heart;
        CurrentShots = cgpc.TotalShotsAtStart;
    }

    private void Update()
    {
        if(!IsMultiPlayer)
        {
            if (KilledSpidersCount == SpiderCount)
            {
                switch (Level)
                {
                    case 1:
                        {
                            DestroyAll();
                            SceneManager.LoadScene("SinglePlayerLevel2");

                            Debug.Log("Level 2");
                            break;
                        }
                    case 2:
                        {
                            DestroyAll();
                            SceneManager.LoadScene("MainMenu");
                            Debug.Log("You Won!");
                            break;
                        }
                }
            }

            HeartSetText(CurrentHealth);
            TotalShotSetText(CurrentShots);
            ScoreSetText(Score);
        }
        else
        {
            if(CooperGamePlayController.Controller.Heart <= 0)
            {
                if (ClientAvatarsStates.State.LocalClientAvatar.name == "CooperAvatar")
                {
                    Debug.Log("You Lost");
                }
                else
                {
                    Debug.Log("You Won");
                }
                PhotonNetwork.LeaveRoom();
                DestroyAll();
                SceneManager.LoadScene("MainMenu");

            }
            else if (SpiderGamePlayController.Controller.Heart <= 0)
            {
                if (ClientAvatarsStates.State.LocalClientAvatar.name == "CooperAvatar")
                {
                    Debug.Log("You Won");
                }
                else
                {
                    Debug.Log("You Lost");
                }
                PhotonNetwork.LeaveRoom();
                DestroyAll();
                SceneManager.LoadScene("MainMenu");

            }
        }
        
        

        if(CurrentHealth <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        DestroyAll();
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Game Over!");
    }

    public void DestroyAll()
    {
        EntityManager eManager = World.Active.EntityManager;
        NativeArray<Entity> all = eManager.GetAllEntities();
        eManager.DestroyEntity(all);
    }

    public void BackToMenu()
    {
        DestroyAll();
        SceneManager.LoadScene("MainMenu");
        
    }

    public void TotalShotSetText(int bulletCount)
    {
        TotalShotText.text = "Bullets Reaminig : " + bulletCount;
    }

    public void HeartSetText(int HeartCount)
    {
        HeartText.text = "Hearts : " + HeartCount;
    }


    public void ScoreSetText(int HeartCount)
    {
        ScoreText.text = "Score : " + HeartCount;
    }

    
}

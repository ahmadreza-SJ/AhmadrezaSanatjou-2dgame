using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;


/// <summary>
/// this class has the functions of mode (single-player and multi-player) buttons
/// </summary>
public class ChooseModeBouttons : MonoBehaviour
{
    
    public void OnSinglePlayerBtnClick()
    {
        SceneManager.LoadScene("SinglePlayerLevel1");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerSetText : MonoBehaviour
{

    public Text CooperHeart;
    public Text SpiderHeart;
    bool IsCooper;


    public static MultiplayerSetText HeartTexts;

    private void Awake()
    {
        if(HeartTexts == null)
        {
            HeartTexts = this;
        }
    }

    private void Update()
    {
        SetCooperHeartText(CooperGamePlayController.Controller.Heart);
        SetSpiderHeartText(SpiderGamePlayController.Controller.Heart);
        
    }

    public void SetCooperHeartText(int HeartCount)
    {
        CooperHeart.text = "Cooper Remaining Hearts : " + HeartCount;
    }

    public void SetSpiderHeartText(int HeartCount)
    {
        SpiderHeart.text = "Spider Remaining Hearts : " + HeartCount;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonMGR : MonoBehaviour
{
    [SerializeField] GameObject PauseTheGameCanvas;   //インスペクター上でセットする
    public bool isPausingAtMakeTheFirstRoad;   //MakeTheFirstRoadのときのエラー回避のため。BackToGameButtonMGRからも使う

    public void OpenPauseMenu()
    {
        if (GameManager.instance.state == GameManager.State.MakeTheFirstRoad)
        {
            isPausingAtMakeTheFirstRoad = true;
            Debug.Log("PauseTheGameを呼びます");
            GameManager.instance.PauseTheGame();
            PauseTheGameCanvas.SetActive(true);
        }
        else
        {
            Debug.Log("PauseTheGameを呼びます");
            GameManager.instance.PauseTheGame();
            PauseTheGameCanvas.SetActive(true);
        }
        //Debug.Log("PauseTheGameを呼びます");
        //GameManager.instance.PauseTheGame();
        //PauseTheGameCanvas.SetActive(true);
    }
}

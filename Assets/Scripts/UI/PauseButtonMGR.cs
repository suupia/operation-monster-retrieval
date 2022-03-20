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
            GameManager.instance.PauseTheGame();
            PauseTheGameCanvas.SetActive(true);
        }
        else
        {
            GameManager.instance.PauseTheGame();
            PauseTheGameCanvas.SetActive(true);

            //Pause時には、ManualRoute関連で操作中の物は全てリセットする
            GameManager.instance.curveToMouseMGR.ResetCopyingManualRoue();
            if(GameManager.instance.inputMGR.GetManualRouteNumber() != -1)
            {
                GameManager.instance.selectCharacterButtonMGRs[GameManager.instance.inputMGR.GetManualRouteNumber()].ResetToNormalColor();
            }
        }
    }
}

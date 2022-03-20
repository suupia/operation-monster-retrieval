using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonMGR : MonoBehaviour
{
    [SerializeField] GameObject PauseTheGameCanvas;   //�C���X�y�N�^�[��ŃZ�b�g����
    public bool isPausingAtMakeTheFirstRoad;   //MakeTheFirstRoad�̂Ƃ��̃G���[����̂��߁BBackToGameButtonMGR������g��

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

            //Pause���ɂ́AManualRoute�֘A�ő��쒆�̕��͑S�ă��Z�b�g����
            GameManager.instance.curveToMouseMGR.ResetCopyingManualRoue();
            if(GameManager.instance.inputMGR.GetManualRouteNumber() != -1)
            {
                GameManager.instance.selectCharacterButtonMGRs[GameManager.instance.inputMGR.GetManualRouteNumber()].ResetToNormalColor();
            }
        }
    }
}

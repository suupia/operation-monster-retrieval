using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavingCombatButton : MonoBehaviour
{
    [SerializeField] GameObject pauseTheGameCanvas; //インスペクター上でセットする。（InputMGRにも同様にしてセットしていることに注意）
    public void LeavingCombat() //EventTriggerで呼ぶ
    {
        GameManager.instance.StartSelectingStage();
        GameManager.instance.selectStageCanvas.SetActive(false);
        GameManager.instance.menuCanvas.SetActive(true);

        //pauseTheGameCanvas.SetActive(false);

        GameManager.instance.inputMGR.ClosePauseTheGameCanvas();
    }
}

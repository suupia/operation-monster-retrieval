using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMenuButtonMGR : MonoBehaviour
{
    public void BackToMenu() //EventTrigger�ŌĂ�
    {
        GameManager.instance.StartSelectingStage();
        GameManager.instance.selectStageCanvas.SetActive(false);
        GameManager.instance.menuCanvas.SetActive(true);
    }

}

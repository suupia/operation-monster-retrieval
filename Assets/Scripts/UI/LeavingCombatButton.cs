using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavingCombatButton : MonoBehaviour
{
    [SerializeField] GameObject pauseTheGameCanvas; //�C���X�y�N�^�[��ŃZ�b�g����B�iInputMGR�ɂ����l�ɂ��ăZ�b�g���Ă��邱�Ƃɒ��Ӂj
    public void LeavingCombat() //EventTrigger�ŌĂ�
    {
        GameManager.instance.StartSelectingStage();
        GameManager.instance.selectStageCanvas.SetActive(false);
        GameManager.instance.menuCanvas.SetActive(true);

        //pauseTheGameCanvas.SetActive(false);

        GameManager.instance.inputMGR.ClosePauseTheGameCanvas();
    }
}

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
            Debug.Log("PauseTheGame���Ăт܂�");
            GameManager.instance.PauseTheGame();
            PauseTheGameCanvas.SetActive(true);
        }
        else
        {
            Debug.Log("PauseTheGame���Ăт܂�");
            GameManager.instance.PauseTheGame();
            PauseTheGameCanvas.SetActive(true);
        }
        //Debug.Log("PauseTheGame���Ăт܂�");
        //GameManager.instance.PauseTheGame();
        //PauseTheGameCanvas.SetActive(true);
    }
}

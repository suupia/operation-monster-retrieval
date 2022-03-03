using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToTheGameButtonMGR : MonoBehaviour
{
    [SerializeField] PauseButtonMGR pauseButtonMGR;
    [SerializeField] GameObject PauseTheGameCanvas;
    public void BackToTheGame()
    {
        if (pauseButtonMGR.isPausingAtMakeTheFirstRoad)
        {
            pauseButtonMGR.isPausingAtMakeTheFirstRoad = false;
            Debug.Log("MakeTheFirstRoad���Ăт܂�");
            GameManager.instance.MakeTheFirstRoad();
            PauseTheGameCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("RuuningGame���Ăт܂�");
            GameManager.instance.RunningGame();
            PauseTheGameCanvas.SetActive(false);
        }
    }
}

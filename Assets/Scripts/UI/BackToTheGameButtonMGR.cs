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
            Debug.Log("MakeTheFirstRoadを呼びます");
            GameManager.instance.MakeTheFirstRoad();
            PauseTheGameCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("RuuningGameを呼びます");
            GameManager.instance.RunningGame();
            PauseTheGameCanvas.SetActive(false);
        }
    }
}

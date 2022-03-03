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
            Debug.Log("MakeTheFirstRoad‚ðŒÄ‚Ñ‚Ü‚·");
            GameManager.instance.MakeTheFirstRoad();
            PauseTheGameCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("RuuningGame‚ðŒÄ‚Ñ‚Ü‚·");
            GameManager.instance.RunningGame();
            PauseTheGameCanvas.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSelectStageButton : MonoBehaviour
{
    public void OpenSelectStage() //EventTrigger‚ÅŒÄ‚Ô
    {
        GameManager.instance.menuCanvas.SetActive(false);
        GameManager.instance.selectStageCanvas.SetActive(true);
    }
}

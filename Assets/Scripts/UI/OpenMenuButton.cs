using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMenuButton : MonoBehaviour
{
    public void OpenMenu() //EventTrigger‚ÅŒÄ‚Ô
    {
        GameManager.instance.selectStageCanvas.SetActive(false);
        GameManager.instance.menuCanvas.SetActive(true);
    }
}

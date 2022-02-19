using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenButton : MonoBehaviour
{

    public void SwitchFullScreen() //EventTrigger‚ÅŒÄ‚Ô
    {
        GameManager.instance.statusCanvasMGR.SwitchFullScreen();
    }

}

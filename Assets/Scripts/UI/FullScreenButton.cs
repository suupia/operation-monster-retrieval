using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenButton : MonoBehaviour
{

    public void SwitchFullScreen() //EventTrigger�ŌĂ�
    {
        GameManager.instance.statusCanvasMGR.SwitchFullScreen();
    }

}

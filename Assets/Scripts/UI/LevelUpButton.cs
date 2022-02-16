using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelUpButton : MonoBehaviour
{
    public void LevelUpCharacter() //EventTrigger‚ÅŒÄ‚Ô
    {
       GameManager.instance.statusCanvasMGR.LevelUpCharacterDisplayed();
    }
}

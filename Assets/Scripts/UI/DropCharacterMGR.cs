using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCharacterMGR : MonoBehaviour
{
    [SerializeField] int dropNum;

    public void DropCharacter() //Event Trigger‚ÅŒÄ‚Ô
    {
        Debug.LogWarning($"{dropNum}‚Éƒhƒƒbƒv‚³‚ê‚Ü‚µ‚½");

        GameManager.instance.DropCharacterData(dropNum);

    }

    public void PointerDownCharacterInCombat()
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInCombat(dropNum);
    }
}

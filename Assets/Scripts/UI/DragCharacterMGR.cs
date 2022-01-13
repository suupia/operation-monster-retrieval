using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCharacterMGR : MonoBehaviour
{
    [SerializeField] int dragNum;
    public void DragCharacter() //Event Trigger‚ÅŒÄ‚Ô
    {
        Debug.LogWarning($"{dragNum}‚ªƒhƒ‰ƒbƒO‚³‚ê‚Ü‚µ‚½");

        GameManager.instance.DragCharacterData(dragNum);
    }
}

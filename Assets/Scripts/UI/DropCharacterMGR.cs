using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCharacterMGR : MonoBehaviour
{
    [SerializeField] int dropNum;

    public void DropCharacter() //Event Trigger�ŌĂ�
    {
        Debug.LogWarning($"{dropNum}�Ƀh���b�v����܂���");

        GameManager.instance.DropCharacterData(dropNum);

    }

    public void PointerDownCharacterInCombat()
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInCombat(dropNum);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCharacterMGR : MonoBehaviour
{
    [SerializeField] int dragNum;
    public void DragCharacter() //Event Trigger�ŌĂ�
    {
        Debug.LogWarning($"{dragNum}���h���b�O����܂���");

        GameManager.instance.DragCharacterData(dragNum);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCharacterMGR : MonoBehaviour
{
    [SerializeField] int dragNum;
    public void DragCharacter() //Event Triggerで呼ぶ
    {
        Debug.LogWarning($"{dragNum}がドラッグされました");

        GameManager.instance.DragCharacterData(dragNum);
    }
}

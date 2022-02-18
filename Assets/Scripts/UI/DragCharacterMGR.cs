using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragCharacterMGR : MonoBehaviour
{
    [SerializeField] int dragNum; //CharacterInReverse(GameObject)の添え字をインスペクター上でセットしておく
    public void DragCharacter() //EventTriggerで呼ぶ
    {
        Debug.LogWarning($"{dragNum}がドラッグされました");

        GameManager.instance.DragCharacterData(dragNum);
    }

    public void PointerDownCharacterInReserve() //EventTriggerで呼ぶ（２つセットしていることに注意）
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInReserve(dragNum);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DropCharacterMGR : MonoBehaviour
{
    [SerializeField] int dropNum;
    Image thumbnailImage;

    private void Start()
    {
        thumbnailImage = GetComponent<Image>();
        if (dropNum < GameManager.instance.characterPrefabs.Length) //モンスターの種類だけサムネイル画像を取得する
        {
            thumbnailImage.sprite = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[dropNum]).GetThumbnailSprite();

        }

    }
    public void DropCharacter() //Event Triggerで呼ぶ
    {
        //Debug.Log($"{dropNum}にドロップされました");

        GameManager.instance.DropCharacterData(dropNum);

        thumbnailImage.sprite = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[dropNum]).GetThumbnailSprite();

        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNumの場合はそのままIDになる


    }

    public void PointerDownCharacterInCombat()
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInCombat(dropNum);
    }

    public void PointerDown() //EventTriggerで呼ぶ　DraggedCharacterThumbnail用
    {
        //Debug.Log($"{dropNum}がPointerDownされました");

        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().ShowDraggedCharacterThumbnail(GameManager.instance.IDsOfCharactersInCombat[dropNum]); //dragNumの場合はそのままIDになる

    }
    public void PointerUp() //EventTriggerで呼ぶ　DraggedCharacterThumbnail用
    {
        //Debug.Log($"{dropNum}がPointerUpされました");

        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNumの場合はそのままIDになる
    }

}

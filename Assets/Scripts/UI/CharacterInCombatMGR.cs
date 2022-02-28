using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CharacterInCombatMGR : MonoBehaviour
{
    [SerializeField] int dropNum;
    Image thumbnailImage;

    private void Start()
    {
        thumbnailImage = GetComponent<Image>();

        StartCoroutine(LateStart(0.3f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        thumbnailImage.sprite = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[dropNum]).GetThumbnailSprite();
    }


    public void DropCharacter() //Event Triggerで呼ぶ
    {
        //Debug.Log($"{dropNum}にドロップされました");

        GameManager.instance.DropCharacterData(dropNum);

        Sprite draggedSprite = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[dropNum]).GetThumbnailSprite();


        thumbnailImage.sprite = draggedSprite;

        //サムネイルをSelectCharacterにもセットする
        GameManager.instance.selectCharacterButtonMGRs[dropNum].SetSelectCharacterImage(draggedSprite);

        //ドラッグしていたサムネイルを消す
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

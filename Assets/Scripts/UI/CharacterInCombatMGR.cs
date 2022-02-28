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


    public void DropCharacter() //Event Trigger�ŌĂ�
    {
        //Debug.Log($"{dropNum}�Ƀh���b�v����܂���");

        GameManager.instance.DropCharacterData(dropNum);

        Sprite draggedSprite = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[dropNum]).GetThumbnailSprite();


        thumbnailImage.sprite = draggedSprite;

        //�T���l�C����SelectCharacter�ɂ��Z�b�g����
        GameManager.instance.selectCharacterButtonMGRs[dropNum].SetSelectCharacterImage(draggedSprite);

        //�h���b�O���Ă����T���l�C��������
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�


    }

    public void PointerDownCharacterInCombat()
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInCombat(dropNum);
    }

    public void PointerDown() //EventTrigger�ŌĂԁ@DraggedCharacterThumbnail�p
    {
        //Debug.Log($"{dropNum}��PointerDown����܂���");

        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().ShowDraggedCharacterThumbnail(GameManager.instance.IDsOfCharactersInCombat[dropNum]); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�

    }
    public void PointerUp() //EventTrigger�ŌĂԁ@DraggedCharacterThumbnail�p
    {
        //Debug.Log($"{dropNum}��PointerUp����܂���");

        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�
    }

}

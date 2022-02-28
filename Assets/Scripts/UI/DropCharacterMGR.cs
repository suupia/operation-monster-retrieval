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
        if (dropNum < GameManager.instance.characterPrefabs.Length) //�����X�^�[�̎�ނ����T���l�C���摜���擾����
        {
            thumbnailImage.sprite = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[dropNum]).GetThumbnailSprite();

        }

    }
    public void DropCharacter() //Event Trigger�ŌĂ�
    {
        //Debug.Log($"{dropNum}�Ƀh���b�v����܂���");

        GameManager.instance.DropCharacterData(dropNum);

        thumbnailImage.sprite = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[dropNum]).GetThumbnailSprite();

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

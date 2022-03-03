using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterInReserveMGR : MonoBehaviour
{
    [SerializeField] int dragNum; //CharacterInReverse(GameObject)�̓Y�������C���X�y�N�^�[��ŃZ�b�g���Ă���
    [SerializeField] Image characterInReserveImage; //�C���X�y�N�^�[��ŃZ�b�g����

    private void Start()
    {
        if (GameManager.instance.InitializationFlag == false) GameManager.instance.Initialization();

        if (dragNum < GameManager.instance.characterPrefabs.Length) //�����X�^�[�̎�ނ����T���l�C���摜���擾����
        {
            characterInReserveImage.sprite = GameManager.instance.GetCharacterDatabase(dragNum).GetThumbnailSprite();
        }
    }


    public void UpdateCharacterInReserve() //�T���l�C���̐F�����߂�
    {
        //Debug.Log($"UpdateCharacterInReserve�����s���܂�\ndragNum:{dragNum}");
        if (dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum] && dragNum <GameManager.instance.characterInReserveMGRs.Length /*���̎����ł�dragNum��9�܂�(0�`8)*/)
        {

            characterInReserveImage.color = Color.white;
        }
        else
        {
            characterInReserveImage.color = new Color(0.04f, 0.04f, 0.04f, 1);//���ɋ߂��F�ɂ���
        }
    }

    public void DragCharacter() //EventTrigger�ŌĂ�
    {
        //Debug.Log($"{dragNum}���h���b�O����܂���");

        if (!(dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }

        GameManager.instance.DragCharacterData(dragNum);

    }

    public void PointerDownCharacterInReserve() //EventTrigger�ŌĂԁi�Q�Z�b�g���Ă��邱�Ƃɒ��Ӂj
    {
        if (!(dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInReserve(dragNum);
    }

    public void PointerDown() //EventTrigger�ŌĂԁ@DraggedCharacterThumbnail�p
    {
        if (!(dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().ShowDraggedCharacterThumbnail(dragNum); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�

    }
    public void PointerUp() //EventTrigger�ŌĂԁ@DraggedCharacterThumbnail�p
    {
        if (!(dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�
    }

}

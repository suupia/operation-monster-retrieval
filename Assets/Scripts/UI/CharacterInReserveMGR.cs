using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterInReserveMGR : MonoBehaviour
{
    [SerializeField] int dragNum; //CharacterInReverse(GameObject)�̓Y�������C���X�y�N�^�[��ŃZ�b�g���Ă���
    Image characterInReserveImage;

    private void Start()
    {
        characterInReserveImage = this.gameObject.GetComponent<Image>();
        StartCoroutine(LateStart(0.3f)); //characterDatabase��GameManager��Start�ŏ���������邽�߁A�኱�x��Ă���T���l�C���̏�����������
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (dragNum < GameManager.instance.characterPrefabs.Length) //�����X�^�[�̎�ނ����T���l�C���摜���擾����
        {
           characterInReserveImage.sprite = GameManager.instance.GetCharacterDatabase(dragNum).GetThumbnailSprite();
        }

    }

    public void UpdateCharacterInReserve() //�T���l�C���̐F�����߂�
    {
        if (dragNum <= GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum])
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

        if (!(dragNum <= GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }

        GameManager.instance.DragCharacterData(dragNum);

    }

    public void PointerDownCharacterInReserve() //EventTrigger�ŌĂԁi�Q�Z�b�g���Ă��邱�Ƃɒ��Ӂj
    {
        if (!(dragNum <= GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInReserve(dragNum);
    }

    public void PointerDown() //EventTrigger�ŌĂԁ@DraggedCharacterThumbnail�p
    {
        if (!(dragNum <= GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().ShowDraggedCharacterThumbnail(dragNum); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�

    }
    public void PointerUp() //EventTrigger�ŌĂԁ@DraggedCharacterThumbnail�p
    {
        if (!(dragNum <= GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragCharacterMGR : MonoBehaviour
{
    [SerializeField] int dragNum; //CharacterInReverse(GameObject)�̓Y�������C���X�y�N�^�[��ŃZ�b�g���Ă���

    private void Start()
    {
        StartCoroutine(LateStart(0.5f)); //characterDatabase��GameManager��Start�ŏ���������邽�߁A�኱�x��Ă���T���l�C���̏�����������
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (dragNum < GameManager.instance.characterPrefabs.Length) //�����X�^�[�̎�ނ����T���l�C���摜���擾����
        {
            this.gameObject.GetComponent<Image>().sprite = GameManager.instance.GetCharacterDatabase(dragNum).GetThumbnailSprite();
        }

    }
    public void DragCharacter() //EventTrigger�ŌĂ�
    {
        //Debug.Log($"{dragNum}���h���b�O����܂���");

        GameManager.instance.DragCharacterData(dragNum);

    }

    public void PointerDownCharacterInReserve() //EventTrigger�ŌĂԁi�Q�Z�b�g���Ă��邱�Ƃɒ��Ӂj
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInReserve(dragNum);
    }

    public void PointerDown() //EventTrigger�ŌĂԁ@DraggedCharacterThumbnail�p
    {
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().ShowDraggedCharacterThumbnail(dragNum); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�

    }
    public void PointerUp() //EventTrigger�ŌĂԁ@DraggedCharacterThumbnail�p
    {
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNum�̏ꍇ�͂��̂܂�ID�ɂȂ�
    }

}

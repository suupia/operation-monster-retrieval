using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragCharacterMGR : MonoBehaviour
{
    [SerializeField] int dragNum; //CharacterInReverse(GameObject)�̓Y�������C���X�y�N�^�[��ŃZ�b�g���Ă���
    public void DragCharacter() //EventTrigger�ŌĂ�
    {
        Debug.LogWarning($"{dragNum}���h���b�O����܂���");

        GameManager.instance.DragCharacterData(dragNum);
    }

    public void PointerDownCharacterInReserve() //EventTrigger�ŌĂԁi�Q�Z�b�g���Ă��邱�Ƃɒ��Ӂj
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInReserve(dragNum);
    }
}

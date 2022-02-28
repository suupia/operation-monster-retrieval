using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectStageButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //�C���X�y�N�^�[��ŃZ�b�g����

    [SerializeField] ButtonSizeMGR buttonSizeMGR; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] Image selectSatageButtonImage; //�C���X�y�N�^�[��ŃZ�b�g���� �F��ς��邽��

    Color cannotSelectColor; //���ɋ߂��F�ɂ���


    private void Start()
    {
        cannotSelectColor = new Color(0.04f,0.04f,0.04f,1);//���ɋ߂��F�ɂ���

        StartCoroutine(LateStart(0.3f)) ;
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        UpdateSelectStageButtonMGR();

    }

    public void UpdateSelectStageButtonMGR() //StagesClearedNum�̃v���p�e�B�ōX�V����
    {
        Debug.LogWarning($"UpdateSelectStageButtonMGR���J�n���܂�\nStagesClearedNum +1:{GameManager.instance.StagesClearedNum + 1}");
        if (buttonNum <= GameManager.instance.StagesClearedNum +1) //�i�N���A�X�e�[�W�̓Y�����j+1�@�̃X�e�[�W�܂őI���ł���
        {
            buttonSizeMGR.SetIsActive(true);
            selectSatageButtonImage.color = Color.white;
            //Debug.Log("buttonSizeMGR��true�ɂ��܂���");
        }
        else
        {
            buttonSizeMGR.SetIsActive(false);
            selectSatageButtonImage.color = cannotSelectColor;
            //Debug.Log("buttonSizeMGR��false�ɂ��܂���");
        }
    }

    public void PointerDown() //EventTrigger�ŌĂ�
    {
        Debug.Log($"SelectStageButton{buttonNum}��������܂���");

        if (buttonNum <= GameManager.instance.StagesClearedNum +1)
        {
            GameManager.instance.mapMGR.SetStageNum(buttonNum);
            GameManager.instance.SetupGame();
        }
        else
        {
            //�������Ȃ�
            //�����Ȃ����ʉ��Ƃ����邩������Ȃ�
        }
    }


}

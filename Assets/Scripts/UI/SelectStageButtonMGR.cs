using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectStageButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //�C���X�y�N�^�[��ŃZ�b�g����

    private Button button;


    private void Start()
    {
        button = gameObject.GetComponent<Button>();
    }

    public void PointerDown()
    {
        Debug.Log($"SelectStageButton{buttonNum}��������܂���");



        GameManager.instance.mapMGR.SetStageNum(buttonNum);
        GameManager.instance.SetupGame();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //�C���X�y�N�^�[��ŃZ�b�g����

    public void OnClick()
    {
        Debug.Log($"Button{buttonNum}��������܂���");

        GameManager.instance.SpawnCharacter(buttonNum);
    }
}

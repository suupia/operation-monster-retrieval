using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //�C���X�y�N�^�[��ŃZ�b�g����

    Text modeSwitchButtonText;

    //�ȉ�modeSwitchButton�̕ϐ�
    CharacterMGR.Mode mode = CharacterMGR.Mode.Auto; //�f�t�H���g��AutoMode

    private void Start()
    {
        modeSwitchButtonText = transform.Find("ModeSwitchButton/Text").gameObject.GetComponent<Text>(); //���I�u�W�F�N�g����Text�R���|�[�l���g���擾����
        modeSwitchButtonText.text = "Auto Mode"; //�f�t�H���g��AutoMode
    }

    public void OnClick()
    {
        Debug.Log($"Button{buttonNum}��������܂���");

        GameManager.instance.SpawnCharacter(buttonNum);
    }

    public void SwitchMode()
    {
        Debug.Log($"Button{buttonNum}��modeSwitchButton��������܂���");
        switch (mode)
        {
            case CharacterMGR.Mode.Auto:
                mode = CharacterMGR.Mode.Manual;
                modeSwitchButtonText.text = "Manual Mode";
                GameManager.instance.SetCharacterMode(buttonNum, CharacterMGR.Mode.Manual);
                break;
            case CharacterMGR.Mode.Manual:
                mode = CharacterMGR.Mode.Auto;
                modeSwitchButtonText.text = "Auto Mode";
                GameManager.instance.SetCharacterMode(buttonNum, CharacterMGR.Mode.Auto);
                break;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //�C���X�y�N�^�[��ŃZ�b�g����

    Text modeSwitchButtonText;

    //�ȉ�modeSwitchButton�̕ϐ�
    CharacterMGR.Mode mode = CharacterMGR.Mode.Auto; //�f�t�H���g��AutoMode

    //�ȉ��AManualRoute�p�̕ϐ�
    private Button button;
    [SerializeField] ColorBlock colorBlock;

    private void Start()
    {
        modeSwitchButtonText = transform.Find("ModeSwitchButton/Text").gameObject.GetComponent<Text>(); //���I�u�W�F�N�g����Text�R���|�[�l���g���擾����
        modeSwitchButtonText.text = "Auto Mode"; //�f�t�H���g��AutoMode
        button = gameObject.GetComponent<Button>();
    }

    public void PointerDown()
    {
        Debug.Log($"SelectCharacterButton{buttonNum}��������܂���");

        if (Input.GetMouseButton(0))
        {
            GameManager.instance.SpawnCharacter(buttonNum);
        }
        else if (Input.GetMouseButton(1))
        {
            if (button.colors != colorBlock) //����button���I������Ă��Ȃ���ԂŁA����button���N���b�N���ꂽ�Ƃ�
            {
                SetToSelectedColor();
            }
            else //����button���I������Ă����ԂŁA����button���N���b�N���ꂽ�Ƃ�
            {
                ResetToNormalColor();
                GameManager.instance.inputMGR.SetManualRouteNumber(-1);
            }
        }
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

    private void SetToSelectedColor()
    {
        if (GameManager.instance.inputMGR.GetSelectedButtonMGR() != null) //����button��I�����Ă����Ƃ�
        {
            GameManager.instance.inputMGR.GetSelectedButtonMGR().ResetToNormalColor(); //����button�̐F��߂�
        }
        button.colors = colorBlock;
        GameManager.instance.inputMGR.SetManualRouteNumber(buttonNum);
        GameManager.instance.inputMGR.SetSelectedButtonMGR(this);
        Debug.LogWarning($"ManualRoute��I������L�����N�^�[�����肵�܂��� number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void ResetToNormalColor()
    {
        button.colors = ColorBlock.defaultColorBlock;
        Debug.LogWarning($"ManualRoute�̃L�����N�^�\�I�����������܂��� number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }
}


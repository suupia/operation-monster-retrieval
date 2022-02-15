using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //�C���X�y�N�^�[��ŃZ�b�g����

    [SerializeField] Image buttonImage; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] Image modeSwitchButtonImage;
    [SerializeField] Text modeSwitchButtonText;
    [SerializeField] Text costText;

    Color canSpawnCharacterColor = Color.white;
    Color canNotSpawnCharacterColor = Color.gray;

    //�ȉ��AmodeSwitchButton�̕ϐ�
    CharacterMGR.Mode mode = CharacterMGR.Mode.Auto; //�f�t�H���g��AutoMode

    //�ȉ��AManualRoute�p�̕ϐ�
   [SerializeField] bool isEditingManualRoute;
    Color selectedColor = Color.cyan;
    Color notSelectedColor = Color.white;

    //�ȉ��ACoolTime�p�̕ϐ�
    [SerializeField] Image emptyGauge;
    [SerializeField] Image filledGauge;
    [SerializeField] Color emptyGaugeColor;
    [SerializeField] Color filledGaugeColor;

    public void InitiSelectCharacterButton() //GameManager��State.PlayingGame�ɂȂ����Ƃ��ɌĂ�
    {
        modeSwitchButtonText.text = "Auto"; //�f�t�H���g��AutoMode

        isEditingManualRoute = false;

        emptyGauge.color = Color.clear;
        filledGauge.color = Color.clear;

        costText.text = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[buttonNum]).GetCost() + "ene";
    }
    void Update()
    {

        if (GameManager.instance.energyMGR.CurrentEnergy >= GameManager.instance.GetCharacterMGRFromButtonNum(buttonNum).GetCost())
        {
            if (isEditingManualRoute)
            {
                buttonImage.color = canSpawnCharacterColor * selectedColor; //��Z�ŏ�������

            }
            else
            {
                buttonImage.color = canSpawnCharacterColor * notSelectedColor;

            }
        }
        else
        {
            if (isEditingManualRoute)
            {
                buttonImage.color = canNotSpawnCharacterColor * selectedColor;

            }
            else
            {
                buttonImage.color = canNotSpawnCharacterColor * notSelectedColor;

            }
        }
        if(GameManager.instance.state == GameManager.State.ShowingResults && isEditingManualRoute)
        {
            ResetToNormalColor();
        }
    }
    public void PointerDown() //EventTrigger�ŌĂ�
    {
        if (GameManager.instance.state != GameManager.State.RunningGame) return;

        Debug.Log($"SelectCharacterButton{buttonNum}��������܂���");

        if (Input.GetMouseButton(0))
        {
            GameManager.instance.SpawnCharacter(buttonNum);
        }
        else if (Input.GetMouseButton(1))
        {
            if (!isEditingManualRoute) //����button���I������Ă��Ȃ���ԂŁA����button���N���b�N���ꂽ�Ƃ�
            {
                Debug.LogWarning("SetToSelectedColor�����s���܂�");
                SetToSelectedColor();
            }
            else //����button���I������Ă����ԂŁA����button���N���b�N���ꂽ�Ƃ�
            {
                Debug.LogWarning("ResetToNormalColor�����s���܂�");

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
                modeSwitchButtonText.text = "Manual";
                GameManager.instance.SetCharacterMode(buttonNum, CharacterMGR.Mode.Manual);
                break;
            case CharacterMGR.Mode.Manual:
                mode = CharacterMGR.Mode.Auto;
                modeSwitchButtonText.text = "Auto";
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
        isEditingManualRoute = true;
        buttonImage.color = selectedColor;
        Debug.LogWarning($"buttonImage.color��{selectedColor}�ɕύX���܂���");
        GameManager.instance.inputMGR.SetManualRouteNumber(buttonNum);
        GameManager.instance.inputMGR.SetSelectedButtonMGR(this);
        Debug.LogWarning($"ManualRoute��I������L�����N�^�[�����肵�܂��� number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void ResetToNormalColor()
    {
        isEditingManualRoute = false;
        buttonImage.color = notSelectedColor;
        Debug.LogWarning($"ManualRoute�̃L�����N�^�\�I�����������܂��� number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void RefreshGauge(float percentage)
    {
        filledGauge.fillAmount = percentage;
        if (percentage >= 1)
        {
            emptyGauge.color = Color.clear;
            filledGauge.color = Color.clear;
        }
        else
        {
            emptyGauge.color = emptyGaugeColor;
            filledGauge.color = filledGaugeColor;
        }
    }
}


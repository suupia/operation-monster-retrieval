using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //�C���X�y�N�^�[��ŃZ�b�g����

    [SerializeField] Image selectCharacterImage; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] GameObject changeColorMask; //�F���܂������ڂ��Ȃ����ɂ͂���GameObject��false�ɂ���
    [SerializeField] Image changeColorImage; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] Image modeSwitchButtonImage;
    [SerializeField] Text modeSwitchButtonText;
    [SerializeField] Text costText;

    Color canSpawnCharacterColor;
    Color canNotSpawnCharacterColor;

    bool spawningCharacter;
    //�ȉ��AmodeSwitchButton�̕ϐ�
    CharacterMGR.Mode mode = CharacterMGR.Mode.Auto; //�f�t�H���g��AutoMode

    //�ȉ��AManualRoute�p�̕ϐ�
   [SerializeField] bool isEditingManualRoute;
    Color selectedColor;
    Color notSelectedColor;

    //�ȉ��ACoolTime�p�̕ϐ�
    [SerializeField] Image emptyGauge;
    [SerializeField] Image filledGauge;
    [SerializeField] Color emptyGaugeColor;
    [SerializeField] Color filledGaugeColor;

    public void InitiSelectCharacterButton() //GameManager��State.PlayingGame�ɂȂ����Ƃ��ɌĂ�
    {
        modeSwitchButtonText.text = "Auto"; //�f�t�H���g��AutoMode

        isEditingManualRoute = false;

        //�F�̏�����
        canSpawnCharacterColor = Color.white;
        canNotSpawnCharacterColor = new Color(0.4f,0.4f,0.4f,1); //�኱�Z���O���[
        selectedColor = Color.cyan;
        notSelectedColor = Color.gray;
        emptyGauge.color = Color.clear;
        filledGauge.color = Color.clear;

        //�摜�̏�����
        SetSelectCharacterImage(GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[buttonNum]).GetThumbnailSprite());

        costText.text = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[buttonNum]).GetCost() + "ene";
    }

    //Seeter
    public void SetSelectCharacterImage(Sprite sprite)
    {
        selectCharacterImage.sprite = sprite;
    }

    void Update()
    {
        Color finalColor;
        if (GameManager.instance.energyMGR.CurrentEnergy >= GameManager.instance.GetCharacterMGRFromButtonNum(buttonNum).GetCost() && !spawningCharacter)
        {
            if (isEditingManualRoute)
            {
                finalColor = canSpawnCharacterColor * selectedColor; //��Z�ŏ�������
                Debug.LogWarning($"finalColor:{finalColor}");


            }
            else
            {
                finalColor = Color.clear; //���̎������ʂŁA������������
                Debug.LogWarning($"finalColor:{finalColor}");
            }
        }
        else
        {
            if (isEditingManualRoute)
            {
                finalColor = canNotSpawnCharacterColor * selectedColor;
                Debug.LogWarning($"finalColor:{finalColor}");


            }
            else
            {
                finalColor = canNotSpawnCharacterColor * notSelectedColor;
                Debug.LogWarning($"canNotSpawnCharacterColor:{canNotSpawnCharacterColor}, notSelectedColor:{notSelectedColor}");
                Debug.LogWarning($"finalColor:{finalColor}");


            }
        }

        finalColor.a *= 0.5f;�@//�����܂ł�if����finalColor�͕s�����F�ɂȂ��Ă���̂ŁA�����x��50���ɂ���


        Debug.LogWarning($"finalColor:{finalColor}");

        if (finalColor != Color.clear)
        {
            changeColorMask.SetActive(true);
            changeColorImage.color = finalColor;
        }
        else
        {
            changeColorMask.SetActive(false);
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
            }
        }

    }

    public void SwitchMode() //ModeSwitchButton��EventTrigger�ŌĂ�
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
        changeColorImage.color = selectedColor;
        Debug.LogWarning($"changeColorImage.color:{changeColorImage.color}");

        Debug.LogWarning($"buttonImage.color��{selectedColor}�ɕύX���܂���");
        GameManager.instance.inputMGR.SetManualRouteNumber(buttonNum);
        GameManager.instance.inputMGR.SetSelectedButtonMGR(this);
        Debug.LogWarning($"ManualRoute��I������L�����N�^�[�����肵�܂��� number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void ResetToNormalColor()
    {
        isEditingManualRoute = false;
        changeColorImage.color = notSelectedColor;
        Debug.LogWarning($"changeColorImage.color:{changeColorImage.color}");

        Debug.LogWarning($"ManualRoute�̃L�����N�^�\�I�����������܂��� number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
        GameManager.instance.inputMGR.SetManualRouteNumber(-1);       //InputMGR�ł�ManualRoute�̑I�����O��
    }

    public void RefreshGauge(float percentage)
    {
        filledGauge.fillAmount = percentage;
        if (percentage >= 1)
        {
            spawningCharacter = false;
            emptyGauge.color = Color.clear;
            filledGauge.color = Color.clear;
        }
        else
        {
            spawningCharacter = true;
            emptyGauge.color = emptyGaugeColor;
            filledGauge.color = filledGaugeColor;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //�C���X�y�N�^�[��ŃZ�b�g����

    [SerializeField] ButtonSizeMGR buttonSizeMGR; //�C���X�y�N�^�[��ŃZ�b�g����

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
    [SerializeField] Color manualModeColor;
    [SerializeField] Color autoModeColor;

    //�ȉ��AManualRoute�p�̕ϐ�
    [SerializeField] bool isEditingManualRoute;
    [SerializeField] Color selectedColor;
    Color notSelectedColor;
    [SerializeField] GameObject skillCanvas;
    [SerializeField] Image skillTriggerRouteImage;

    //�ȉ��ACoolTime�p�̕ϐ�
    [SerializeField] Image emptyGauge;
    [SerializeField] Image filledGauge;
    [SerializeField] Color emptyGaugeColor;
    [SerializeField] Color filledGaugeColor;


    public void InitiSelectCharacterButton() //GameManager��State.PlayingGame�ɂȂ����Ƃ��ɌĂ�
    {
        modeSwitchButtonText.text = "Auto"; //�f�t�H���g��AutoMode
        modeSwitchButtonImage.color = autoModeColor;

        isEditingManualRoute = false;

        //�F�̏�����
        canSpawnCharacterColor = Color.white;
        canNotSpawnCharacterColor = new Color(0.4f,0.4f,0.4f,1); //�኱�Z���O���[
        //selectedColor = Color.cyan;    �F�����̂��߂�Inspector��ŕύX����
        notSelectedColor = Color.gray;
        emptyGauge.color = Color.clear;
        filledGauge.color = Color.clear;

        skillTriggerRouteImage.sprite = GameManager.instance.characterSkillsDataMGR.skillTriggerImages[GameManager.instance.GetCharacterMGRFromButtonNum(buttonNum).GetSkillNum()] ;
        ResetToNormalColor();

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
        //if (GameManager.instance.energyMGR.CurrentEnergy >= GameManager.instance.GetCharacterMGRFromButtonNum(buttonNum).GetCost())
        //{
        //    buttonSizeMGR.SetIsActive(true);
        //}
        //else
        //{
        //    buttonSizeMGR.SetIsActive(false);
        //}


        Color finalColor;
        if (GameManager.instance.energyMGR.CurrentEnergy >= GameManager.instance.GetCharacterMGRFromButtonNum(buttonNum).GetCost() && !spawningCharacter && GameManager.instance.CurrentCharacterNum <GameManager.instance.MaxCharacterNum)
        {
            buttonSizeMGR.SetIsActive(true);

            if (isEditingManualRoute)
            {
                finalColor = canSpawnCharacterColor * selectedColor; //��Z�ŏ�������
                //Debug.Log($"finalColor:{finalColor}");
            }
            else
            {
                finalColor = Color.clear; //���̎������ʂŁA������������
                //Debug.Log($"finalColor:{finalColor}");
            }
        }
        else
        {
            buttonSizeMGR.SetIsActive(false);

            if (isEditingManualRoute)
            {
                finalColor = canNotSpawnCharacterColor * selectedColor;
                //Debug.Log($"finalColor:{finalColor}");
            }
            else
            {
                finalColor = canNotSpawnCharacterColor * notSelectedColor;
                //Debug.Log($"canNotSpawnCharacterColor:{canNotSpawnCharacterColor}, notSelectedColor:{notSelectedColor}");
            }
        }

        finalColor.a *= 0.5f;�@//�����܂ł�if����finalColor�͕s�����F�ɂȂ��Ă���̂ŁA�����x��50���ɂ���


        //Debug.Log($"finalColor:{finalColor}");

        if (finalColor != Color.clear)
        {
            changeColorMask.SetActive(true);
            changeColorImage.color = finalColor;
        }
        else
        {
            changeColorMask.SetActive(false);
        }

        if (GameManager.instance.state == GameManager.State.ShowingResults && isEditingManualRoute) //ShowingResults����PointerTail������
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
                //Debug.Log("SetToSelectedColor�����s���܂�");
                SetToSelectedColor();
                StartCopyingManualRoute();
            }
            else //����button���I������Ă����ԂŁA����button���N���b�N���ꂽ�Ƃ�
            {
                //Debug.Log("ResetToNormalColor�����s���܂�");

                ResetToNormalColor();
            }
        }

    }

    public void StartCopyingManualRoute() //PointerDown�ŌĂ�
    {
        if (!Input.GetMouseButtonDown(1)) return; //�E�N���b�N�̂Ƃ������������s��

        Debug.LogWarning($"ManualRoute�̃R�s�[���J�n���܂��� buttonNum={buttonNum}");
        GameManager.instance.copyingSelectCharacterButtonNum = buttonNum; //Drop����Button�ɂ���buttonNum��n���K�v������̂ŁAGameManager���o�R����
        GameManager.instance.copyingManualRoute = true;
        StartCoroutine(LateStartCoyingManualRoute());
    }

    private IEnumerator LateStartCoyingManualRoute() //InputMGR�Ƃ̌��ˍ����ŁA��t���[���x�点��
    {
        yield return null;
        GameManager.instance.copyingManualRoute = true;
        GameManager.instance.copyingSelectCharacterButtonNum = buttonNum;
        GameManager.instance.manualRouteDatas[buttonNum].ShowSelectedManualRoute(); //����Button�ɑΉ�����ManualRoute��PointerTail�ŕ\�����鏈��������
        GameManager.instance.curveToMouseMGR.SetCircles(buttonNum);
        GameManager.instance.curveToMouseMGR.StartIlluminatingPointersCroutine();
    }

    public void ShowLineBetweenButtonAndPointer() //EventTrigger Drag�ŌĂ�
    {
        if (!GameManager.instance.copyingManualRoute) return;

        GameManager.instance.curveToMouseMGR.SetCircles(buttonNum);
    }

    
    public void PasteManualRoute()
    {
        if (GameManager.instance.copyingSelectCharacterButtonNum == buttonNum) return;
        if(GameManager.instance.copyingSelectCharacterButtonNum < 0 || GameManager.instance.copyingSelectCharacterButtonNum > 3) //IndexOutOfRange
        {
            return;
        }
        //ManualRoute���R�s�[
        GameManager.instance.manualRouteDatas[buttonNum].SetManualRoute(GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetManualRoute());
        GameManager.instance.manualRouteDatas[buttonNum].SetNonDiagonalManualRoute(GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetNonDiagonalManualRoute());
        GameManager.instance.manualRouteDatas[buttonNum].SetNonDiagonalPoints(GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetNonDiagonalPoints());
        Debug.LogWarning($"ManualRoute��{GameManager.instance.copyingSelectCharacterButtonNum}����{buttonNum}�փR�s�[���܂���\nManualRoute[{buttonNum}]={string.Join(",", GameManager.instance.manualRouteDatas[buttonNum].GetManualRoute())}");

        //Paste���ɃR�s�[���̑I�����O��
        GameManager.instance.selectCharacterButtonMGRs[GameManager.instance.copyingSelectCharacterButtonNum].ResetToNormalColor();
        //pointerTail������
        GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].DestroyPointerTails();
        //CurvePointer������
        GameManager.instance.curveToMouseMGR.DestroyLineBetweenButtonAndPointer();
    }

    public void PointerEnter()
    {
        GameManager.instance.mouseEnteredSelectCharacterButtonNum = buttonNum;
    }

    public void PointerExit()
    {
        GameManager.instance.mouseEnteredSelectCharacterButtonNum = -1;
    }


    public void SwitchMode() //ModeSwitchButton��EventTrigger�ŌĂ�
    {
        Debug.Log($"Button{buttonNum}��modeSwitchButton��������܂���");
        switch (mode)
        {
            case CharacterMGR.Mode.Auto:
                mode = CharacterMGR.Mode.Manual;
                modeSwitchButtonText.text = "Manual";
                modeSwitchButtonImage.color = manualModeColor;
                GameManager.instance.SetCharacterMode(buttonNum, CharacterMGR.Mode.Manual);
                break;
            case CharacterMGR.Mode.Manual:
                mode = CharacterMGR.Mode.Auto;
                modeSwitchButtonText.text = "Auto";
                modeSwitchButtonImage.color = autoModeColor;
                GameManager.instance.SetCharacterMode(buttonNum, CharacterMGR.Mode.Auto);
                break;
        }

    }

    private void SetToSelectedColor()
    {
        if (GameManager.instance.inputMGR.GetSelectedButtonMGR() != null) //����button��I�����Ă����Ƃ�
        {
            GameManager.instance.inputMGR.GetSelectedButtonMGR().ResetToNormalColor(); //����button�̐F��߂�

            GameManager.instance.inputMGR.GetSelectedButtonMGR().CloseSkillCanvas(); //����button��SkillCanvas�����
        }
        OpenSkillCanvas(); //SkillCanvas���J��(�F�̕ύX�Ɠ����Ȃ̂ł����ɏ���)
        isEditingManualRoute = true;
        changeColorImage.color = selectedColor;
        //Debug.Log($"changeColorImage.color:{changeColorImage.color}");

        //Debug.Log($"buttonImage.color��{selectedColor}�ɕύX���܂���");
        GameManager.instance.inputMGR.SetManualRouteNumber(buttonNum);
        GameManager.instance.inputMGR.SetSelectedButtonMGR(this);
        Debug.Log($"ManualRoute��I������L�����N�^�[�����肵�܂��� number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void ResetToNormalColor()
    {
        CloseSkillCanvas(); //SkillCanvas�����
        isEditingManualRoute = false;
        changeColorImage.color = notSelectedColor;
        //Debug.Log($"changeColorImage.color:{changeColorImage.color}");

        Debug.Log($"ManualRoute�̃L�����N�^�\�I�����������܂��� number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
        GameManager.instance.inputMGR.SetManualRouteNumber(-1);       //InputMGR�ł�ManualRoute�̑I�����O��
        GameManager.instance.inputMGR.isEditingManualRoute = false; 
    }

    private void OpenSkillCanvas() //ManualMode�ҏW���ɂ��̃L�����N�^�[�̃X�L����\������
    {
        skillCanvas.SetActive(true);
    }

    private void CloseSkillCanvas()
    {
        skillCanvas.SetActive(false);
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


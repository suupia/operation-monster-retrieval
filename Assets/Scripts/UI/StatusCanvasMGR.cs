using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusCanvasMGR : MonoBehaviour
{
    CharacterMGR characterMGRDisplayed; //UpdateStatusCanvas()�ōX�V����
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;
    [SerializeField] Text atkText;
    [SerializeField] Text attackIntervalText;
    [SerializeField] Text attackRangeText;
    [SerializeField] Text spdText;
    [SerializeField] Text coolTimeText;
    [SerializeField] Text costText;
    [SerializeField] Image characterImage;

    [SerializeField] Text levelUpCostText; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] int[] levelUpCostArray; //���x���A�b�v�ɕK�v�Ȍo���l���C���X�y�N�^�[��Ō��߂�

    [SerializeField] Text EXPText;
    int _EXPretained; //�ێ����Ă���o���l �v���p�e�B��setter��EXPText���X�V����

    public int EXPretained //�v���p�e�B
    {
        get{ return _EXPretained; }
        set{ 
            _EXPretained = value;
            EXPText.text = $"EXP:{_EXPretained,8:D}"; //alignment�Ƃ���8���w�肵�Ă���i�E�����ŏ���D��8�������B�������A�����̕��Ɠ����Ƃ͌���Ȃ��݂����j

        }
    }

    public CharacterMGR GetCharacterMGRDisplayed()
    {
        return characterMGRDisplayed;
    }

    private void Start()
    {
        //Debug.LogWarning("StatusCanbasMGR��Start()�����s���܂�");
        UpdateStatusCanvas(0); //�ŏ���ID:0��Bat��\�������Ă���


        //�f�o�b�O�p
        EXPretained += 10000; //���x���A�b�v�̎��������₷���悤�ɍŏ�����o���l��^���Ă���
    }
    public void UpdateStatusCanvasInCombat(int dropNum)
    {
        UpdateStatusCanvas(GameManager.instance.IDsOfCharactersInCombat[dropNum]);
    }

    public void UpdateStatusCanvasInReserve(int dragNum)
    {
        UpdateStatusCanvas(dragNum);
    }

    void UpdateStatusCanvas(int characterTypeID)
    {
        Debug.LogWarning("StatusCanvas���X�V���܂�");
        characterMGRDisplayed = GameManager.instance.GetCharacterDatabase(characterTypeID);

        nameText.text = characterMGRDisplayed.GetCharacterName();
        levelText.text = " LV :"+(characterMGRDisplayed.GetLevel()+1).ToString();
        costText.text = "Cost:" + characterMGRDisplayed.GetCost().ToString();
        hpText.text ="  HP  :"+ characterMGRDisplayed.GetMaxHp().ToString();
        atkText.text = " ATK :"+characterMGRDisplayed.GetAtk().ToString();
        spdText.text = " SPD :" + characterMGRDisplayed.GetSpd().ToString();
        attackIntervalText.text = "ATK Interval:"+characterMGRDisplayed.GetAttackInterval().ToString();
        attackRangeText.text = " ATK Range :"+characterMGRDisplayed.GetAttackRange().ToString();
        coolTimeText.text = "Cool Time:"+characterMGRDisplayed.GetCoolTime().ToString();

        if (characterMGRDisplayed.GetLevel()< characterMGRDisplayed.GetMaxLevel())
        {
            levelUpCostText.text = $"���̃��x���܂�\nEXP{levelUpCostArray[characterMGRDisplayed.GetLevel()]}";
        }
        else
        {
            levelUpCostText.text = $"�ő僌�x���ł�";
        }

        if (characterMGRDisplayed.GetSprite() != null)
        {
            characterImage.sprite = characterMGRDisplayed.GetSprite();
        }
    }

    public void LevelUpCharacterDisplayed()
    {
        characterMGRDisplayed.LevelUp();

        UpdateStatusCanvas(characterMGRDisplayed.GetCharacterTypeID());
    }
}

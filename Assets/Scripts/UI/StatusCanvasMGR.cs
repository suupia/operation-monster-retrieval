using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusCanvasMGR : MonoBehaviour
{
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

    [SerializeField] Text EXPText;
    int _EXPretained; //�ێ����Ă���o���l �v���p�e�B��setter��EXPText���X�V����

    public int EXPretained
    {
        get{ return _EXPretained; }
        set{ 
            _EXPretained = value;
            UpdateEXPText();
        }
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
        nameText.text = GameManager.instance.GetCharacterDatabase(characterTypeID).GetCharacterName();
        levelText.text = " LV :"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetLevel().ToString();
        costText.text = "Cost:" + GameManager.instance.GetCharacterDatabase(characterTypeID).GetCost().ToString();
        hpText.text ="  HP  :"+ GameManager.instance.GetCharacterDatabase(characterTypeID).GetMaxHp().ToString();
        atkText.text = " ATK :"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetAtk().ToString();
        spdText.text = " SPD :" + GameManager.instance.GetCharacterDatabase(characterTypeID).GetSpd().ToString();
        attackIntervalText.text = "ATK Interval:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetAttackInterval().ToString();
        attackRangeText.text = " ATK Range :"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetAttackRange().ToString();
        coolTimeText.text = "Cool Time:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetCoolTime().ToString();

        if (GameManager.instance.GetCharacterDatabase(characterTypeID).GetSprite() != null)
        {
            characterImage.sprite = GameManager.instance.GetCharacterDatabase(characterTypeID).GetSprite();

        }
    }

    void UpdateEXPText()
    {
        EXPText.text = $"EXP:{EXPretained,8:D}"; //alignment�Ƃ���8���w�肵�Ă���i�E�����ŏ���D��8�������B�������A�����̕��Ɠ����Ƃ͌���Ȃ��݂����j
    }

}

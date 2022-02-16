using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusCanvasMGR : MonoBehaviour
{
    CharacterMGR characterMGRDisplayed; //UpdateStatusCanvas()で更新する
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

    [SerializeField] Text levelUpCostText; //インスペクター上でセットする
    [SerializeField] int[] levelUpCostArray; //レベルアップに必要な経験値をインスペクター上で決める

    [SerializeField] Text EXPText;
    int _EXPretained; //保持している経験値 プロパティのsetterでEXPTextを更新する

    public int EXPretained //プロパティ
    {
        get{ return _EXPretained; }
        set{ 
            _EXPretained = value;
            EXPText.text = $"EXP:{_EXPretained,8:D}"; //alignmentとして8を指定している（右揃えで書式Dの8文字分。ただし、数字の幅と同じとは限らないみたい）

        }
    }

    public CharacterMGR GetCharacterMGRDisplayed()
    {
        return characterMGRDisplayed;
    }

    private void Start()
    {
        //Debug.LogWarning("StatusCanbasMGRのStart()を実行します");
        UpdateStatusCanvas(0); //最初はID:0のBatを表示させておく


        //デバッグ用
        EXPretained += 10000; //レベルアップの実装をしやすいように最初から経験値を与えておく
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
        Debug.LogWarning("StatusCanvasを更新します");
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
            levelUpCostText.text = $"次のレベルまで\nEXP{levelUpCostArray[characterMGRDisplayed.GetLevel()]}";
        }
        else
        {
            levelUpCostText.text = $"最大レベルです";
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

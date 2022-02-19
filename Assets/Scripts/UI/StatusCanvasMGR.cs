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
    [SerializeField] Image characterFullScreenImage;

    [SerializeField] Text levelUpCostText; //インスペクター上でセットする
    [SerializeField] int[] levelUpCostArray; //レベルアップに必要な経験値をインスペクター上で決める

    [SerializeField] GameObject fullScreenCanvas; //インスペクタ上でセットする
    bool isDisplayingFullScreen = false;

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
        attackIntervalText.text = "ATK Interval:"+characterMGRDisplayed.GetAtkInterval().ToString();
        attackRangeText.text = " ATK Range :"+characterMGRDisplayed.GetAtkRange().ToString();
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
        //Debug.LogWarning($"characterMGRDisplayed.GetLevel():{characterMGRDisplayed.GetLevel()}");

        if (characterMGRDisplayed.GetLevel() == characterMGRDisplayed.GetMaxLevel())
        {
            //Debug.LogWarning("最大レベルなので処理を中断します");
            return;
        }

        if (EXPretained >= levelUpCostArray[characterMGRDisplayed.GetLevel()])
        {
            EXPretained -= levelUpCostArray[characterMGRDisplayed.GetLevel()];
            characterMGRDisplayed.LevelUp();
            UpdateStatusCanvas(characterMGRDisplayed.GetCharacterTypeID());

        }
        else
        {
            //Debug.LogWarning($"EXPが足りないためレベルアップできません");
            levelUpCostText.text = $"EXPが足りないため\nレベルアップできません";
            Invoke("UpdateStatusCanvas",1); //levelUpCostTextを更新するため

        }

    }

    public void SwitchFullScreen() //OpenであってもCloseであってもどちらもこの関数で対応できる
    {
        if (isDisplayingFullScreen)
        {
            Debug.LogWarning("全画面スクリーンを閉じます");
            isDisplayingFullScreen = false;

            fullScreenCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("全画面スクリーンを開きます");
            isDisplayingFullScreen = true;

            characterFullScreenImage.sprite = characterMGRDisplayed.GetSprite();
            fullScreenCanvas.SetActive(true);
        }
    }
}

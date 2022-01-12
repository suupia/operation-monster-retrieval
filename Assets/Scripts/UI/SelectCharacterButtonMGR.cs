using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //インスペクター上でセットする

    [SerializeField] Image buttonImage; //インスペクター上でセットする
    [SerializeField] Image modeSwitchButtonImage;
    [SerializeField] Text modeSwitchButtonText;

    Color canSpawnCharacterColor = Color.white;
    Color canNotSpawnCharacterColor = Color.gray;

    //以下、modeSwitchButtonの変数
    CharacterMGR.Mode mode = CharacterMGR.Mode.Auto; //デフォルトはAutoMode

    //以下、ManualRoute用の変数
    Color selectedColor = Color.cyan;
    Color notSelectedColor = Color.white;

    //以下、CoolTime用の変数
    [SerializeField] Image emptyGauge;
    [SerializeField] Image filledGauge;
    [SerializeField] Color emptyGaugeColor;
    [SerializeField] Color filledGaugeColor;

    public void InitiSelectCharacterButton() //GameManagerがState.PlayingGameになったときに呼ぶ
    {
        modeSwitchButtonText.text = "Auto Mode"; //デフォルトはAutoMode

        emptyGauge.color = Color.clear;
        filledGauge.color = Color.clear;
    }
    void Update()
    {
        if(GameManager.instance.energyMGR.CurrentEnergy >=  GameManager.instance.GetCharacterMGRFromButtonNum(buttonNum).GetCost())
        {
            buttonImage.color = canSpawnCharacterColor;
            //modeSwitchButtonImage.color = canSpawnCharacterColor;
        }
        else
        {
            buttonImage.color = canNotSpawnCharacterColor;
            //modeSwitchButtonImage.color = canNotSpawnCharacterColor;

        }

    }
    public void PointerDown() //EventTriggerで呼ぶ
    {
        if (GameManager.instance.state != GameManager.State.PlayingGame) return;

        Debug.Log($"SelectCharacterButton{buttonNum}が押されました");

        if (Input.GetMouseButton(0))
        {
            GameManager.instance.SpawnCharacter(buttonNum);
        }
        else if (Input.GetMouseButton(1))
        {
            if (buttonImage.color != selectedColor) //このbuttonが選択されていない状態で、このbuttonがクリックされたとき
            {
                SetToSelectedColor();
            }
            else //このbuttonが選択されている状態で、このbuttonがクリックされたとき
            {
                ResetToNormalColor();
                GameManager.instance.inputMGR.SetManualRouteNumber(-1);
            }
        }
    }

    public void SwitchMode()
    {
        Debug.Log($"Button{buttonNum}のmodeSwitchButtonが押されました");
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
        if (GameManager.instance.inputMGR.GetSelectedButtonMGR() != null) //他のbuttonを選択していたとき
        {
            GameManager.instance.inputMGR.GetSelectedButtonMGR().ResetToNormalColor(); //そのbuttonの色を戻す
        }
        buttonImage.color = selectedColor;
        GameManager.instance.inputMGR.SetManualRouteNumber(buttonNum);
        GameManager.instance.inputMGR.SetSelectedButtonMGR(this);
        Debug.LogWarning($"ManualRouteを選択するキャラクターを決定しました number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void ResetToNormalColor()
    {
        buttonImage.color = notSelectedColor;
        Debug.LogWarning($"ManualRouteのキャラクタ―選択を解除しました number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
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


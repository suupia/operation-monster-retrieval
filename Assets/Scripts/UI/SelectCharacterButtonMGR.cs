using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //インスペクター上でセットする

    [SerializeField] Image selectCharacterImage; //インスペクター上でセットする
    [SerializeField] GameObject changeColorMask; //色をまったく載せない時にはこのGameObjectをfalseにする
    [SerializeField] Image changeColorImage; //インスペクター上でセットする
    [SerializeField] Image modeSwitchButtonImage;
    [SerializeField] Text modeSwitchButtonText;
    [SerializeField] Text costText;

    Color canSpawnCharacterColor;
    Color canNotSpawnCharacterColor;

    bool spawningCharacter;
    //以下、modeSwitchButtonの変数
    CharacterMGR.Mode mode = CharacterMGR.Mode.Auto; //デフォルトはAutoMode

    //以下、ManualRoute用の変数
   [SerializeField] bool isEditingManualRoute;
    Color selectedColor;
    Color notSelectedColor;

    //以下、CoolTime用の変数
    [SerializeField] Image emptyGauge;
    [SerializeField] Image filledGauge;
    [SerializeField] Color emptyGaugeColor;
    [SerializeField] Color filledGaugeColor;

    public void InitiSelectCharacterButton() //GameManagerがState.PlayingGameになったときに呼ぶ
    {
        modeSwitchButtonText.text = "Auto"; //デフォルトはAutoMode

        isEditingManualRoute = false;

        //色の初期化
        canSpawnCharacterColor = Color.white;
        canNotSpawnCharacterColor = new Color(0.4f,0.4f,0.4f,1); //若干濃いグレー
        selectedColor = Color.cyan;
        notSelectedColor = Color.gray;
        emptyGauge.color = Color.clear;
        filledGauge.color = Color.clear;

        //画像の初期化
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
                finalColor = canSpawnCharacterColor * selectedColor; //乗算で処理する
                Debug.LogWarning($"finalColor:{finalColor}");


            }
            else
            {
                finalColor = Color.clear; //この時だけ別で、透明扱いする
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

        finalColor.a *= 0.5f;　//ここまでのif分でfinalColorは不透明色になっているので、透明度を50％にする


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
    public void PointerDown() //EventTriggerで呼ぶ
    {
        if (GameManager.instance.state != GameManager.State.RunningGame) return;

        Debug.Log($"SelectCharacterButton{buttonNum}が押されました");

        if (Input.GetMouseButton(0))
        {
            GameManager.instance.SpawnCharacter(buttonNum);
        }
        else if (Input.GetMouseButton(1))
        {
            if (!isEditingManualRoute) //このbuttonが選択されていない状態で、このbuttonがクリックされたとき
            {
                Debug.LogWarning("SetToSelectedColorを実行します");
                SetToSelectedColor();
            }
            else //このbuttonが選択されている状態で、このbuttonがクリックされたとき
            {
                Debug.LogWarning("ResetToNormalColorを実行します");

                ResetToNormalColor();
            }
        }

    }

    public void SwitchMode() //ModeSwitchButtonのEventTriggerで呼ぶ
    {
        Debug.Log($"Button{buttonNum}のmodeSwitchButtonが押されました");
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
        if (GameManager.instance.inputMGR.GetSelectedButtonMGR() != null) //他のbuttonを選択していたとき
        {
            GameManager.instance.inputMGR.GetSelectedButtonMGR().ResetToNormalColor(); //そのbuttonの色を戻す
        }
        isEditingManualRoute = true;
        changeColorImage.color = selectedColor;
        Debug.LogWarning($"changeColorImage.color:{changeColorImage.color}");

        Debug.LogWarning($"buttonImage.colorを{selectedColor}に変更しました");
        GameManager.instance.inputMGR.SetManualRouteNumber(buttonNum);
        GameManager.instance.inputMGR.SetSelectedButtonMGR(this);
        Debug.LogWarning($"ManualRouteを選択するキャラクターを決定しました number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void ResetToNormalColor()
    {
        isEditingManualRoute = false;
        changeColorImage.color = notSelectedColor;
        Debug.LogWarning($"changeColorImage.color:{changeColorImage.color}");

        Debug.LogWarning($"ManualRouteのキャラクタ―選択を解除しました number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
        GameManager.instance.inputMGR.SetManualRouteNumber(-1);       //InputMGRでのManualRouteの選択を外す
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //インスペクター上でセットする

    [SerializeField] ButtonSizeMGR buttonSizeMGR; //インスペクター上でセットする

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
    [SerializeField] Color manualModeColor;
    [SerializeField] Color autoModeColor;

    //以下、ManualRoute用の変数
    [SerializeField] bool isEditingManualRoute;
    [SerializeField] Color selectedColor;
    Color notSelectedColor;
    [SerializeField] GameObject skillCanvas;
    [SerializeField] Image skillTriggerRouteImage;

    //以下、CoolTime用の変数
    [SerializeField] Image emptyGauge;
    [SerializeField] Image filledGauge;
    [SerializeField] Color emptyGaugeColor;
    [SerializeField] Color filledGaugeColor;


    public void InitiSelectCharacterButton() //GameManagerがState.PlayingGameになったときに呼ぶ
    {
        modeSwitchButtonText.text = "Auto"; //デフォルトはAutoMode
        modeSwitchButtonImage.color = autoModeColor;

        isEditingManualRoute = false;

        //色の初期化
        canSpawnCharacterColor = Color.white;
        canNotSpawnCharacterColor = new Color(0.4f,0.4f,0.4f,1); //若干濃いグレー
        //selectedColor = Color.cyan;    色調整のためにInspector上で変更する
        notSelectedColor = Color.gray;
        emptyGauge.color = Color.clear;
        filledGauge.color = Color.clear;

        skillTriggerRouteImage.sprite = GameManager.instance.characterSkillsDataMGR.skillTriggerImages[GameManager.instance.GetCharacterMGRFromButtonNum(buttonNum).GetSkillNum()] ;
        ResetToNormalColor();

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
                finalColor = canSpawnCharacterColor * selectedColor; //乗算で処理する
                //Debug.Log($"finalColor:{finalColor}");
            }
            else
            {
                finalColor = Color.clear; //この時だけ別で、透明扱いする
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

        finalColor.a *= 0.5f;　//ここまでのif分でfinalColorは不透明色になっているので、透明度を50％にする


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

        if (GameManager.instance.state == GameManager.State.ShowingResults && isEditingManualRoute) //ShowingResults時にPointerTailを消す
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
                //Debug.Log("SetToSelectedColorを実行します");
                SetToSelectedColor();
                StartCopyingManualRoute();
            }
            else //このbuttonが選択されている状態で、このbuttonがクリックされたとき
            {
                //Debug.Log("ResetToNormalColorを実行します");

                ResetToNormalColor();
            }
        }

    }

    public void StartCopyingManualRoute() //PointerDownで呼ぶ
    {
        if (!Input.GetMouseButtonDown(1)) return; //右クリックのときだけ処理を行う

        Debug.LogWarning($"ManualRouteのコピーを開始しました buttonNum={buttonNum}");
        GameManager.instance.copyingSelectCharacterButtonNum = buttonNum; //DropしたButtonにこのbuttonNumを渡す必要があるので、GameManagerを経由する
        GameManager.instance.copyingManualRoute = true;
        StartCoroutine(LateStartCoyingManualRoute());
    }

    private IEnumerator LateStartCoyingManualRoute() //InputMGRとの兼ね合いで、一フレーム遅らせる
    {
        yield return null;
        GameManager.instance.copyingManualRoute = true;
        GameManager.instance.copyingSelectCharacterButtonNum = buttonNum;
        GameManager.instance.manualRouteDatas[buttonNum].ShowSelectedManualRoute(); //このButtonに対応するManualRouteをPointerTailで表示する処理を書く
        GameManager.instance.curveToMouseMGR.SetCircles(buttonNum);
        GameManager.instance.curveToMouseMGR.StartIlluminatingPointersCroutine();
    }

    public void ShowLineBetweenButtonAndPointer() //EventTrigger Dragで呼ぶ
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
        //ManualRouteをコピー
        GameManager.instance.manualRouteDatas[buttonNum].SetManualRoute(GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetManualRoute());
        GameManager.instance.manualRouteDatas[buttonNum].SetNonDiagonalManualRoute(GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetNonDiagonalManualRoute());
        GameManager.instance.manualRouteDatas[buttonNum].SetNonDiagonalPoints(GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetNonDiagonalPoints());
        Debug.LogWarning($"ManualRouteを{GameManager.instance.copyingSelectCharacterButtonNum}から{buttonNum}へコピーしました\nManualRoute[{buttonNum}]={string.Join(",", GameManager.instance.manualRouteDatas[buttonNum].GetManualRoute())}");

        //Paste時にコピー元の選択を外す
        GameManager.instance.selectCharacterButtonMGRs[GameManager.instance.copyingSelectCharacterButtonNum].ResetToNormalColor();
        //pointerTailを消す
        GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].DestroyPointerTails();
        //CurvePointerを消す
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


    public void SwitchMode() //ModeSwitchButtonのEventTriggerで呼ぶ
    {
        Debug.Log($"Button{buttonNum}のmodeSwitchButtonが押されました");
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
        if (GameManager.instance.inputMGR.GetSelectedButtonMGR() != null) //他のbuttonを選択していたとき
        {
            GameManager.instance.inputMGR.GetSelectedButtonMGR().ResetToNormalColor(); //そのbuttonの色を戻す

            GameManager.instance.inputMGR.GetSelectedButtonMGR().CloseSkillCanvas(); //そのbuttonのSkillCanvasを閉じる
        }
        OpenSkillCanvas(); //SkillCanvasを開く(色の変更と同時なのでここに書く)
        isEditingManualRoute = true;
        changeColorImage.color = selectedColor;
        //Debug.Log($"changeColorImage.color:{changeColorImage.color}");

        //Debug.Log($"buttonImage.colorを{selectedColor}に変更しました");
        GameManager.instance.inputMGR.SetManualRouteNumber(buttonNum);
        GameManager.instance.inputMGR.SetSelectedButtonMGR(this);
        Debug.Log($"ManualRouteを選択するキャラクターを決定しました number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void ResetToNormalColor()
    {
        CloseSkillCanvas(); //SkillCanvasを閉じる
        isEditingManualRoute = false;
        changeColorImage.color = notSelectedColor;
        //Debug.Log($"changeColorImage.color:{changeColorImage.color}");

        Debug.Log($"ManualRouteのキャラクタ―選択を解除しました number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
        GameManager.instance.inputMGR.SetManualRouteNumber(-1);       //InputMGRでのManualRouteの選択を外す
        GameManager.instance.inputMGR.isEditingManualRoute = false; 
    }

    private void OpenSkillCanvas() //ManualMode編集時にそのキャラクターのスキルを表示する
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


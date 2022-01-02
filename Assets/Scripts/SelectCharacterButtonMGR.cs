using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //インスペクター上でセットする

    Text modeSwitchButtonText;

    //以下modeSwitchButtonの変数
    CharacterMGR.Mode mode = CharacterMGR.Mode.Auto; //デフォルトはAutoMode

    //以下、ManualRoute用の変数
    private Button button;
    [SerializeField] ColorBlock colorBlock;

    private void Start()
    {
        modeSwitchButtonText = transform.Find("ModeSwitchButton/Text").gameObject.GetComponent<Text>(); //孫オブジェクトからTextコンポーネントを取得する
        modeSwitchButtonText.text = "Auto Mode"; //デフォルトはAutoMode
        button = gameObject.GetComponent<Button>();
    }

    public void PointerDown()
    {
        Debug.Log($"SelectCharacterButton{buttonNum}が押されました");

        if (Input.GetMouseButton(0))
        {
            GameManager.instance.SpawnCharacter(buttonNum);
        }
        else if (Input.GetMouseButton(1))
        {
            if (button.colors != colorBlock) //このbuttonが選択されていない状態で、このbuttonがクリックされたとき
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
        button.colors = colorBlock;
        GameManager.instance.inputMGR.SetManualRouteNumber(buttonNum);
        GameManager.instance.inputMGR.SetSelectedButtonMGR(this);
        Debug.LogWarning($"ManualRouteを選択するキャラクターを決定しました number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }

    public void ResetToNormalColor()
    {
        button.colors = ColorBlock.defaultColorBlock;
        Debug.LogWarning($"ManualRouteのキャラクタ―選択を解除しました number={GameManager.instance.inputMGR.GetManualRouteNumber()}");
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //インスペクター上でセットする

    Text modeSwitchButtonText;

    //以下modeSwitchButtonの変数
    CharacterMGR.Mode mode = CharacterMGR.Mode.Auto; //デフォルトはAutoMode

    private void Start()
    {
        modeSwitchButtonText = transform.Find("ModeSwitchButton/Text").gameObject.GetComponent<Text>(); //孫オブジェクトからTextコンポーネントを取得する
        modeSwitchButtonText.text = "Auto Mode"; //デフォルトはAutoMode
    }

    public void OnClick()
    {
        Debug.Log($"Button{buttonNum}が押されました");

        GameManager.instance.SpawnCharacter(buttonNum);
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
}

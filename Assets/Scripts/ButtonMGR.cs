using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //インスペクター上でセットする

    public void OnClick()
    {
        Debug.Log($"Button{buttonNum}が押されました");

        GameManager.instance.SpawnCharacter(buttonNum);
    }
}

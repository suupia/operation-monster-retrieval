using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectStageButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //インスペクター上でセットする

    private Button button;


    private void Start()
    {
        button = gameObject.GetComponent<Button>();
    }

    public void PointerDown()
    {
        Debug.Log($"SelectStageButton{buttonNum}が押されました");



        GameManager.instance.mapMGR.SetStageNum(buttonNum);
        GameManager.instance.SetupGame();
    }


}

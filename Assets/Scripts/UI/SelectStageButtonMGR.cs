using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectStageButtonMGR : MonoBehaviour
{
    [SerializeField] int buttonNum; //インスペクター上でセットする

    [SerializeField] ButtonSizeMGR buttonSizeMGR; //インスペクター上でセットする
    [SerializeField] Image selectSatageButtonImage; //インスペクター上でセットする 色を変えるため

    Color cannotSelectColor; //黒に近い色にする


    private void Start()
    {
        cannotSelectColor = new Color(0.04f,0.04f,0.04f,1);//黒に近い色にする

        StartCoroutine(LateStart(0.3f)) ;
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        UpdateSelectStageButtonMGR();

    }

    public void UpdateSelectStageButtonMGR() //StagesClearedNumのプロパティで更新する
    {
        Debug.LogWarning($"UpdateSelectStageButtonMGRを開始します\nStagesClearedNum +1:{GameManager.instance.StagesClearedNum + 1}");
        if (buttonNum <= GameManager.instance.StagesClearedNum +1) //（クリアステージの添え字）+1　のステージまで選択できる
        {
            buttonSizeMGR.SetIsActive(true);
            selectSatageButtonImage.color = Color.white;
            //Debug.Log("buttonSizeMGRをtrueにしました");
        }
        else
        {
            buttonSizeMGR.SetIsActive(false);
            selectSatageButtonImage.color = cannotSelectColor;
            //Debug.Log("buttonSizeMGRをfalseにしました");
        }
    }

    public void PointerDown() //EventTriggerで呼ぶ
    {
        Debug.Log($"SelectStageButton{buttonNum}が押されました");

        if (buttonNum <= GameManager.instance.StagesClearedNum +1)
        {
            GameManager.instance.mapMGR.SetStageNum(buttonNum);
            GameManager.instance.SetupGame();
        }
        else
        {
            //何もしない
            //押せない効果音とかつけるかもしれない
        }
    }


}

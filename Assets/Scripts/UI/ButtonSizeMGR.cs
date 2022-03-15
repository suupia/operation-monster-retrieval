using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSizeMGR : MonoBehaviour
{

    [SerializeField] RectTransform rectTransform; //インスペクター上でセットしておく

    bool isActive = true;

    //Setter
    public void SetIsActive(bool flag) //外部から変更する
    {
        if(flag == false && gameObject.activeInHierarchy) RestoreSize();   //フラグがfalseの時は、サイズを戻してからfalseにする(初期化の時はこのスクリプトを付けているオブジェクトがfalseのときがあるので、それは飛ばす)

        isActive = flag;
    }

    [SerializeField] float magnificationPower = 1.15f; //拡大倍率
    float time = 0.08f; //サイズ変更が完了するまでの時間

    float initiWidth; //ゲーム開始時のサイズを保持しておく
    float initiHeight;

    private void Start()
    {
        initiWidth = rectTransform.sizeDelta.x;
        initiHeight = rectTransform.sizeDelta.y;
    }
    public void IncreaseSize() //EventTriggerのPointerEnterで呼ぶ
    {
        if (!isActive) return;
        StartCoroutine(MakeButtonBiggerCoroutine());
    }

    public void RestoreSize() //EventTriggerのPointerExitで呼ぶ
    {
        if (!isActive) return;
        StartCoroutine(RestoreTheSizeCoroutine());
    }

    IEnumerator MakeButtonBiggerCoroutine()
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            float rate = (magnificationPower - 1) * (t / time) + 1; //線形補完
            rectTransform.sizeDelta = new Vector2(initiWidth * rate, initiHeight * rate);
            yield return null;
        }
    }

    IEnumerator RestoreTheSizeCoroutine()
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            float rate = (1 - 1) * (t / time) + 1; //線形補完
            rectTransform.sizeDelta = new Vector2(initiWidth * rate, initiHeight * rate);
            yield return null;
        }
    }
}

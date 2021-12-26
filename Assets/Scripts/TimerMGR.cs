using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimerMGR : MonoBehaviour
{
    [SerializeField] Text timerText; //インスペクター上でセットする

    bool isActive = true;
    [SerializeField] float timeLimit; //秒で指定する
    [SerializeField] float timer; //デバッグしやすいようにSerializeFieldにしておく

    void Start()
    {
        InitiTimer();
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        timerText.text = ConvertStringTime(timeLimit - timer);

        if (timer > timeLimit)
        {
            Debug.LogWarning("戦闘に負けました");
            GameManager.instance.LoseTheGame();
        }
    }

    public void InitiTimer()
    {
        timer = 0;
    }
    public void StopTimer()
    {
        isActive = false;
    }
    public void ReStartTimer()
    {
        isActive = true;
    }
    string ConvertStringTime(float time) //秒をmm:ssに変換するメソッド
    {
        int m = (int)(time / 60f); //秒から分の値を取得
        int s = (int)(time % 60f); //秒のみを取得
        return m.ToString("00") + ":" + s.ToString("00");
    }
}

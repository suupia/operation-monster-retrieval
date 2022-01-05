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

    public void InitiTimer() //GameManagerがState.PlayingGameになったときに呼ぶ
    {
        timer = 0;
    }
    void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.state != GameManager.State.PlayingGame) return; //以下の処理はGameManagerがPlayingGameの時のみ実行される


        timer += Time.deltaTime;
        timerText.text = ConvertStringTime(timeLimit - timer);

        if (timer > timeLimit)
        {
            Debug.LogWarning("戦闘に負けました");
            GameManager.instance.StartShowingResults(false);
        }
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

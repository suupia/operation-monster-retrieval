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

    //プロパティ
    public float Timer //CharacterSkillsMGRから呼びたいのでプロパティを作っておく
    {
        get { return timer; }
        set { timer = value; }
    }
    public void InitiTimer() //GameManagerがState.PlayingGameになったときに呼ぶ
    {
        Timer = 0;
        timerText.text = ConvertStringTime(timeLimit - Timer);

    }
    void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.state != GameManager.State.RunningGame) return; //以下の処理はGameManagerがPlayingGameの時のみ実行される


        Timer += Time.deltaTime * GameManager.instance.gameSpeed;
        timerText.text = ConvertStringTime(timeLimit - Timer);

        if (Timer > timeLimit)
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

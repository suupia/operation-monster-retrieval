using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CriWare;

public class MusicMGR : MonoBehaviour
{
    CriAtomExPlayer bgmAtomExPlayer;
    CriAtomExPlayback bgmPlayback;  //パラメーター更新のために必要
    CriAtomExPlayer seAtomExPlayer;
    CriAtomExPlayback sePlayback;
    CriAtomExAcb stageBGMAcb;          //StageBGMシートのacbファイルを読み込む
    CriAtomEx.CueInfo[] stageBGMCueInfoList; //StageBGMシートのキュー情報
    CriAtomExAcb combatSEAcb;

    [SerializeField] Slider bgmVolSlider; //インスペクター上でセットする
    [SerializeField] Slider seVolSlider;    //インスペクター上でセットする

    bool isSetup; //スライダーを初期化したときにOnBGMVolSliderChangedなどでnull参照が起きないようにするために必要

    IEnumerator Start()
    {
        isSetup = true;

        //ボリューム調節のスライダーの位置をjsonから読み込む(保存は戦闘終了時に行う)
        bgmVolSlider.value = GameManager.instance.saveMGR.GetBGMVolume();
        seVolSlider.value = GameManager.instance.saveMGR.GetSEVolume();


        //キューシートファイルのロード待ち
        while (CriAtom.CueSheetsAreLoading)
        {
            yield return null;
        }

        //AtomExPlayerの生成
        Debug.Log("AtomExPlayerを生成します");
        bgmAtomExPlayer = new CriAtomExPlayer();
        bgmAtomExPlayer.SetVolume(bgmVolSlider.value);
        seAtomExPlayer = new CriAtomExPlayer();
        seAtomExPlayer.SetVolume(seVolSlider.value);

        //Cue情報の取得
        stageBGMAcb = CriAtom.GetAcb("StageBGM");
        combatSEAcb = CriAtom.GetAcb("CombatSE");
        stageBGMCueInfoList = stageBGMAcb.GetCueInfoList();

        isSetup = false;
    }

    private void OnDestroy()
    {
        bgmAtomExPlayer.Dispose();
    }

    //public void StartAndStopSound(int index)
    //{
    //    if (atomExPlayer == null)
    //    {
    //        Debug.LogError($"atomExPlayerがインスタンス化されていません");
    //        return;
    //    }

    //    //CriAtomExPlayerの状態を取得
    //    CriAtomExPlayer.Status status = atomExPlayer.GetStatus();

    //    //プレイヤーにキューを設定
    //    atomExPlayer.SetCue(atomExMUSICAcb, musicCueInfoList[index].name);

    //    switch (status)
    //    {
    //        case CriAtomExPlayer.Status.Stop:
    //            atomExPlayer.Start();
    //            break;
    //        case CriAtomExPlayer.Status.Playing:
    //            atomExPlayer.Stop();
    //            break;
    //        default:
    //            Debug.LogError($"atomExPlayer.GetStatus()の状態が[Stop]でも[Playing]でもありません　atomExPlayer.GetStatus():{atomExPlayer.GetStatus()}");
    //            break;
    //    }

    //}

    //StageBGM関連
    public void StartStageBGM(int index)
    {
        if (bgmAtomExPlayer == null)
        {
            Debug.LogError($"atomExPlayerがインスタンス化されていません");
            return;
        }

        if (bgmAtomExPlayer.GetStatus() == CriAtomExPlayer.Status.Stop)
        {
            //プレイヤーにStageBGMシートのキューを設定
            bgmAtomExPlayer.SetCue(stageBGMAcb, stageBGMCueInfoList[index].name);
            //再生
            bgmPlayback =  bgmAtomExPlayer.Start();
        }
        else
        {
            Debug.LogError($"atomExPlayer.GetStatus()の状態が[Stop]でありません　atomExPlayer.GetStatus():{bgmAtomExPlayer.GetStatus()}");
        }

    }

    public void StopStageBGM(int index)
    {
        if (bgmAtomExPlayer == null)
        {
            Debug.LogError($"bgmAtomExPlayerがインスタンス化されていません");
            return;
        }

        if (bgmAtomExPlayer.GetStatus() == CriAtomExPlayer.Status.Playing)
        {
            //プレイヤーにMUSICシートのキューを設定
            bgmAtomExPlayer.SetCue(stageBGMAcb, stageBGMCueInfoList[index].name);
            //停止
            bgmAtomExPlayer.Stop();
        }
        else
        {
            Debug.LogError($"bgmAtomExPlayer.GetStatus()の状態が[Playing]でありません　bgmAtomExPlayer.GetStatus():{bgmAtomExPlayer.GetStatus()}");
        }
    }

    public void StopAllBGM() //デバッグ用なのでずさんなつくり
    {
        bgmAtomExPlayer.Stop();
    }

    //CombatSE関連
    public void StartCombatSE(string queName)
    {
        if (seAtomExPlayer == null)
        {
            Debug.LogError($"seAtomExPlayerがインスタンス化されていません");
            return;
        }

        //SEだから、seAtomExPlayerの状態によらず、StartでOK
        seAtomExPlayer.SetCue(combatSEAcb, queName);
        seAtomExPlayer.Start();

    }

    public void OnBGMVolSliderChanged() //スライダーのイベントコールバック用（インスペクター上でセットする）
    {
        if (isSetup) return;
        bgmAtomExPlayer.SetVolume(bgmVolSlider.value);
        bgmAtomExPlayer.Update(bgmPlayback);
    }

    public void OnSEVolSliderChanged()
    {
        if (isSetup) return;
        seAtomExPlayer.SetVolume(seVolSlider.value);
        seAtomExPlayer.Update(sePlayback);
    }

    public void SaveBGMAndSEVolume()
    {
        GameManager.instance.saveMGR.SaveBGMVolume(bgmVolSlider.value);
        GameManager.instance.saveMGR.SaveSEVolume(seVolSlider.value);

    }
}

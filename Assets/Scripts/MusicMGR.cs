using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CriWare;

public class MusicMGR : MonoBehaviour
{
    private CriAtomExPlayer atomExPlayer;
    private CriAtomExAcb atomExStageBGMAcb;          //StageBGMシートのacbファイルを読み込む
    private CriAtomEx.CueInfo[] stageBGMCueInfoList; //StageBGMシートのキュー情報

    IEnumerator Start()
    {
        //キューシートファイルのロード待ち
        while (CriAtom.CueSheetsAreLoading)
        {
            yield return null;
        }

        //AtomExPlayerの生成
        Debug.Log("AtomExPlayerを生成します");
        atomExPlayer = new CriAtomExPlayer();

        //Cue情報の取得
        atomExStageBGMAcb = CriAtom.GetAcb("StageBGM");
        stageBGMCueInfoList = atomExStageBGMAcb.GetCueInfoList();
    }

    private void OnDestroy()
    {
        atomExPlayer.Dispose();
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

    public void StartStageBGM(int index)
    {
        if (atomExPlayer == null)
        {
            Debug.LogError($"atomExPlayerがインスタンス化されていません");
            return;
        }

        if (atomExPlayer.GetStatus() == CriAtomExPlayer.Status.Stop)
        {
            //プレイヤーにStageBGMシートのキューを設定
            atomExPlayer.SetCue(atomExStageBGMAcb, stageBGMCueInfoList[index].name);
            //再生
            atomExPlayer.Start();
        }
        else
        {
            Debug.LogError($"atomExPlayer.GetStatus()の状態が[Stop]でありません　atomExPlayer.GetStatus():{atomExPlayer.GetStatus()}");
        }

    }

    public void StopStageBGM(int index)
    {
        if (atomExPlayer == null)
        {
            Debug.LogError($"atomExPlayerがインスタンス化されていません");
            return;
        }

        if (atomExPlayer.GetStatus() == CriAtomExPlayer.Status.Playing)
        {
            //プレイヤーにMUSICシートのキューを設定
            atomExPlayer.SetCue(atomExStageBGMAcb, stageBGMCueInfoList[index].name);
            //停止
            atomExPlayer.Stop();
        }
        else
        {
            Debug.LogError($"atomExPlayer.GetStatus()の状態が[Playing]でありません　atomExPlayer.GetStatus():{atomExPlayer.GetStatus()}");
        }
    }

    public void StopAllBGM() //デバッグ用なのでずさんなつくり
    {
        atomExPlayer.Stop();
    }

}

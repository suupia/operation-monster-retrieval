using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CriWare;

public class MusicMGR : MonoBehaviour
{
    private CriAtomExPlayer atomExPlayer;
    private CriAtomExAcb atomExStageBGMAcb;          //StageBGM�V�[�g��acb�t�@�C����ǂݍ���
    private CriAtomEx.CueInfo[] stageBGMCueInfoList; //StageBGM�V�[�g�̃L���[���

    IEnumerator Start()
    {
        //�L���[�V�[�g�t�@�C���̃��[�h�҂�
        while (CriAtom.CueSheetsAreLoading)
        {
            yield return null;
        }

        //AtomExPlayer�̐���
        Debug.Log("AtomExPlayer�𐶐����܂�");
        atomExPlayer = new CriAtomExPlayer();

        //Cue���̎擾
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
    //        Debug.LogError($"atomExPlayer���C���X�^���X������Ă��܂���");
    //        return;
    //    }

    //    //CriAtomExPlayer�̏�Ԃ��擾
    //    CriAtomExPlayer.Status status = atomExPlayer.GetStatus();

    //    //�v���C���[�ɃL���[��ݒ�
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
    //            Debug.LogError($"atomExPlayer.GetStatus()�̏�Ԃ�[Stop]�ł�[Playing]�ł�����܂���@atomExPlayer.GetStatus():{atomExPlayer.GetStatus()}");
    //            break;
    //    }

    //}

    public void StartStageBGM(int index)
    {
        if (atomExPlayer == null)
        {
            Debug.LogError($"atomExPlayer���C���X�^���X������Ă��܂���");
            return;
        }

        if (atomExPlayer.GetStatus() == CriAtomExPlayer.Status.Stop)
        {
            //�v���C���[��StageBGM�V�[�g�̃L���[��ݒ�
            atomExPlayer.SetCue(atomExStageBGMAcb, stageBGMCueInfoList[index].name);
            //�Đ�
            atomExPlayer.Start();
        }
        else
        {
            Debug.LogError($"atomExPlayer.GetStatus()�̏�Ԃ�[Stop]�ł���܂���@atomExPlayer.GetStatus():{atomExPlayer.GetStatus()}");
        }

    }

    public void StopStageBGM(int index)
    {
        if (atomExPlayer == null)
        {
            Debug.LogError($"atomExPlayer���C���X�^���X������Ă��܂���");
            return;
        }

        if (atomExPlayer.GetStatus() == CriAtomExPlayer.Status.Playing)
        {
            //�v���C���[��MUSIC�V�[�g�̃L���[��ݒ�
            atomExPlayer.SetCue(atomExStageBGMAcb, stageBGMCueInfoList[index].name);
            //��~
            atomExPlayer.Stop();
        }
        else
        {
            Debug.LogError($"atomExPlayer.GetStatus()�̏�Ԃ�[Playing]�ł���܂���@atomExPlayer.GetStatus():{atomExPlayer.GetStatus()}");
        }
    }

    public void StopAllBGM() //�f�o�b�O�p�Ȃ̂ł�����Ȃ���
    {
        atomExPlayer.Stop();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CriWare;

public class MusicMGR : MonoBehaviour
{
    CriAtomExPlayer bgmAtomExPlayer;
    CriAtomExPlayback bgmPlayback;  //�p�����[�^�[�X�V�̂��߂ɕK�v
    CriAtomExPlayer seAtomExPlayer;
    CriAtomExPlayback sePlayback;
    CriAtomExAcb stageBGMAcb;          //StageBGM�V�[�g��acb�t�@�C����ǂݍ���
    CriAtomEx.CueInfo[] stageBGMCueInfoList; //StageBGM�V�[�g�̃L���[���
    CriAtomExAcb combatSEAcb;

    [SerializeField] Slider bgmVolSlider; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] Slider seVolSlider;    //�C���X�y�N�^�[��ŃZ�b�g����

    bool isSetup; //�X���C�_�[�������������Ƃ���OnBGMVolSliderChanged�Ȃǂ�null�Q�Ƃ��N���Ȃ��悤�ɂ��邽�߂ɕK�v

    IEnumerator Start()
    {
        isSetup = true;

        //�{�����[�����߂̃X���C�_�[�̈ʒu��json����ǂݍ���(�ۑ��͐퓬�I�����ɍs��)
        bgmVolSlider.value = GameManager.instance.saveMGR.GetBGMVolume();
        seVolSlider.value = GameManager.instance.saveMGR.GetSEVolume();


        //�L���[�V�[�g�t�@�C���̃��[�h�҂�
        while (CriAtom.CueSheetsAreLoading)
        {
            yield return null;
        }

        //AtomExPlayer�̐���
        Debug.Log("AtomExPlayer�𐶐����܂�");
        bgmAtomExPlayer = new CriAtomExPlayer();
        bgmAtomExPlayer.SetVolume(bgmVolSlider.value);
        seAtomExPlayer = new CriAtomExPlayer();
        seAtomExPlayer.SetVolume(seVolSlider.value);

        //Cue���̎擾
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

    //StageBGM�֘A
    public void StartStageBGM(int index)
    {
        if (bgmAtomExPlayer == null)
        {
            Debug.LogError($"atomExPlayer���C���X�^���X������Ă��܂���");
            return;
        }

        if (bgmAtomExPlayer.GetStatus() == CriAtomExPlayer.Status.Stop)
        {
            //�v���C���[��StageBGM�V�[�g�̃L���[��ݒ�
            bgmAtomExPlayer.SetCue(stageBGMAcb, stageBGMCueInfoList[index].name);
            //�Đ�
            bgmPlayback =  bgmAtomExPlayer.Start();
        }
        else
        {
            Debug.LogError($"atomExPlayer.GetStatus()�̏�Ԃ�[Stop]�ł���܂���@atomExPlayer.GetStatus():{bgmAtomExPlayer.GetStatus()}");
        }

    }

    public void StopStageBGM(int index)
    {
        if (bgmAtomExPlayer == null)
        {
            Debug.LogError($"bgmAtomExPlayer���C���X�^���X������Ă��܂���");
            return;
        }

        if (bgmAtomExPlayer.GetStatus() == CriAtomExPlayer.Status.Playing)
        {
            //�v���C���[��MUSIC�V�[�g�̃L���[��ݒ�
            bgmAtomExPlayer.SetCue(stageBGMAcb, stageBGMCueInfoList[index].name);
            //��~
            bgmAtomExPlayer.Stop();
        }
        else
        {
            Debug.LogError($"bgmAtomExPlayer.GetStatus()�̏�Ԃ�[Playing]�ł���܂���@bgmAtomExPlayer.GetStatus():{bgmAtomExPlayer.GetStatus()}");
        }
    }

    public void StopAllBGM() //�f�o�b�O�p�Ȃ̂ł�����Ȃ���
    {
        bgmAtomExPlayer.Stop();
    }

    //CombatSE�֘A
    public void StartCombatSE(string queName)
    {
        if (seAtomExPlayer == null)
        {
            Debug.LogError($"seAtomExPlayer���C���X�^���X������Ă��܂���");
            return;
        }

        //SE������AseAtomExPlayer�̏�Ԃɂ�炸�AStart��OK
        seAtomExPlayer.SetCue(combatSEAcb, queName);
        seAtomExPlayer.Start();

    }

    public void OnBGMVolSliderChanged() //�X���C�_�[�̃C�x���g�R�[���o�b�N�p�i�C���X�y�N�^�[��ŃZ�b�g����j
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

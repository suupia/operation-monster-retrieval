using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimerMGR : MonoBehaviour
{
    [SerializeField] Text timerText; //�C���X�y�N�^�[��ŃZ�b�g����

    bool isActive = true;
    [SerializeField] float timeLimit; //�b�Ŏw�肷��
    [SerializeField] float timer; //�f�o�b�O���₷���悤��SerializeField�ɂ��Ă���

    //�v���p�e�B
    public float Timer //CharacterSkillsMGR����Ăт����̂Ńv���p�e�B������Ă���
    {
        get { return timer; }
        set { timer = value; }
    }
    public void InitiTimer() //GameManager��State.PlayingGame�ɂȂ����Ƃ��ɌĂ�
    {
        Timer = 0;
        timerText.text = ConvertStringTime(timeLimit - Timer);

    }
    void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.state != GameManager.State.RunningGame) return; //�ȉ��̏�����GameManager��PlayingGame�̎��̂ݎ��s�����


        Timer += Time.deltaTime * GameManager.instance.gameSpeed;
        timerText.text = ConvertStringTime(timeLimit - Timer);

        if (Timer > timeLimit)
        {
            Debug.LogWarning("�퓬�ɕ����܂���");
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
    string ConvertStringTime(float time) //�b��mm:ss�ɕϊ����郁�\�b�h
    {
        int m = (int)(time / 60f); //�b���番�̒l���擾
        int s = (int)(time % 60f); //�b�݂̂��擾
        return m.ToString("00") + ":" + s.ToString("00");
    }
}

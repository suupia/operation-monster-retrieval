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
            Debug.LogWarning("�퓬�ɕ����܂���");
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
    string ConvertStringTime(float time) //�b��mm:ss�ɕϊ����郁�\�b�h
    {
        int m = (int)(time / 60f); //�b���番�̒l���擾
        int s = (int)(time % 60f); //�b�݂̂��擾
        return m.ToString("00") + ":" + s.ToString("00");
    }
}

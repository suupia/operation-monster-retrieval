using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnergyMGR : MonoBehaviour
{
    [SerializeField] Text energyText; //�C���X�y�N�^�[��ŃZ�b�g����

    bool isActive = true;

    [SerializeField] float energyGain; //1�b������̃G�l���M�[�l����
    [SerializeField] float energyMax; //�G�l���M�[�̍ő�l
    float currentEnergy; //���݂̃G�l���M�[��

    //�v���p�e�B
    public float CurrentEnergy
    {
        get { return currentEnergy; }
        set { currentEnergy = value; }
    }


    void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.state != GameManager.State.PlayingGame) return; //�ȉ��̏�����GameManager��PlayingGame�̎��̂ݎ��s�����


        currentEnergy += energyGain * Time.deltaTime;
        energyText.text = Mathf.FloorToInt(currentEnergy) + "/" + Mathf.FloorToInt(energyMax);

    }

    public void InitiEnergy()
    {
        currentEnergy = 0;
    }
    public void StopEnergyCounting()
    {
        isActive = false;
    }
    public void ReStartEnergyCounting()
    {
        isActive = true;
    }

}

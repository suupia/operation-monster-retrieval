using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnergyMGR : MonoBehaviour
{
    [SerializeField] Text energyText; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] Image energyLevelUpImage;
    [SerializeField] Text energyLevelUpText;

    bool isActive = true;

    float energyGain; //1�b������̃G�l���M�[�l����
    [SerializeField] float[] energyGainArray; //���x���A�b�v�łǂ̂悤�ɕω����Ă������z��Ō��߂�
    float energyMax; //�G�l���M�[�̍ő�l
    [SerializeField] float[] energyMaxArray; //���x���A�b�v�łǂ̂悤�ɕω����Ă������z��Ō��߂�

    [SerializeField] float currentEnergy; //���݂̃G�l���M�[�ʁ@�f�o�b�O���₷���悤��SerializeField�ɂ��Ă���

    [SerializeField] int level; //�f�o�b�O���₷���悤��SerializeField�ɂ��Ă���
    [SerializeField] float[] energyRequiredArray;

    Color canLevelUpColor = Color.white;
    Color canNotLevelUpColor = Color.gray;

    //�v���p�e�B
    public float CurrentEnergy
    {
        get { return currentEnergy; }
        set { 
            if(value >= energyMax)
            {
                currentEnergy = energyMax; //�s�b�^���ő�l�ɂ���
            }
            else
            {
                currentEnergy = value;
            }
            energyText.text = Mathf.FloorToInt(currentEnergy) + "/" + Mathf.FloorToInt(energyMax)+"ene";
        }
    }
    int LEVEL
    {
        get { return level; }
        set {
            if (value >= energyRequiredArray.Length) {
                Debug.Log($"���x�����ő�Ȃ̂ł���ȏヌ�x���A�b�v���܂���");
                return;
            }
            level = value;
            energyLevelUpText.text = "LEVEL " + (level+1) +"\n"+energyRequiredArray[level]+"ene";
        }
    }
    public void InitiEnergy() //GameManager��State.PlayingGame�ɂȂ����Ƃ��ɌĂ�
    {
        CurrentEnergy = 0;
        LEVEL = 0;
        energyGain = energyGainArray[LEVEL];
        energyMax = energyMaxArray[LEVEL];
    }

    void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.state != GameManager.State.PlayingGame) return; //�ȉ��̏�����GameManager��PlayingGame�̎��̂ݎ��s�����

        if (GameManager.instance.energyMGR.CurrentEnergy >= energyRequiredArray[LEVEL])
        {
            energyLevelUpImage.color = canLevelUpColor;
        }
        else
        {
            energyLevelUpImage.color = canNotLevelUpColor;
        }

        CurrentEnergy += energyGain * Time.deltaTime;

    }



    public void StopEnergyCounting()
    {
        isActive = false;
    }
    public void ReStartEnergyCounting()
    {
        isActive = true;
    }

    public void LevelUpEnergy() //EventTrigger�ŌĂ�
    {
        if (GameManager.instance.state != GameManager.State.PlayingGame) return; //�ȉ��̏�����GameManager��PlayingGame�̎��̂ݎ��s�����

        if (CurrentEnergy < energyRequiredArray[LEVEL]) return; //Energy������Ȃ��ƃ��x���A�b�v�ł��Ȃ�

        if (LEVEL >= energyRequiredArray.Length) return; //LEVEL���ő�Ȃ̂ł���ȏヌ�x���A�b�v���Ȃ�

        CurrentEnergy -= energyRequiredArray[LEVEL];
        LEVEL++;
        energyGain = energyGainArray[LEVEL];
        energyMax = energyMaxArray[LEVEL];
    }

}

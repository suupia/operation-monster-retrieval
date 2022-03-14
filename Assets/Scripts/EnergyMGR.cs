using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnergyMGR : MonoBehaviour
{
    [SerializeField] ButtonSizeMGR buttonSizeMGR; //�C���X�y�N�^�[��ŃZ�b�g����

    [SerializeField] Text energyText; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] Image energyLevelUpImage;
    [SerializeField] Text energyLevelUpText;
    [SerializeField] GameObject energyLevelUpHighlight;

    [SerializeField] Image ImageToChangeColor;

    bool isActive = true;

    float energyGain; //1�b������̃G�l���M�[�l����

    [SerializeField] int energyToMakeRoad; //������邽�߂ɕK�v�ȃG�l���M�[�̗�
    public int EnergyToMakeRoad
    {
        get { return energyToMakeRoad; }
    }

    [SerializeField] float[] energyGainArray; //���x���A�b�v�łǂ̂悤�ɕω����Ă������z��Ō��߂�
    float energyMax; //�G�l���M�[�̍ő�l
    [SerializeField] float[] energyMaxArray; //���x���A�b�v�łǂ̂悤�ɕω����Ă������z��Ō��߂�

    [SerializeField] float currentEnergy; //���݂̃G�l���M�[�ʁ@�f�o�b�O���₷���悤��SerializeField�ɂ��Ă���

    [SerializeField] int level; //�f�o�b�O���₷���悤��SerializeField�ɂ��Ă��� �����l��0�Ȃ̂Ŏ��ۂ̃��x����level+1
    [SerializeField] float[] energyRequiredArray;

    Color canLevelUpColor = Color.clear;//����
    Color canNotLevelUpColor = new Color(0, 0, 0, 0.3f); //�s�����x������D�F

    [SerializeField] Color[] energyLevelUpButtonColors; //Level�ɂ���ĐF��ς���p

    //�f�o�b�O���[�h
    [SerializeField] bool isInDebugMode;


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
        energyLevelUpImage.color = energyLevelUpButtonColors[LEVEL];

        ImageToChangeColor.color = canNotLevelUpColor;
        energyLevelUpHighlight.SetActive(false);
        buttonSizeMGR.SetIsActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.state != GameManager.State.RunningGame) return; //�ȉ��̏�����GameManager��PlayingGame�̎��̂ݎ��s�����

        if(LEVEL + 1 >= energyRequiredArray.Length)
        {
            ImageToChangeColor.color = canLevelUpColor; //LEVEL���ő�̂Ƃ���color��clear�ɂ���
            energyLevelUpHighlight.SetActive(true);
            buttonSizeMGR.SetIsActive(false); //LEVEL���ő�̎��̓J�[�\�������킹�Ă��{�^���͑傫���Ȃ�Ȃ�
        }
        else if (GameManager.instance.energyMGR.CurrentEnergy >= energyRequiredArray[LEVEL])
        {
            ImageToChangeColor.color = canLevelUpColor;
            energyLevelUpHighlight.SetActive(true);
            buttonSizeMGR.SetIsActive(true);
        }
        else
        {
            ImageToChangeColor.color = canNotLevelUpColor;
            energyLevelUpHighlight.SetActive(false);
            buttonSizeMGR.SetIsActive(false);
        }

        CurrentEnergy += energyGain * Time.deltaTime;

        //�f�o�b�O���[�h
        if (isInDebugMode) CurrentEnergy = energyMax;

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
        if (GameManager.instance.state != GameManager.State.RunningGame) return; //�ȉ��̏�����GameManager��PlayingGame�̎��̂ݎ��s�����

        if (CurrentEnergy < energyRequiredArray[LEVEL]) return; //Energy������Ȃ��ƃ��x���A�b�v�ł��Ȃ�

        if (LEVEL +1 >= energyRequiredArray.Length) return; //LEVEL���ő�Ȃ̂ł���ȏヌ�x���A�b�v���Ȃ�

        CurrentEnergy -= energyRequiredArray[LEVEL];
        LEVEL++;
        energyGain = energyGainArray[LEVEL];
        energyMax = energyMaxArray[LEVEL];

        //level�ɂ����button�̐F��ς���
        energyLevelUpImage.color = energyLevelUpButtonColors[LEVEL];
    }

}

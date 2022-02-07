using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnergyMGR : MonoBehaviour
{
    [SerializeField] Text energyText; //インスペクター上でセットする
    [SerializeField] Image energyLevelUpImage;
    [SerializeField] Text energyLevelUpText;

    [SerializeField] Image ImageToChangeColor;

    bool isActive = true;

    float energyGain; //1秒あたりのエネルギー獲得量
    [SerializeField] float[] energyGainArray; //レベルアップでどのように変化していくか配列で決める
    float energyMax; //エネルギーの最大値
    [SerializeField] float[] energyMaxArray; //レベルアップでどのように変化していくか配列で決める

    [SerializeField] float currentEnergy; //現在のエネルギー量　デバッグしやすいようにSerializeFieldにしておく

    [SerializeField] int level; //デバッグしやすいようにSerializeFieldにしておく
    [SerializeField] float[] energyRequiredArray;

    Color canLevelUpColor = Color.clear;//透明
    Color canNotLevelUpColor = new Color(0, 0, 0, 0.3f); //不当明度がある灰色


    //プロパティ
    public float CurrentEnergy
    {
        get { return currentEnergy; }
        set { 
            if(value >= energyMax)
            {
                currentEnergy = energyMax; //ピッタリ最大値にする
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
                Debug.Log($"レベルが最大なのでこれ以上レベルアップしません");
                return;
            }
            level = value;
            energyLevelUpText.text = "LEVEL " + (level+1) +"\n"+energyRequiredArray[level]+"ene";
        }
    }
    public void InitiEnergy() //GameManagerがState.PlayingGameになったときに呼ぶ
    {
        CurrentEnergy = 0;
        LEVEL = 0;
        energyGain = energyGainArray[LEVEL];
        energyMax = energyMaxArray[LEVEL];
    }

    void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.state != GameManager.State.RunningGame) return; //以下の処理はGameManagerがPlayingGameの時のみ実行される

        if (GameManager.instance.energyMGR.CurrentEnergy >= energyRequiredArray[LEVEL])
        {
            ImageToChangeColor.color = canLevelUpColor;
        }
        else
        {
            ImageToChangeColor.color = canNotLevelUpColor;
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

    public void LevelUpEnergy() //EventTriggerで呼ぶ
    {
        if (GameManager.instance.state != GameManager.State.SetupGame) return; //以下の処理はGameManagerがPlayingGameの時のみ実行される

        if (CurrentEnergy < energyRequiredArray[LEVEL]) return; //Energyが足りないとレベルアップできない

        if (LEVEL >= energyRequiredArray.Length) return; //LEVELが最大なのでこれ以上レベルアップしない

        CurrentEnergy -= energyRequiredArray[LEVEL];
        LEVEL++;
        energyGain = energyGainArray[LEVEL];
        energyMax = energyMaxArray[LEVEL];
    }

}

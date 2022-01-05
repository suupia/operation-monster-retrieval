using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnergyMGR : MonoBehaviour
{
    [SerializeField] Text energyText; //インスペクター上でセットする

    bool isActive = true;

    [SerializeField] float energyGain; //1秒あたりのエネルギー獲得量
    [SerializeField] float energyMax; //エネルギーの最大値
    float currentEnergy; //現在のエネルギー量

    //プロパティ
    public float CurrentEnergy
    {
        get { return currentEnergy; }
        set { currentEnergy = value; }
    }


    void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.state != GameManager.State.PlayingGame) return; //以下の処理はGameManagerがPlayingGameの時のみ実行される


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

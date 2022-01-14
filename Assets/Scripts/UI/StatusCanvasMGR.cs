using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusCanvasMGR : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text characterTypeIDText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;
    [SerializeField] Text atkText;
    [SerializeField] Text attackIntervalText;
    [SerializeField] Text attackRangeText;
    [SerializeField] Text spdText;
    [SerializeField] Text coolTimeText;
    [SerializeField] Text costText;

    public void UpdateStatusCanvasInCombat(int dropNum)
    {
        UpdateStatusCanvas(GameManager.instance.IDsOfCharactersInCombat[dropNum]);
    }

    public void UpdateStatusCanvasInReserve(int dragNum)
    {
        UpdateStatusCanvas(dragNum);
    }

    void UpdateStatusCanvas(int characterTypeID)
    {
        nameText.text = GameManager.instance.GetCharacterDatabase(characterTypeID).GetCharacterName();
        characterTypeIDText.text = "ID:"+ GameManager.instance.GetCharacterDatabase(characterTypeID).GetCharacterTypeID().ToString();
        levelText.text = "LEVEL:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetLevel().ToString();
        hpText.text ="HP:"+ GameManager.instance.GetCharacterDatabase(characterTypeID).GetMaxHp().ToString();
        atkText.text = "ATK:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetAtk().ToString();
        attackIntervalText.text = "Attack Interval:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetAttackInterval().ToString();
        attackRangeText.text = "Attack Range:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetAttackRange().ToString();
        spdText.text = "SPEED:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetSpd().ToString();
        coolTimeText.text = "Cool Time:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetCoolTime().ToString();
        costText.text = "Cost:"+GameManager.instance.GetCharacterDatabase(characterTypeID).GetCost().ToString();
    }

}

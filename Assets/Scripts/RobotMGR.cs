using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMGR : Unit
{
    public void SetCharacterData(int robotTypeID)  
    {
        autoRoute = GameManager.instance.robotAutoRouteDatas[robotTypeID];
        Debug.LogWarning($"autoRoute:{autoRoute}");
    }




    protected override void CheckIfCauseSkill() { } //何も実装しない


    public override void Die()
    {
        Debug.Log("Robotがやられました");
    }
}

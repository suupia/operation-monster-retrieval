using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMGR : Unit
{
    protected override void CheckIfCauseSkill() { } //何も実装しない


    public override void Die()
    {
        Debug.Log("Robotがやられました");
    }
}

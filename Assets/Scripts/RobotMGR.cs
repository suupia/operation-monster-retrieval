using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMGR : Unit
{
    protected override void CheckIfCauseSkill() { } //�����������Ȃ�


    public override void Die()
    {
        Debug.Log("Robot������܂���");
    }
}

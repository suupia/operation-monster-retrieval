using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMGR : Unit
{
    protected override void CheckIfCauseSkill() { } //‰½‚àŽÀ‘•‚µ‚È‚¢


    public override void Die()
    {
        Debug.Log("Robot‚ª‚â‚ç‚ê‚Ü‚µ‚½");
    }
}

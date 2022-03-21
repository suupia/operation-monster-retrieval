using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMGR : Unit
{
    public void SetRobotData(int robotTypeID)
    {
        autoRoute = GameManager.instance.robotAutoRouteDatas[robotTypeID];
        Debug.Log($"ロボットのautoRoute:{autoRoute}");
    }




    protected override void CheckIfCauseSkill() { } //何も実装しない


    public override void Die()
    {
        Debug.Log($"HPが0以下になったので、ロボットを消去します gridPos:{gridPos},transform.pos{transform.position}のロボット");

        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.robotID); //数値データをを消去する
        GameManager.instance.CurrentRobotNum--;
        GameManager.instance.mapMGR.GetMap().RemoveUnit(gridPos, this);
        //GameManager.instance.mapMGR.GetMap().SetCharacterMGR(gridPos,null); //スクリプトをを消去する

        GameManager.instance.musicMGR.StartCombatSE("Explosion");
        Destroy(this.gameObject);
    }
}

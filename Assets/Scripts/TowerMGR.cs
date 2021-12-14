using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMGR : Facility
{

    public override void Die()
    {
        Debug.Log($"HPが0以下になったので、タワーを破壊します gridPos:{gridPos}のタワー");
        
        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos,GameManager.instance.towerID); //先にデータを消去する

        Destroy(this.gameObject);
    }
}

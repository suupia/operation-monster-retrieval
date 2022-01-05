using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleMGR : Facility
{
    public override void SetDirection(Vector2 directionVector)
    {
        //Castleは向きを変えることはない
    }
    public override void Die()
    {

        Debug.Log($"HPが0以下になったので、城を破壊します gridPos:{gridPos}の城");

        GameManager.instance.StartShowingResults(true);

        Destroy(this.gameObject);
    }
}

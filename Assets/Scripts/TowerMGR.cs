using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMGR : Facility
{

    public override void Destroy()
    {
        Debug.LogWarning($"HP��0�ȉ��ɂȂ����̂ŁA�^���[��j�󂵂܂� gridPos:{gridPos}");
        
        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos,GameManager.instance.towerID); //��Ƀf�[�^����������

        Destroy(this.gameObject);
    }
}

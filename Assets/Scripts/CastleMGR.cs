using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleMGR : Facility
{
    public override void Die()
    {

        Debug.Log($"HP��0�ȉ��ɂȂ����̂ŁA���j�󂵂܂� gridPos:{gridPos}�̏�");

        Destroy(this.gameObject);
    }
}
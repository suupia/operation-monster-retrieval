using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleMGR : Facility
{
    public override void SetDirection(Vector2 directionVector)
    {
        //Castle�͌�����ς��邱�Ƃ͂Ȃ�
    }
    public override void Die()
    {

        Debug.Log($"HP��0�ȉ��ɂȂ����̂ŁA���j�󂵂܂� gridPos:{gridPos}�̏�");

        GameManager.instance.WinTheGame();

        Destroy(this.gameObject);
    }
}

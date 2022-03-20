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




    protected override void CheckIfCauseSkill() { } //�����������Ȃ�


    public override void Die()
    {
        Debug.Log($"HP��0�ȉ��ɂȂ����̂ŁA���{�b�g���������܂� gridPos:{gridPos},transform.pos{transform.position}�̃��{�b�g");

        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.robotID); //���l�f�[�^������������
        GameManager.instance.CurrentRobotNum--;
        GameManager.instance.mapMGR.GetMap().RemoveUnit(gridPos, this);
        //GameManager.instance.mapMGR.GetMap().SetCharacterMGR(gridPos,null); //�X�N���v�g������������


        Destroy(this.gameObject);
    }
}
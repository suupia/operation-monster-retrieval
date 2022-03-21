using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CastleMGR : Facility
{
    //HPGauge�p
    [SerializeField] Image emptyGauge;
    [SerializeField] Image filledGauge;
    [SerializeField] Color emptyGaugeColor;
    [SerializeField] Color filledGaugeColor;

    //�v���p�e�B�̃I�[�o�[���C�h
    public override int HP
    {
        get { return hp; }
        set
        {
            hp = value;

            RefreshGauge((float)hp/maxHp); // (int) / (int)��int�^�ɂȂ�A���̏ꍇ0�ɂȂ邱�Ƃɒ���

            if (hp <= 0)
            {
                Die();
            }

        }
    }

    public override void SetDirection(Vector2 directionVector)
    {
        //Castle�͌�����ς��邱�Ƃ͂Ȃ�
    }
    public override void Die()
    {

        Debug.Log($"HP��0�ȉ��ɂȂ����̂ŁA���j�󂵂܂� gridPos:{gridPos}�̏�");

        if (IsEnemySide)
        {
            //�v���C���[�̏���
            GameManager.instance.StartShowingResults(true);
        }
        else
        {
            //�v���C���[�̕���
            GameManager.instance.StartShowingResults(false);
        }

        GameManager.instance.mapMGR.MakeRoadByTowerDead(gridPos.x,gridPos.y);


        Destroy(this.gameObject);
    }

    public void RefreshGauge(float percentage)
    {
        //Debug.Log($"fillAmount(percentage):{percentage}");

        filledGauge.fillAmount = percentage;
        if (percentage >= 1)
        {
            emptyGauge.color = Color.clear;
            filledGauge.color = Color.clear;
        }
        else
        {
            emptyGauge.color = emptyGaugeColor;
            filledGauge.color = filledGaugeColor;
        }
    }
}

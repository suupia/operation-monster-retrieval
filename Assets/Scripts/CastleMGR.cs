using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CastleMGR : Facility
{
    //HPGauge用
    [SerializeField] Image emptyGauge;
    [SerializeField] Image filledGauge;
    [SerializeField] Color emptyGaugeColor;
    [SerializeField] Color filledGaugeColor;

    //プロパティのオーバーライド
    public override int HP
    {
        get { return hp; }
        set
        {
            hp = value;

            RefreshGauge((float)hp/maxHp); // (int) / (int)はint型になり、この場合0になることに注意

            if (hp <= 0)
            {
                Die();
            }

        }
    }

    public override void SetDirection(Vector2 directionVector)
    {
        //Castleは向きを変えることはない
    }
    public override void Die()
    {

        Debug.Log($"HPが0以下になったので、城を破壊します gridPos:{gridPos}の城");

        if (IsEnemySide)
        {
            //プレイヤーの勝ち
            GameManager.instance.StartShowingResults(true);
        }
        else
        {
            //プレイヤーの負け
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

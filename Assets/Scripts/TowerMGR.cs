using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMGR : Facility
{
    SpriteRenderer spriteRenderer;
    [SerializeField] protected Sprite[] towerSprites; //下、左、上、左下、左上の順番でインスペクター上でセットする

    Direction direction;
    private enum Direction
    {
        Front,
        Back,
        Right,
        Left,
        DiagRightFront,
        DiagLeftFront,
        DiagRightBack,
        DiagLeftBack
    }
    new void Start() //オーバーライド
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void SetDirection(Vector2 directionVector)
    {
        if (directionVector == Vector2.zero) //引数の方向ベクトルがゼロベクトルの時は何もしない
        {
            return;
        }

        float angle = Vector2.SignedAngle(Vector2.right, directionVector);
        //Debug.Log($"SetDirectionのangleは{angle}です");


        //先に画像の向きを決定する
        if (directionVector.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); //元の画像が左向きのため
        }
        else if (directionVector.x <= 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        //directionとanimationを決定する
        if (-22.5f <= angle && angle < 22.5f)
        {
            direction = Direction.Right;
            spriteRenderer.sprite = towerSprites[1];
        }
        else if (22.5f <= angle && angle < 67.5f)
        {
            direction = Direction.DiagRightBack;
            spriteRenderer.sprite = towerSprites[4];

        }
        else if (67.5f <= angle && angle < 112.5f)

        {
            direction = Direction.Back;
            spriteRenderer.sprite = towerSprites[2];

        }
        else if (112.5f <= angle && angle < 157.5f)
        {
            direction = Direction.DiagLeftBack;
            spriteRenderer.sprite = towerSprites[4];

        }
        else if (-157.5f <= angle && angle < -112.5f)
        {
            direction = Direction.DiagLeftFront;
            spriteRenderer.sprite = towerSprites[3];

        }
        else if (-112.5f <= angle && angle < -67.5f)
        {
            direction = Direction.Front;
            spriteRenderer.sprite = towerSprites[0];

        }
        else if (-67.5f <= angle && angle < -22.5f)
        {
            direction = Direction.DiagRightFront;
            spriteRenderer.sprite = towerSprites[3];

        }
        else //角度は-180から180までで端点は含まないらしい。そのため、Direction.Leftはelseで処理することにした。
        {
            direction = Direction.Left;
            spriteRenderer.sprite = towerSprites[1];

        }
    }

    public override void Die()
    {
        Debug.Log($"HPが0以下になったので、タワーを破壊します gridPos:{gridPos}のタワー");
        
        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos,GameManager.instance.facilityID); //先にデータを消去する

        Destroy(this.gameObject);
    }
}

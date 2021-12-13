using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTailMGR : MonoBehaviour
{
    private int pointerTailIndex;
    // Start is called before the first frame update
    void Start()
    {
        pointerTailIndex = GameManager.instance.pointerMGR.GetPoinerTails().IndexOf(gameObject);    //pointerTailsにPointeTailが順番に仕舞われているので、Indexを取得しておく
    }

    private void LateUpdate()
    {
        if(pointerTailIndex < GameManager.instance.pointerMGR.GetPoinerTails().Count - 3)      //このPointerTailがある程度前のものだった場合、処理は行わない
        {
            return;
        }
        RotatePointerTail();
    }
    private void RotatePointerTail()
    {
        int angle = 0;

        if (GameManager.instance.pointerMGR.GetManualRoute().Count >= pointerTailIndex + 2 && GameManager.instance.pointerMGR.GetPoinerTails().Count > pointerTailIndex + 1)       //pointerTailがこれ自身の後ろに存在する場合の処理
        {
            if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.up + Vector2Int.right == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 2] && 
                GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
            {
                angle = 45;
            }
            else if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.up + Vector2Int.left == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 2] &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
            {
                angle = 135;
            }
            else if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.down + Vector2Int.right == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 2] &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
            {
                angle = -45;
            }
            else if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.down + Vector2Int.left == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 2] &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
            {
                angle = -135;
            }

            //斜めを向く場合、ここまででangleが0以外に定まる
            if (angle != 0)
            {
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.localScale = new Vector3(Mathf.Sqrt(2), 1, 1);                //斜めを向くときは長さをsqrt2にする
                GameManager.instance.pointerMGR.GetPoinerTails()[pointerTailIndex + 1].SetActive(false);      //これを斜めに向かせる場合、この次のPointerTailは表示する必要がないので、非アクティブにする
                                                                                                              //大丈夫だと思うけど、この次のPointerTailのUpdateが先に呼ばれると困る気がするので、一応メモとして
                return;    //斜めを向いたときはここでreturnされる
            }
            else
            {
                GameManager.instance.pointerMGR.GetPoinerTails()[pointerTailIndex + 1].SetActive(true);        //これが斜めを向かないときは、次のPointerTailをアクティブにする
            }
        }


        if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].x == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].x)      //以下、縦横を向かせる
        {
            if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].y < GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].y)
            {
                angle = 90;
            }
            else
            {
                angle = -90;

            }
        }
        else if(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].y == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].y)
        {
            if(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].x < GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].x)
            {
                angle = 0;
            }
            else
            {
                angle = 180;
            }
        }
        transform.localScale = new Vector3(1, 1, 1);        //斜めを向かないときはscaleは長さを1に

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }
}

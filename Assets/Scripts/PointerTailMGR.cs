using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTailMGR : MonoBehaviour
{
    private int pointerTailIndex;
    private List<Vector2Int> manualRoute;
    private List<GameObject> pointerTails;
    private MapData map;

    [SerializeField] bool nonDiagonal;     //これがtrueのとき、PointerTailは斜めを向かない

    //プロパティ
    public bool NonDiagonal
    {
        get { return nonDiagonal; }
        set { nonDiagonal = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        pointerTails = GameManager.instance.pointerMGR.GetPoinerTails();
        pointerTailIndex = pointerTails.IndexOf(gameObject);    //pointerTailsにPointeTailが順番に仕舞われているので、Indexを取得しておく
        manualRoute = GameManager.instance.pointerMGR.GetManualRoute();
        map = GameManager.instance.mapMGR.GetMap();
    }

    private void LateUpdate()
    {
        if (pointerTailIndex < GameManager.instance.pointerMGR.GetPoinerTails().Count - 4)      //このPointerTailがある程度前のものだった場合、処理は行わない
        {
            return;
        }
        RotatePointerTail();
    }
    private void RotatePointerTail()
    {
        int angle = 0;

        ManageNonDiagonal();

        if (manualRoute.Count >= pointerTailIndex + 2 && GameManager.instance.pointerMGR.GetPoinerTails().Count > pointerTailIndex + 1)       //pointerTailがこれ自身の後ろに存在する場合の処理
        {
            if (!nonDiagonal)
            {
                if (manualRoute[pointerTailIndex] + Vector2Int.up + Vector2Int.right == manualRoute[pointerTailIndex + 2] &&
                    map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                    map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
                {
                    angle = 45;
                }
                else if (manualRoute[pointerTailIndex] + Vector2Int.up + Vector2Int.left == manualRoute[pointerTailIndex + 2] &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
                {
                    angle = 135;
                }
                else if (manualRoute[pointerTailIndex] + Vector2Int.down + Vector2Int.right == manualRoute[pointerTailIndex + 2] &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
                {
                    angle = -45;
                }
                else if (manualRoute[pointerTailIndex] + Vector2Int.down + Vector2Int.left == manualRoute[pointerTailIndex + 2] &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
                {
                    angle = -135;
                }
            }
            //斜めを向く場合、ここまででangleが0以外に定まる

            if (angle != 0)
            {
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.localScale = new Vector3(Mathf.Sqrt(2), 1, 1);                //斜めを向くときは長さをsqrt2にする
                    pointerTails[pointerTailIndex + 1].SetActive(false);      //これを斜めに向かせる場合、この次のPointerTailは表示する必要がないので、非アクティブにする
                                                                                                              //大丈夫だと思うけど、この次のPointerTailのUpdateが先に呼ばれると困る気がするので、一応メモとして
                return;    //斜めを向いたときはここでreturnされる
            }
            else
            {
                    pointerTails[pointerTailIndex + 1].SetActive(true);        //これが斜めを向かないときは、次のPointerTailをアクティブにする
            }
        }


        if (manualRoute[pointerTailIndex].x == manualRoute[pointerTailIndex + 1].x)      //以下、縦横を向かせる
        {
            if (manualRoute[pointerTailIndex].y < manualRoute[pointerTailIndex + 1].y)
            {
                angle = 90;
            }
            else
            {
                angle = -90;

            }
        }
        else if(manualRoute[pointerTailIndex].y == manualRoute[pointerTailIndex + 1].y)
        {
            if(manualRoute[pointerTailIndex].x < manualRoute[pointerTailIndex + 1].x)
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

    private void ManageNonDiagonal()
    {

        if (pointerTailIndex >= 2 && manualRoute.Count >= pointerTailIndex + 3)           //最小の回転を含むときの処理　このPointerTailが回転に含まれる5点のうち3点目のときに処理を行う
        {
            if (manualRoute[pointerTailIndex + 2] == manualRoute[pointerTailIndex - 2])
            {
                for (int i = 0; i < 3; i++)
                {
                    pointerTails[pointerTailIndex - i].SetActive(true);              //斜めになっているPointerTailの次のPointerTailは非アクティブにしてあるので、アクティブにする
                    pointerTails[pointerTailIndex - i].GetComponent<PointerTailMGR>().NonDiagonal = true;
                }
            }
        }
        else if (pointerTailIndex >= 2 && manualRoute.Count == pointerTailIndex + 2 && NonDiagonal &&
                 pointerTails[pointerTailIndex - 1].GetComponent<PointerTailMGR>().NonDiagonal &&
                 pointerTails[pointerTailIndex - 2].GetComponent<PointerTailMGR>().NonDiagonal)        //このPointerTailが最小の回転に含まれる3点目かつ、PointerがこのPointerTailの次のマスにあるとき(回転になっていないとき)
        {
            if (pointerTailIndex >= 3 && pointerTails[pointerTailIndex - 3].GetComponent<PointerTailMGR>().NonDiagonal)
            {
                NonDiagonal = false;
                return;
            }
            for (int i = 2; i >= 0; i--)
            {
                pointerTails[pointerTailIndex - i].GetComponent<PointerTailMGR>().NonDiagonal = false;      //回転になっていないので、斜めを向けるようにする(一応PointerTailが置かれた順に処理を行う)
                if (i == 1)
                {
                    pointerTails[pointerTailIndex - i].SetActive(false);              //斜め表示に戻す際、このPointerTailの一つ前のPointerTailは非アクティブにする
                                                                                      //別のところで行われる処理だが、他の処理との順番がかみ合わないかったのでここに書いておく
                                                                                      //多分大丈夫だけど、応急処置的にも見えるので後々問題が起こるかも
                }
            }
        }

    }
}

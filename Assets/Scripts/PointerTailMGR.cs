using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTailMGR : MonoBehaviour
{
    [SerializeField] int pointerTailIndex;
    private List<Vector2Int> manualRoute;
    private List<GameObject> pointerTails;
    private MapData map;
    private int numOfChildren;
    private GameObject[] children;
    private SpriteRenderer[] childRenderers;
    private Vector2[] defaultPointerTailSizes;
    private Color noChangedColor;
    [SerializeField] Color changedColor;
    [SerializeField] Sprite[] firstTailSprites;
    [SerializeField] bool isCrossing;             //これがtrueのときは終点のSpriteをchangedColorにする

    private int horizontal1Index = 0;
    private int horizontal2Index = 1;
    private int diagonal1Index = 2;
    private int diagonal2Index = 3;

    [SerializeField] bool nonDiagonal;     //これがtrueのとき、PointerTailは斜めを向けないようにする（最小の回転用）
    [SerializeField] bool isDiagonal;      //斜めを向いてるとき、true

    //プロパティ
    public bool NonDiagonal
    {
        get { return nonDiagonal; }
        set { nonDiagonal = value; }
    }

    public bool GetIsDiagonal()
    {
        return isDiagonal;
    }
    public void SetIsDiagonal(bool b)
    {
        isDiagonal = b;
    }

    public bool GetIsCrossing()
    {
        return isCrossing;
    }

    public void SetIsCrossing(bool b)
    {
        isCrossing = b;
    }
    // Start is called before the first frame update
    void Start()
    {
        pointerTails = GameManager.instance.pointerMGR.GetPoinerTails();
        pointerTailIndex = pointerTails.IndexOf(gameObject);    //pointerTailsにPointeTailが順番に仕舞われているので、Indexを取得しておく
        manualRoute = GameManager.instance.pointerMGR.GetManualRoute();
        map = GameManager.instance.mapMGR.GetMap();

        numOfChildren = 4;
        children = new GameObject[numOfChildren];
        childRenderers = new SpriteRenderer[numOfChildren];
        defaultPointerTailSizes = new Vector2[numOfChildren];
        for (int i = 0; i < numOfChildren; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            childRenderers[i] = children[i].GetComponent<SpriteRenderer>();
            defaultPointerTailSizes[i] = childRenderers[i].size;
            childRenderers[i].sortingOrder = pointerTailIndex;
        }
        noChangedColor = childRenderers[0].color;      //Inspectorの方で全子オブジェクトの色を統一して置く

        if(pointerTailIndex == 0)        //一番最初のPointerTailは始点を表示するのでSpriteを変える
        {
            childRenderers[horizontal1Index].sprite = firstTailSprites[0];
            childRenderers[diagonal1Index].sprite = firstTailSprites[1];
            children[horizontal2Index].SetActive(false);
            children[diagonal2Index].SetActive(false);
        }
    }

    private void LateUpdate()
    {
        ChangeColor();
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
                transform.rotation = Quaternion.AngleAxis(angle-45, Vector3.forward);
                ChangeToDiagonal(true);                                   //斜めのSpriteを表示する
                pointerTails[pointerTailIndex + 1].SetActive(false);      //これを斜めに向かせる場合、この次のPointerTailは表示する必要がないので、非アクティブにする
                                                                          //大丈夫だと思うけど、この次のPointerTailのUpdateが先に呼ばれると困る気がするので、一応メモとして
                SetIsDiagonal(true);
                return;    //斜めを向いたときはここでreturnされる
            }
            else
            {
                pointerTails[pointerTailIndex + 1].SetActive(true);        //これが斜めを向かないときは、次のPointerTailをアクティブにする
                SetIsDiagonal(false);
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
        ChangeToDiagonal(false);                 //SpriteをHorizontalにする
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
                    if(i == 2 && !pointerTails[pointerTailIndex - i].GetComponent<PointerTailMGR>().isDiagonal) { break; }    //一番目の点が斜めになっていないとき(=0番目の点が斜めのとき)は一番目の点はそのままにする
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

    private void ChangeToDiagonal(bool b)
    {
        //斜めにする 
        if (b)
        {
            children[horizontal1Index].SetActive(false);
            children[horizontal2Index].SetActive(false);
            children[diagonal1Index].SetActive(true);
            children[diagonal2Index].SetActive(true);
        }else  //横にする
        {
            children[horizontal1Index].SetActive(true);
            children[horizontal2Index].SetActive(true);
            children[diagonal1Index].SetActive(false);
            children[diagonal2Index].SetActive(false);
        }

    }

    private void ChangeColor()
    {
        ManageIsCrossing();
        for (int i = 0; i < manualRoute.Count; i++)
        {
            Vector2Int m = manualRoute[i];
            if(m.x == manualRoute[pointerTailIndex].x && m.y == manualRoute[pointerTailIndex].y && i < pointerTailIndex && pointerTails[i].activeSelf)       //PointerTailが交差したとき
            {
                if (Vector3.Angle(pointerTails[i].transform.rotation.eulerAngles, transform.rotation.eulerAngles) % 180 == 0 && pointerTails[i].GetComponent<PointerTailMGR>().GetIsDiagonal() == GetIsDiagonal())    //PointerTailが重なったとき
                {
                    foreach (SpriteRenderer sr in childRenderers)        //自分自身が重なっているときは全子オブジェクトをchangedColorにする
                    {
                        sr.color = changedColor;
                    }
                    return;
                }
            }
        }

        childRenderers[horizontal1Index].color = noChangedColor;            //isCrossingのときはHorizotal2, Diagonal2はchangedColorにしたいので、ここではいじらない
        childRenderers[diagonal1Index].color = noChangedColor;
    }

    private void ManageIsCrossing()
    {
        if (GetIsCrossing())
        {
            childRenderers[horizontal2Index].color = changedColor;
            childRenderers[diagonal2Index].color = changedColor;
        }
        else          //Horizontal2, Diagonal2をnoChangedColorにするのはここだけ
        {
            childRenderers[horizontal2Index].color = noChangedColor;
            childRenderers[diagonal2Index].color = noChangedColor;
        }

        for (int i = 0; i < manualRoute.Count; i++)
        {
            if (i >= pointerTailIndex) break;    //Thisより手前のPointerTailと重なっているかが知りたいので

            Vector2Int m = manualRoute[i];
            if (m.x == manualRoute[pointerTailIndex].x && m.y == manualRoute[pointerTailIndex].y && pointerTails[i].activeSelf)       //PointerTailが交差したとき
            {
                Debug.LogWarning("isCrossing:" + transform.position);
                if (pointerTails[pointerTailIndex - 1].activeSelf)       //一つ前のPointerTailがactiveのとき
                {
                    pointerTails[pointerTailIndex - 1].GetComponent<PointerTailMGR>().SetIsCrossing(true);
                }
                else         //一つ前のPointerTailがactiveでないとき（＝二つ前の点と斜めで繋がっているとき）
                {
                    pointerTails[pointerTailIndex - 2].GetComponent<PointerTailMGR>().SetIsCrossing(true);
                }
                return;
            }
        }
        if (pointerTailIndex > 0)
        {
            pointerTails[pointerTailIndex - 1].GetComponent<PointerTailMGR>().SetIsCrossing(false);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTailMGR : MonoBehaviour
{
    [SerializeField] int pointerTailIndex;
    private List<Vector2Int> subjectRoute;
    private List<GameObject> pointerTails;
    private MapData map;
    private int numOfChildren;
    private GameObject[] children;
    private SpriteRenderer[] childSprites;
    private Vector2[] defaultPointerTailSizes;
    [SerializeField] private Color noChangedColor;
    [SerializeField] Color changedColor;
    [SerializeField] Sprite[] firstTailSprites;
    [SerializeField] bool isCrossing;             //これがtrueのときは終点のSpriteをchangedColorにする

    private int horizontal1Index = 0;
    private int horizontal2Index = 1;
    private int diagonal1Index = 2;
    private int diagonal2Index = 3;

    [SerializeField] bool nonDiagonal;     //これがtrueのとき、PointerTailは斜めを向けないようにする（最小の回転用）
    [SerializeField] bool isDiagonal;      //斜めを向いてるとき、true
    //[SerializeField] public bool forComplement;      //このPoitnerTailが補完用のときtrue

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

        if (GameManager.instance.inputMGR.isEditingManualRoute) //ManualRoute編集中
        {
            pointerTails = GameManager.instance.pointerMGR.GetPointerTails();
            pointerTailIndex = pointerTails.IndexOf(gameObject);    //pointerTailsにPointeTailが順番に仕舞われているので、Indexを取得しておく
            subjectRoute = GameManager.instance.pointerMGR.GetManualRoute();
        }
        else　if (GameManager.instance.copyingManualRoute) //ManualRouteコピー中に呼ばれたとき
        {
            pointerTails = GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetPointerTails();
            pointerTailIndex = pointerTails.IndexOf(gameObject);    //pointerTailsにPointeTailが順番に仕舞われているので、Indexを取得しておく
            subjectRoute = GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetNonDiagonalManualRoute();

            //色を少し透明にする
            noChangedColor.a *= 0.5f;
            changedColor.a *= 0.5f;
        }

        map = GameManager.instance.mapMGR.GetMap();

        numOfChildren = 4;
        children = new GameObject[numOfChildren];
        childSprites = new SpriteRenderer[numOfChildren];
        defaultPointerTailSizes = new Vector2[numOfChildren];
        for (int i = 0; i < numOfChildren; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            childSprites[i] = children[i].GetComponent<SpriteRenderer>();
            defaultPointerTailSizes[i] = childSprites[i].size;
            childSprites[i].sortingOrder = pointerTailIndex;
        }

        if(pointerTailIndex == 0)        //一番最初のPointerTailは始点を表示するのでSpriteを変える
        {
            childSprites[horizontal1Index].sprite = firstTailSprites[0];
            childSprites[diagonal1Index].sprite = firstTailSprites[1];
            children[horizontal2Index].SetActive(false);
            children[diagonal2Index].SetActive(false);
        }

        foreach(SpriteRenderer sr in childSprites) //色を初期化
        {
            sr.color = noChangedColor;
        }
    }

    private void LateUpdate()
    {
        ChangeColor();

        //if (pointerTailIndex < GameManager.instance.pointerMGR.GetPoinerTails().Count - 4)      //このPointerTailがある程度前のものだった場合、処理は行わない→一瞬でたくさんPointerTailが生成されたときに困るのでなし
        //{
        //    return;
        //}

        RotatePointerTail();
    }
    private void RotatePointerTail()
    {
        int angle = 0;

        ManageNonDiagonal();

        bool pointerTailExistAfterThis = false;
        if (GameManager.instance.inputMGR.isEditingManualRoute)
        {
            if (subjectRoute.Count >= pointerTailIndex + 2 && GameManager.instance.pointerMGR.GetPointerTails().Count > pointerTailIndex + 1) pointerTailExistAfterThis = true;
        }
        else if (GameManager.instance.copyingManualRoute)
        {
            if (subjectRoute.Count >= pointerTailIndex + 2 && GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].GetPointerTails().Count > pointerTailIndex + 1) pointerTailExistAfterThis = true;
        }

        if (pointerTailExistAfterThis)       //pointerTailがこれ自身の後ろに存在する場合の処理
        {
            if (!nonDiagonal)
            {
                if (subjectRoute[pointerTailIndex] + Vector2Int.up + Vector2Int.right == subjectRoute[pointerTailIndex + 2] &&
                    map.GetValue(subjectRoute[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                    map.GetValue(subjectRoute[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
                {
                    angle = 45;
                }
                else if (subjectRoute[pointerTailIndex] + Vector2Int.up + Vector2Int.left == subjectRoute[pointerTailIndex + 2] &&
                         map.GetValue(subjectRoute[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                         map.GetValue(subjectRoute[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
                {
                    angle = 135;
                }
                else if (subjectRoute[pointerTailIndex] + Vector2Int.down + Vector2Int.right == subjectRoute[pointerTailIndex + 2] &&
                         map.GetValue(subjectRoute[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                         map.GetValue(subjectRoute[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
                {
                    angle = -45;
                }
                else if (subjectRoute[pointerTailIndex] + Vector2Int.down + Vector2Int.left == subjectRoute[pointerTailIndex + 2] &&
                         map.GetValue(subjectRoute[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                         map.GetValue(subjectRoute[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
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
            }
        }

         SetIsDiagonal(false);        //これ以降の処理は斜めを向かないときに行われるため、ここでisDiagonalをfalseに

        if (subjectRoute[pointerTailIndex].x == subjectRoute[pointerTailIndex + 1].x)      //以下、縦横を向かせる
        {
            if (subjectRoute[pointerTailIndex].y < subjectRoute[pointerTailIndex + 1].y)
            {
                angle = 90;
            }
            else
            {
                angle = -90;

            }
        }
        else if(subjectRoute[pointerTailIndex].y == subjectRoute[pointerTailIndex + 1].y)
        {
            if(subjectRoute[pointerTailIndex].x < subjectRoute[pointerTailIndex + 1].x)
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

        if (pointerTailIndex >= 2 && subjectRoute.Count >= pointerTailIndex + 3)           //最小の回転を含むときの処理　このPointerTailが回転に含まれる5点のうち3点目のときに処理を行う
        {
            if (subjectRoute[pointerTailIndex + 2] == subjectRoute[pointerTailIndex - 2])
            {
                for (int i = 0; i < 3; i++)
                {
                    if(i == 2 && !pointerTails[pointerTailIndex - i].GetComponent<PointerTailMGR>().isDiagonal) { break; }    //一番目の点が斜めになっていないとき(=0番目の点が斜めのとき)は一番目の点はそのままにする
                    pointerTails[pointerTailIndex - i].SetActive(true);              //斜めになっているPointerTailの次のPointerTailは非アクティブにしてあるので、アクティブにする
                    pointerTails[pointerTailIndex - i].GetComponent<PointerTailMGR>().NonDiagonal = true;
                }
            }
        }
        else if (pointerTailIndex >= 2 && subjectRoute.Count == pointerTailIndex + 2 && NonDiagonal &&
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
        for (int i = 0; i < subjectRoute.Count; i++)
        {
            Vector2Int m = subjectRoute[i];
            if(m.x == subjectRoute[pointerTailIndex].x && m.y == subjectRoute[pointerTailIndex].y && i < pointerTailIndex && pointerTails[i].activeInHierarchy)       //PointerTailが交差したとき
            {
                if ((pointerTails[i].transform.rotation.eulerAngles.z - transform.rotation.eulerAngles.z) % 360 == 0
                    && pointerTails[i].GetComponent<PointerTailMGR>().GetIsDiagonal() == GetIsDiagonal())    //PointerTailが同じ向きで重なったとき
                {
                    //Debug.LogWarning("Index:"+pointerTailIndex+",manualIndex:"+ i +",Angle:"+ Vector3.Angle(pointerTails[i].transform.rotation.eulerAngles, transform.rotation.eulerAngles));
                    foreach (SpriteRenderer sr in childSprites)        //自分自身が重なっているときは全子オブジェクトをchangedColorにする
                    {
                        sr.color = changedColor;
                    }
                    return;
                }
            }
        }

        childSprites[horizontal1Index].color = noChangedColor;            //isCrossingのときはHorizotal2, Diagonal2はchangedColorにしたいので、ここではいじらない
        childSprites[diagonal1Index].color = noChangedColor;
    }

    private void ManageIsCrossing()
    {
        if (GetIsCrossing())
        {
            childSprites[horizontal2Index].color = changedColor;
            childSprites[diagonal2Index].color = changedColor;
        }
        else          //Horizontal2, Diagonal2をnoChangedColorにするのはここだけ
        {
            childSprites[horizontal2Index].color = noChangedColor;
            childSprites[diagonal2Index].color = noChangedColor;
        }

        for (int i = 0; i < subjectRoute.Count; i++)
        {
            if (i >= pointerTailIndex) break;    //Thisより手前のPointerTailと重なっているかが知りたいので

            Vector2Int m = subjectRoute[i];
            if (m.x == subjectRoute[pointerTailIndex].x && m.y == subjectRoute[pointerTailIndex].y && pointerTails[i].activeInHierarchy)       //PointerTailが交差したとき
            {
                //Debug.LogWarning("isCrossing:" + transform.position);
                if (pointerTails[pointerTailIndex - 1].activeInHierarchy)       //一つ前のPointerTailがactiveのとき
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

    private void IsOverlapWith(Vector2Int manualPoint, int index)       //与えられた座標のPointerTailと、このPointerTailが重なっているかどうか判定する
    {
        if (manualPoint.x == subjectRoute[pointerTailIndex].x && manualPoint.y == subjectRoute[pointerTailIndex].y)      //与えられた点と自身の点の座標が等しいとき
        {
            if (pointerTails[index].GetComponent<PointerTailMGR>().GetIsDiagonal())        //与えられた座標のPointerTailが斜めのとき
            {
                if (GetIsDiagonal() && subjectRoute[index + 2].x == subjectRoute[pointerTailIndex + 2].x && subjectRoute[index + 2].y == subjectRoute[pointerTailIndex + 2].y)      //同じ向きで重なるとき
                {
                    SetIsCrossing(true);
                    return;
                }
                else if (pointerTails[pointerTailIndex - 2].GetComponent<PointerTailMGR>().GetIsDiagonal() && 
                    subjectRoute[index + 2].x == subjectRoute[pointerTailIndex - 2].x && 
                    subjectRoute[index + 2].y == subjectRoute[pointerTailIndex - 2].y)         //逆向きで重なるとき(逆向きなので、一つ前のPointerTailをisCrossingにする)
                {
                    pointerTails[pointerTailIndex - 2].GetComponent<PointerTailMGR>().SetIsCrossing(true);
                    return;
                }
            }
            else        //与えられた座標のPointerTailが縦or横のとき
            {
                if (subjectRoute[index + 1].x == subjectRoute[pointerTailIndex + 1].x && subjectRoute[index + 1].y == subjectRoute[pointerTailIndex + 1].y)             //同じ向きで重なるとき
                {
                    SetIsCrossing(true);
                    return;
                }
                else if (subjectRoute[index + 1].x == subjectRoute[pointerTailIndex - 1].x && subjectRoute[index + 1].y == subjectRoute[pointerTailIndex - 1].y)     //逆向きで重なるとき(ないけど、一応)
                {
                    pointerTails[pointerTailIndex - 1].GetComponent<PointerTailMGR>().SetIsCrossing(true);
                    return;
                }
            }
        }
    }
}

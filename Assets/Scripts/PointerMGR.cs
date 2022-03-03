using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PointerMGR : MonoBehaviour
{
    //private float moveInterval = 0.3f;
    //private float lastMovementTime;
    [SerializeField] private GameObject pointerTailPrefab;
    private List<GameObject> pointerTails;
    private List<PointerTailMGR> pointerTailMGRs;
    private List<int> nonDiagonalPoints;

    [SerializeField] List<Vector2Int> manualRoute;  //斜めに進むところを斜めにしない状態のRouteを持たせる
    [SerializeField] List<Vector2Int> finalManualRoute;  //斜めに進むところは斜めにした状態のRouteを持たせる
    private bool isOnCastle;

    private Vector3 startPos;

    private void Start()
    {
        manualRoute = new List<Vector2Int>();
        finalManualRoute = new List<Vector2Int>();
        pointerTails = new List<GameObject>();
        nonDiagonalPoints = new List<int>();
        startPos = GameManager.instance.ToWorldPosition(GameManager.instance.mapMGR.GetAllysCastlePos());

        manualRoute.Add(GameManager.instance.mapMGR.GetAllysCastlePos());

    }

    private void Update()
    {
        if (transform.position != startPos && GameManager.instance.state == GameManager.State.ShowingResults)      //戦闘終了時の処理
        {
            ResetPointer();
        }
    }

    //public void MoveByArrowKey(Vector2Int directionVector) //矢印キーで移動
    //{
    //    //positionをグリッドに合わせる
    //    Vector2Int gridPos = GameManager.instance.ToGridPosition(transform.position); //Updateで呼び出されるので毎回更新される
    //    transform.position = GameManager.instance.ToWorldPosition(gridPos);


    //    //arrowKeyFlagを変更する時に毎回ベクトルを割り当てているので、引数でもらったベクトルで入力の状況を判定できる。
    //    if (directionVector != Vector2Int.zero) //矢印キーを押したとき
    //    {
    //        if (GameManager.instance.inputMGR.ArrowKeyTimer - lastMovementTime >= moveInterval)
    //        {
    //            if (!(manualRoute.Count(pos => pos == gridPos + directionVector) < 2) && manualRoute[manualRoute.Count - 2] != gridPos + directionVector) //進もうとしているマスを既に2回以上通っているかつ、来た道を戻らない場合はなにもしない
    //            {
    //                Debug.LogWarning("同じマスを通れるのは2回までです");
    //                return;
    //            }

    //            if (GameManager.instance.mapMGR.GetMapValue(gridPos + directionVector) % GameManager.instance.groundID == 0)
    //            {
    //                transform.position = transform.position + new Vector3(directionVector.x, directionVector.y, 0);
    //                lastMovementTime = GameManager.instance.inputMGR.ArrowKeyTimer;
    //            }

    //        }
    //    }
    //    else //矢印キーを離したとき
    //    {
    //        //GameManager.instance.inputMGR.ArrowKeyTimer = 0;

    //        //ResetLastMovementTime();

    //    }

    //    ManagePlayerRouteList();
    //    ManageMouseTrails();
    //}

    //Getter
    public bool GetIsOnCastle()
    {
        return isOnCastle;
    }
    public List<Vector2Int> GetManualRoute()
    {
        return manualRoute;
    }

    public List<GameObject> GetPoinerTails()
    {
        return pointerTails;
    }
    public List<int> GetNonDiagonalPoints()
    {
        return nonDiagonalPoints;
    }

    public List<Vector2Int> GetFinalManualRoute()
    {
        return finalManualRoute;
    }
    public void SetFinalManualRoute()
    {
        finalManualRoute.Clear();
        nonDiagonalPoints.Clear();
        for (int i = 0; i < manualRoute.Count; i++)
        {
            if (i + 1 != manualRoute.Count)
            {
                if (pointerTails[i].activeSelf)
                {
                    finalManualRoute.Add(manualRoute[i]);

                    if (pointerTails[i].GetComponent<PointerTailMGR>().NonDiagonal)
                    {
                        nonDiagonalPoints.Add(i);
                    }
                }
            }
            else
            {
                finalManualRoute.Add(manualRoute[i]);
            }
        }
    }

    public void MoveByMouse(Vector2Int mouseGridPos) //マウスで移動
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position); //Updateで呼び出されるので毎回更新される

        SetFinalManualRoute();
        if ((mouseGridPos - pointerGridPos).magnitude > 1)       //mouseとpointerが隣接していない場合はなにもしない
        {
            Debug.Log($"縦横のいずれかで隣接しているマスを選んでください　(mouseGridPos - pointerGridPos).magnitude:{(mouseGridPos - pointerGridPos).magnitude}");
            return;
        }

        if (finalManualRoute.Count(pos => pos == mouseGridPos) >= 2 && manualRoute[manualRoute.Count - 2] != mouseGridPos) //進もうとしているマスを既に2回以上通っていて、来た道を戻らない場合はreturn
        {
            Debug.Log("同じマスを通れるのは2回までです");
            return;
        }

        if (finalManualRoute.Count(pos => pos == pointerGridPos) >= 2 && finalManualRoute[0] != pointerGridPos && finalManualRoute[finalManualRoute.IndexOf(pointerGridPos) - 1] == mouseGridPos && manualRoute[manualRoute.Count - 2] != mouseGridPos)    //前に通った道(縦or横)を逆向きに進もうとしていて、来た道を戻らない場合はreturn
        {
            Debug.LogWarning("既にPointerTailが表示されているマスを逆向き(縦or横)に進むことはできません");
            return;
        }

        if (manualRoute.Count >= 2 && finalManualRoute.Count(pos => pos == manualRoute[manualRoute.Count - 2]) >= 2 && finalManualRoute[0] != manualRoute[manualRoute.Count - 2] && finalManualRoute[finalManualRoute.IndexOf(manualRoute[manualRoute.Count - 2]) - 1] == mouseGridPos && manualRoute[manualRoute.Count - 2] != mouseGridPos)    //前に通った道(縦or横)を逆向きに進もうとしていて、来た道を戻らない場合はreturn
        {
            Debug.LogWarning("既にPointerTailが表示されているマスを逆向き(斜め)に進むことはできません");
            return;
        }

        if (GameManager.instance.mapMGR.GetMapValue(mouseGridPos) % GameManager.instance.groundID != 0)
        {
            Debug.Log("groundじゃないので移動不可");
            return;           //groundじゃないので移動不可
        }
        transform.position = GameManager.instance.ToWorldPosition(mouseGridPos);       //Pointerを移動


        IsOnCastle();
        ManageManualRouteList();
        ManageMPointerTails();
        ManageIsCrossing();
    }
    private void ManageManualRouteList()
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position);

        if (manualRoute.Count >= 2 && manualRoute[manualRoute.Count - 2] == pointerGridPos)     //Pointerが一つ戻ったらrouteもそれに合わせる
        {
            manualRoute.RemoveAt(manualRoute.Count - 1);
            Debug.Log($"manualRoute:{string.Join(",", manualRoute)}");
        }
        else if (manualRoute.Count == 0 || (manualRoute[manualRoute.Count - 1] != pointerGridPos && finalManualRoute.Count(pos => pos == pointerGridPos) < 2))           //finalManualRouteに現在のgridPosが2つ以上入っていない場合、manualRouteに追加する
        {
            manualRoute.Add(pointerGridPos);
            Debug.Log($"manualRoute:{string.Join(",", manualRoute)}");
        }
    }


    private void ManageMPointerTails()
    {
        while (manualRoute.Count - 1 > pointerTails.Count)
        {
            pointerTails.Add(Instantiate(pointerTailPrefab, GameManager.instance.ToWorldPosition(manualRoute[pointerTails.Count]), pointerTailPrefab.transform.rotation));
            //pointerTailMGRs.Add(pointerTails[pointerTails.Count - 1].GetComponent<PointerTailMGR>());
            SetOrder();
        }

        while (manualRoute.Count - 1 < pointerTails.Count)
        {
            Destroy(pointerTails[pointerTails.Count - 1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
            //pointerTailMGRs.RemoveAt(pointerTails.Count - 1);
        }
    }

    private void IsOnCastle()
    {
        if ((int)transform.position.x >= GameManager.instance.mapMGR.GetEnemysCastlePos().x - 1 && (int)transform.position.y <= GameManager.instance.mapMGR.GetEnemysCastlePos().y + 1)
        {
            Debug.LogWarning("PointerがCastleに到達しました");
            isOnCastle = true;
        }
        else
        {
            isOnCastle = false;
        }
        return;
    }

    public void ResetPointer()
    {
        manualRoute.Clear();
        finalManualRoute = new List<Vector2Int>(); //ManualRouteに参照を渡しているので、次に使うためにnewする必要がある
        nonDiagonalPoints = new List<int>(); //同上
        isOnCastle = false;
        transform.position = startPos;
        while (pointerTails.Count != 0)
        {
            Destroy(pointerTails[pointerTails.Count - 1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
        }
        manualRoute.Add(GameManager.instance.mapMGR.GetAllysCastlePos());
        Debug.Log($"Pointerを初期化:manualRoute={string.Join(",", manualRoute)}, isOnCastle={isOnCastle}, pointerTails={string.Join(",", pointerTails)}");
    }

    private void SetOrder()
    {
        this.GetComponent<SpriteRenderer>().sortingOrder = pointerTails.Count + 1;
    }

    private void ManageIsCrossing()
    {
        if (pointerTails.Count < 1) return;      //最初の点は必要ない
        int lastPointerTailIndex = pointerTails.Count - 1;
        if (!pointerTails[lastPointerTailIndex].activeSelf)
        {
            lastPointerTailIndex--;
        }

        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position);

        for (int i = 0; i < manualRoute.Count; i++)
        {
            if (i >= manualRoute.Count - 1) break;    //前に通った点かどうか知りたいので

            Vector2Int m = manualRoute[i];
            if (m.x == pointerGridPos.x && m.y == pointerGridPos.y)       //Poniterが、PointerTailが存在する点にいるとき
            {
                if (pointerTails[i].activeSelf)       //その点にあるPointerTailがactiveのとき
                {
                    pointerTails[lastPointerTailIndex].GetComponent<PointerTailMGR>().SetIsCrossing(true);
                    //Debug.LogWarning("isCrossing,Index:" + lastPointerTailIndex + ",manualIndex:" + i);
                    return;
                }
            }
        }
        pointerTails[lastPointerTailIndex].GetComponent<PointerTailMGR>().SetIsCrossing(false);
    }
}

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

    [SerializeField] List<Vector2Int> manualRoute;
    private bool isOnCastle;

    private Vector3 startPos;

    private void Start()
    {
        manualRoute = new List<Vector2Int>();
        pointerTails = new List<GameObject>();
        startPos = new Vector3(1.5f, 1.5f, 0);

        manualRoute.Add(new Vector2Int(1, 1));

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
    public void MoveByMouse(Vector2Int mouseGridPos) //マウスで移動
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position); //Updateで呼び出されるので毎回更新される


        if ((mouseGridPos - pointerGridPos).magnitude > 1)       //mouseとpointerが隣接していない場合はなにもしない
        {
            Debug.Log($"縦横のいずれかで隣接しているマスを選んでください　(mouseGridPos - pointerGridPos).magnitude:{(mouseGridPos - pointerGridPos).magnitude}");
            return;
        }

        if (manualRoute.Count(pos => pos == mouseGridPos) >= 2 && manualRoute[manualRoute.Count - 2] != mouseGridPos)           //進もうとしているマスを既に2回以上通っていて、来た道を戻らない場合はなにもしない
        {
            Debug.Log("同じマスを通れるのは2回までです");
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
    }
    private void ManageManualRouteList()
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position);

        if (manualRoute.Count >= 2 && manualRoute[manualRoute.Count - 2] == pointerGridPos)     //Pointerが一つ戻ったらrouteもそれに合わせる
        {
            manualRoute.RemoveAt(manualRoute.Count - 1);
            Debug.Log($"manualRoute:{string.Join(",", manualRoute)}");
        }
        else if (manualRoute.Count == 0 || (manualRoute[manualRoute.Count - 1] != pointerGridPos && manualRoute.Count(pos => pos == pointerGridPos) < 2))           //playerRouteに現在のgridPosが2つ以上入っていない場合、追加する
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
        if(transform.position.x >= GameManager.instance.mapMGR.GetMapWidth()-3 && transform.position.y >= GameManager.instance.mapMGR.GetMapHeight()-3)
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
        manualRoute = new List<Vector2Int>();
        isOnCastle = false;
        transform.position = startPos;
        while(pointerTails.Count != 0)
        {
            Destroy(pointerTails[pointerTails.Count -1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
        }
        manualRoute.Add(new Vector2Int(1, 1));
        Debug.Log($"Pointerを初期化:manualRoute={string.Join(",", manualRoute)}, isOnCastle={isOnCastle}, pointerTails={string.Join(",", pointerTails)}");
    }
}

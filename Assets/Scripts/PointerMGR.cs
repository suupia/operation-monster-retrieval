using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PointerMGR : MonoBehaviour
{
    private float moveInterval = 0.15f;
    private float lastMovingTime;
    [SerializeField] private GameObject pointerTailPrefab;
    private List<GameObject> pointerTails;

    [System.NonSerialized] public List<Vector2Int> playerRoute;


    private void Start()
    {
        lastMovingTime = -moveInterval;
        playerRoute = new List<Vector2Int>();
        pointerTails = new List<GameObject>();

    }

    public void MoveByArrowKey(Vector2Int directionVector) //矢印キーで移動
    {

        //positionをグリッドに合わせる
        Vector2Int gridPos = GameManager.instance.ToGridPosition(transform.position); //Updateで呼び出されるので毎回更新される
        transform.position = GameManager.instance.ToWorldPosition(gridPos);

        //arrowKeyFlagを変更する時に毎回ベクトルを割り当てているので、引数でもらったベクトルで入力の状況を判定できる。
        if (directionVector != Vector2Int.zero) //矢印キーを押したとき
        {
            if (GameManager.instance.inputMGR.ArrowKeyTimer - lastMovingTime >= moveInterval)
            {
                if (!(playerRoute.Count(pos => pos == gridPos + directionVector) < 2) && playerRoute[playerRoute.Count - 2] != gridPos + directionVector) //進もうとしているマスを既に2回以上通っているかつ、来た道を戻らない場合はなにもしない
                {
                    Debug.LogWarning("同じマスを通れるのは2回までです");
                    return;
                }

                if (GameManager.instance.mapMGR.GetMapValue(gridPos + directionVector) % GameManager.instance.groundID == 0)
                {
                    transform.position = transform.position + new Vector3(directionVector.x, directionVector.y, 0);
                    lastMovingTime = GameManager.instance.inputMGR.ArrowKeyTimer;
                }

            }
        }
        else //矢印キーを離したとき
        {
            GameManager.instance.inputMGR.ArrowKeyTimer = 0;

            lastMovingTime = -moveInterval;

        }

        ManagePlayerRouteList();
        ManageMouseTrails();
    }

    public void MoveByMouse(Vector2Int mouseGridPos) //マウスで移動
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position); //Updateで呼び出されるので毎回更新される


        if ((mouseGridPos - pointerGridPos).magnitude > 1)       //mouseとpointerが隣接していない場合はなにもしない
        {
            Debug.LogWarning($"縦横のいずれかで隣接しているマスを選んでください　(mouseGridPos - pointerGridPos).magnitude:{(mouseGridPos - pointerGridPos).magnitude}");
            return;
        }

        if (playerRoute.Count(pos => pos == mouseGridPos) >= 2 && playerRoute[playerRoute.Count - 2] != mouseGridPos)           //進もうとしているマスを既に2回以上通っていて、来た道を戻らない場合はなにもしない
        {
            Debug.LogWarning("同じマスを通れるのは2回までです");
            return;
        }

        if (GameManager.instance.mapMGR.GetMapValue(mouseGridPos) % GameManager.instance.groundID != 0)
        {
            Debug.LogWarning("groundじゃないので移動不可");
            return;           //groundじゃないので移動不可
        }
        transform.position = GameManager.instance.ToWorldPosition(mouseGridPos);

        ManagePlayerRouteList();
        ManageMouseTrails();
    }
    private void ManagePlayerRouteList()
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position);


        if (playerRoute.Count >= 2 && playerRoute[playerRoute.Count - 2] == pointerGridPos)     //MouseTrailerが一つ戻ったらrouteもそれに合わせる
        {
            playerRoute.RemoveAt(playerRoute.Count - 1);
            Debug.Log(ListToString(playerRoute, "playerRoute"));

        }
        else if (playerRoute.Count == 0 || (playerRoute[playerRoute.Count - 1] != pointerGridPos && playerRoute.Count(pos => pos == pointerGridPos) < 2))           //playerRouteに現在のgridPosが入っていない場合、追加する
        {
            playerRoute.Add(pointerGridPos);
            Debug.Log(ListToString(playerRoute, "playerRoute"));
        }
    }


    private void ManageMouseTrails()
    {
        while (playerRoute.Count > pointerTails.Count)
        {
            pointerTails.Add(Instantiate(pointerTailPrefab, GameManager.instance.ToWorldPosition(playerRoute[pointerTails.Count]), pointerTailPrefab.transform.rotation));
        }

        while (playerRoute.Count < pointerTails.Count)
        {
            Destroy(pointerTails[pointerTails.Count - 1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
        }
    }
    private string ListToString(List<Vector2Int> list, string listName)
    {
        string sentece = listName;
        foreach (Vector2Int i in list)
        {
            sentece += i;
        }
        return sentece;
    }
}

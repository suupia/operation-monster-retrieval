using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MouseTrailerMGR : MonoBehaviour
{
    private bool mouse = true;

    private float count = 0;
    private float moveInterval = 0.15f;
    private float lastMovingTime;
    private bool pressingArrowKey;
    [SerializeField] private GameObject mouseTrailPrefab;

    private List<GameObject> mouseTrails;

    [System.NonSerialized] public List<Vector2Int> playerRoute;
    // Start is called before the first frame update
    void Start()
    {
        lastMovingTime = -moveInterval;
        playerRoute = new List<Vector2Int>();
        mouseTrails = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mouse)        //斜めの移動をどうにかしなきゃいけない
        {
            MoveByMouse();
        }
        else
        {
            MoveByArrowKey();
        }

        //矢印キーを押している間、時間を測る
        if (pressingArrowKey)
        {
            count += Time.deltaTime;
        }

        ManagePlayerRouteList();
        ManageMouseTrails();
    }

    void MoveByArrowKey()            //矢印キーで移動
    {

        //positionをグリッドに合わせる
        Vector2Int gridPos = GameManager.instance.ToGridPosition(transform.position);
        transform.position = GameManager.instance.ToWorldPosition(gridPos);

        //1マスずつ移動
        if (count - lastMovingTime >= moveInterval)
        {
            Vector2Int deltaVec = Vector2Int.zero;
            if (Input.GetKey(KeyCode.DownArrow) && GameManager.instance.mapMGR.GetMapValue(gridPos + Vector2Int.down) % GameManager.instance.groundID == 0)
            {
                deltaVec = Vector2Int.down;
            }
            else if (Input.GetKey(KeyCode.UpArrow) && GameManager.instance.mapMGR.GetMapValue(gridPos + Vector2Int.up) % GameManager.instance.groundID == 0)
            {
                deltaVec = Vector2Int.up;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && GameManager.instance.mapMGR.GetMapValue(gridPos + Vector2Int.left) % GameManager.instance.groundID == 0)
            {
                deltaVec = Vector2Int.left;
            }
            else if (Input.GetKey(KeyCode.RightArrow) && GameManager.instance.mapMGR.GetMapValue(gridPos + Vector2Int.right) % GameManager.instance.groundID == 0)
            {
                deltaVec = Vector2Int.right;
            }
            //ここまでで、押したキーに応じてdeltaVec(移動量)を定める

            if (!(playerRoute.Count(pos => pos == gridPos + deltaVec) < 2) && playerRoute[playerRoute.Count - 2] != gridPos + deltaVec)           //進もうとしているマスを既に2回以上通っていて、来た道を戻らない場合はなにもしない
            {
                Debug.LogWarning("同じマスを通れるのは2回までです");
                return;
            }

            if (deltaVec != Vector2Int.zero)       //キーを押していた場合、MouseTrailerを移動する
            {
                transform.position = transform.position + new Vector3(deltaVec.x, deltaVec.y, 0);
                pressingArrowKey = true;
                lastMovingTime = count;
            }

        }

        //キーを離したときの処理↓
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            count = 0;
            lastMovingTime = -moveInterval;

            pressingArrowKey = false;
        }
    }

    void MoveByMouse()               //マウスで移動
    {
        if (!Input.GetMouseButton(1))     //右クリックしていない場合はなにもしない
        {
            return;
        }
        Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
        Vector2Int mouseGridPos = GameManager.instance.ToGridPosition(mousePos);

        if ((mouseGridPos - GameManager.instance.ToGridPosition(transform.position)).magnitude > Mathf.Sqrt(2))       //MouseTrailerとMouseが隣接していない場合はなにもしない
        {
            Debug.LogWarning("縦横斜めのいずれかで隣接しているマスを選んでください");
            return;
        }

        if (!(playerRoute.Count(pos => pos == mouseGridPos) < 2) && playerRoute[playerRoute.Count - 2] != mouseGridPos)           //進もうとしているマスを既に2回以上通っていて、来た道を戻らない場合はなにもしない
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


    }
    private void ManagePlayerRouteList()
    {
        Vector2Int trailerGridPos = GameManager.instance.ToGridPosition(transform.position);


        if (playerRoute.Count >= 2 && playerRoute[playerRoute.Count - 2] == trailerGridPos)     //MouseTrailerが一つ戻ったらrouteもそれに合わせる
        {
            playerRoute.RemoveAt(playerRoute.Count - 1);
            Debug.Log(ListToString(playerRoute, "playerRoute"));

        }
        else if (playerRoute.Count == 0 || (playerRoute[playerRoute.Count - 1] != trailerGridPos && playerRoute.Count(pos => pos == trailerGridPos) < 2))           //playerRouteに現在のgridPosが入っていない場合、追加する
        {
            playerRoute.Add(trailerGridPos);
            Debug.Log(ListToString(playerRoute, "playerRoute"));
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

    private void ManageMouseTrails()
    {
        while (playerRoute.Count > mouseTrails.Count)
        {
            mouseTrails.Add(Instantiate(mouseTrailPrefab, GameManager.instance.ToWorldPosition(playerRoute[mouseTrails.Count]), mouseTrailPrefab.transform.rotation));
        }

        while (playerRoute.Count < mouseTrails.Count)
        {
            Destroy(mouseTrails[mouseTrails.Count - 1]);
            mouseTrails.RemoveAt(mouseTrails.Count - 1);
        }
    }
}

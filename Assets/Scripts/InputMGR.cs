using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMGR : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] bool canMovePointer;
    [SerializeField] bool leftTouchFlag;
    [SerializeField] bool rightTouchFlag;
    [SerializeField] bool arrowKeyInputFlag;
    Vector2Int arrowKeyVector;

    [SerializeField] int verticalInput;
    [SerializeField] int horizontalInput;

    Vector2 mousePos;
    Vector2Int mouseGridPos;

    //プロパティ

    void Update()
    {
        //左クリック用
        if (Input.GetMouseButtonDown(0))
        {
            leftTouchFlag = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            leftTouchFlag = false;
        }

        //右クリック用
        if (Input.GetMouseButtonDown(1))
        {
            rightTouchFlag = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            rightTouchFlag = false;
        }

        //矢印キー用
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    arrowKeyInputFlag = true;
        //    arrowKeyVector = Vector2Int.right;
        //}else if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    arrowKeyInputFlag = true;
        //    arrowKeyVector = Vector2Int.left;
        //}else if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    arrowKeyInputFlag = true;
        //    arrowKeyVector = Vector2Int.up;
        //}else if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    arrowKeyInputFlag = true;
        //    arrowKeyVector = Vector2Int.down;
        //}
        //if ((Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)) && !(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
        //{
        //    arrowKeyInputFlag = false;
        //    arrowKeyVector = Vector2Int.zero;
        //}


        if (Input.GetKeyDown(KeyCode.P))          //Pointerの操作可、不可を変える
        {
            canMovePointer = !canMovePointer;
        }

        if(GameManager.instance.pointerMGR.GetIsOnCastle() && Input.GetKeyDown(KeyCode.Space))      //Routeを確定する。ここでManualRouteDataにリストを渡す
        {
            GameManager.instance.manualRouteDatas[0].manualRoute = GameManager.instance.pointerMGR.GetManualRoute();       //indexは仮に0としておく。要変更
            GameManager.instance.pointerMGR.ResetPointer();
            canMovePointer = false;
            Debug.LogWarning($"ルートを決定しました。RouteListをManualRouteDataに渡し、Pointerを操作不可にしました ManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[0].GetManualRoute())}");
        }

        if (leftTouchFlag) //道を作る
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
            //Debug.Log($"mousePosは{mousePos}");
            //Debug.Log($"mouseGridPosは{mouseGridPos}");

            GameManager.instance.mapMGR.MakeRoad(mouseGridPos.x,mouseGridPos.y);
        }


        if (canMovePointer && rightTouchFlag)
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
            //Debug.Log($"mousePosは{mousePos}");
            Debug.Log($"mouseGridPosは{mouseGridPos}");

            GameManager.instance.pointerMGR.MoveByMouse(mouseGridPos); //とりあえず、種類0番のキャラクターのルートを決定するようにする
        }
    }

}

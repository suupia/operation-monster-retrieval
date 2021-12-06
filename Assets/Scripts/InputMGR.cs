using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMGR : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] bool touchFlag;
    Vector2 mouseStartPos;
    Vector2 mouseCurrentPos;
    Vector2 mouseEndPos;
    Vector2Int mouseGridPos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            PointerUp();
        }

        if (touchFlag)
        {
            mouseCurrentPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = ToGridPos(mouseCurrentPos);
            Debug.Log($"mouseCurrentPosは{mouseCurrentPos}");
            Debug.Log($"mouseGridPosは{mouseGridPos}");

            GameManager.instance.mapMGR.MakeRoad(mouseGridPos.x,mouseGridPos.y);

        }

        //Debug用
        if (Input.GetKeyDown(KeyCode.F5)/*||true*/) //今はずっとデバッグしておく
        {
            Debug.Log("DebugMapValue()を実行します");
            GameManager.instance.debugMGR.DebugMapValue();
        }
        if(DebugMGR.isFirstDebugMapValue == false)
        {
            GameManager.instance.debugMGR.DebugMapValue();
        }


        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log("DebugAutoRouteValueを実行します");
            GameManager.instance.debugMGR.DebugAutoRouteValue();
        }
    }

    public void PointerDown()
    {
        mouseStartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        touchFlag = true;
    }
    public void PointerUp()
    {
        mouseEndPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        touchFlag = false;
    }

    public Vector2Int ToGridPos(Vector2 vector)
    {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
    }

}

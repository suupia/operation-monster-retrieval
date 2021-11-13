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
            Debug.Log($"mouseCurrentPos‚Í{mouseCurrentPos}");
            Debug.Log($"mouseCurrentPos‚ÌGridPos‚Í{ToGridPos(mouseCurrentPos)}");

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

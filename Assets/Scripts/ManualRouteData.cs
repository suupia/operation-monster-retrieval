using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRouteData : MonoBehaviour
{
    List<Vector2Int> manualRoute;
    List<Vector2Int> nonDiagonalManualRoute;
    List<int> nonDiagonalPoints;

    List<GameObject> pointerTails;
    private bool inEditingPointerTails;

    //コンストラクタ
    public ManualRouteData()
    {
        manualRoute = new List<Vector2Int>();
        nonDiagonalManualRoute = new List<Vector2Int>();
        nonDiagonalPoints = new List<int>();
        pointerTails = new List<GameObject>();
    }

    //Getter
    public List<Vector2Int> GetManualRoute()
    {
        return manualRoute;
    }

    public List<Vector2Int> GetNonDiagonalManualRoute()
    {
        return nonDiagonalManualRoute;
    }
    public List<int> GetNonDiagonalPoints()
    {
        return nonDiagonalPoints;
    }

    public List<GameObject> GetPointerTails()
    {
        return pointerTails;
    }

    //Setter
    public void SetManualRoute(List<Vector2Int> route)
    {
        manualRoute = route;
    }

    public void SetNonDiagonalManualRoute(List<Vector2Int> route)
    {
        nonDiagonalManualRoute = route;
    }
    public void SetNonDiagonalPoints(List<int> list)
    {
        nonDiagonalPoints = list;
    }


    public void ResetManualRouteData()
    {
        manualRoute.Clear();
        nonDiagonalManualRoute.Clear();
        nonDiagonalPoints.Clear();
        DestroyPointerTails();
    }

    public void ShowSelectedManualRoute()
    {
        if (inEditingPointerTails) return;
        inEditingPointerTails = true;

        while (nonDiagonalManualRoute.Count - 1 > pointerTails.Count)
        {
            GameObject pointerTail = GameManager.instance.pointerMGR.SpawnPointerTail(nonDiagonalManualRoute[pointerTails.Count]);
            pointerTails.Add(pointerTail);
        }
        inEditingPointerTails = false;
    }

    public void DestroyPointerTails()
    {
        if (inEditingPointerTails) return;
        inEditingPointerTails = true;

        while (0 < pointerTails.Count)
        {
            Destroy(pointerTails[pointerTails.Count - 1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
            //pointerTailMGRs.RemoveAt(pointerTails.Count - 1);
        }
        inEditingPointerTails = false;
    }
}

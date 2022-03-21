using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRouteData
{
    List<Vector2Int> manualRoute;
    List<Vector2Int> nonDiagonalManualRoute;
    List<int> nonDiagonalPoints;

    List<GameObject> pointerTails;
    bool inEditingPointerTails;

    bool wasEdited; //編集されてルートを持っているときtrue
    List<Vector2Int> nonDiagonalAutoRoute;
    List<Vector2Int> diagonalAutoRoute;

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

    public bool GetWasEdited()
    {
        return wasEdited;
    }

    public List<Vector2Int> GetNonDiagonalAutoRoute()
    {
        return nonDiagonalAutoRoute;
    }
    public List<Vector2Int> GetDiagonalAutoRoute()
    {
        return diagonalAutoRoute;
    }


    public List<Vector2Int> GetNonDiagonalManualorAutoRoute() //ManualRouteが空のときはAutoRouteを返す PointerTailMGR用
    {
        if (wasEdited) return nonDiagonalManualRoute;
        else return nonDiagonalAutoRoute;
    }
    public List<Vector2Int> GetDiagonalManualorAutoRoute() //ManualRouteが空のときはAutoRouteを返す PointerTailMGR用
    {
        if (wasEdited) return manualRoute;
        else return diagonalAutoRoute;
    }


    //Setter
    public void SetManualRoute(List<Vector2Int> route)
    {
        manualRoute = route;
        wasEdited = true;
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
        wasEdited = false;
    }

    public void ShowSelectedManualRoute()
    {
        if (inEditingPointerTails) return;
        inEditingPointerTails = true;

        List<Vector2Int> diagonalRoute;
        if (!wasEdited) //コピー開始時にManualRouteが空のときは、AutoRouteに代入して置いて、これをコピーする
        {
            nonDiagonalAutoRoute = Function.SearchShortestNonDiagonalRouteToCastle(GameManager.instance.mapMGR.GetAllysCastlePos());
            diagonalAutoRoute = Function.SearchShortestDiagonalRouteToCastle(GameManager.instance.mapMGR.GetAllysCastlePos());

            diagonalRoute = diagonalAutoRoute;
        }
        else
        {
            diagonalRoute = manualRoute;
        }


        while (diagonalRoute.Count - 1 > pointerTails.Count)
        {
            GameObject pointerTail = GameManager.instance.pointerMGR.SpawnPointerTail(diagonalRoute[pointerTails.Count]);
            pointerTails.Add(pointerTail);
        }
        inEditingPointerTails = false;
    }

    public void DestroyPointerTails()
    {
        if (inEditingPointerTails) return;
        inEditingPointerTails = true;

        GameManager.instance.pointerMGR.DestroyPointerTails(pointerTails);

        inEditingPointerTails = false;
    }

    public void CopyManualRouteData(ManualRouteData manualRouteData)
    {
        if (manualRouteData.GetWasEdited())
        {
            SetManualRoute(manualRouteData.GetManualRoute());
            SetNonDiagonalManualRoute(manualRouteData.GetNonDiagonalManualRoute());
            //SetNonDiagonalPoints(manualRouteData.GetNonDiagonalPoints());
        }
        else
        {
            SetManualRoute(manualRouteData.GetDiagonalAutoRoute());
            SetNonDiagonalManualRoute(manualRouteData.GetNonDiagonalAutoRoute());
            //SetNonDiagonalPoints();
        }
    }
}

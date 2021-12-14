using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRouteData
{
    List<Vector2Int> manualRoute;
    List<Vector2Int> nonDiagonalPoints;


    //コンストラクタ
    public ManualRouteData()
    {
        manualRoute = new List<Vector2Int>();
        nonDiagonalPoints = new List<Vector2Int>();
    }

    //Getter
    public List<Vector2Int> GetManualRoute()
    {
        return manualRoute;
    }

    public List<Vector2Int> GetNonDiagonalPoints()
    {
        return nonDiagonalPoints;
    }
    //Setter
    public void SetManualRoute(List<Vector2Int> route)
    {
        manualRoute = route;
    }

    public void SetNonDiaonalPoints(List<Vector2Int> list)
    {
        nonDiagonalPoints = list;
    }

}

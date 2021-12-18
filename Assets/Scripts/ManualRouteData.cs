using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRouteData
{
    List<Vector2Int> manualRoute;
    List<int> nonDiagonalPoints;
     

    //コンストラクタ
    public ManualRouteData()
    {
        manualRoute = new List<Vector2Int>();
        nonDiagonalPoints = new List<int>();
    }

    //Getter
    public List<Vector2Int> GetManualRoute()
    {
        return manualRoute;
    }

    public List<int> GetNonDiagonalPoints()
    {
        return nonDiagonalPoints;
    }
    //Setter
    public void SetManualRoute(List<Vector2Int> route)
    {
        manualRoute = route;
    }

    public void SetNonDiaonalPoints(List<int> list)
    {
        nonDiagonalPoints = list;
    }

}

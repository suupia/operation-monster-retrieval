using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRouteData
{
    List<Vector2Int> manualRoute;


    //コンストラクタ
    public ManualRouteData()
    {
        manualRoute = new List<Vector2Int>();
    }

    //Getter
    public List<Vector2Int> GetManualRoute()
    {
        return manualRoute;
    }

    //Setter
    public void SetManualRoute(List<Vector2Int> route)
    {
        manualRoute = route;
    }
}

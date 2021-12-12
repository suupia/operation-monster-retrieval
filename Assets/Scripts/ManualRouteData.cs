using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRouteData
{
    [System.NonSerialized] public List<Vector2Int> manualRoute;



    //コンストラクタ
    public ManualRouteData()
    {
        manualRoute = new List<Vector2Int>();
    }

    public List<Vector2Int> GetManualRoute()
    {
        return manualRoute;
    }
}

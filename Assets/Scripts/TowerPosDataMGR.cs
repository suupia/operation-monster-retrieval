using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPosDataMGR : MonoBehaviour
{
    TowerPosData[] towerPosDataArray;
    public TowerPosData[] GetTowerPosDataArray()
    {
        return towerPosDataArray;
    }
    void Awake()
    {
        //towerPosDataインスタンスを7個作る
        towerPosDataArray = new TowerPosData[7];

        for (int i = 0; i<towerPosDataArray.Length;i++)
        {
            string data = Resources.Load<TextAsset>("JSON/towerPoss"+i).ToString();
            towerPosDataArray[i] = JsonUtility.FromJson<TowerPosData>(data);
            Debug.Log(string.Join(",", towerPosDataArray[i].posData) );
        }

    }


}

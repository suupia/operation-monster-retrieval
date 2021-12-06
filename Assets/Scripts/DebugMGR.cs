using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DebugMGR : MonoBehaviour
{
    public static bool isFirstDebugMapValue=true;
    public static bool isFirstDebugAutoRouteValue=true;

    Text[] mapValueTextArray;
    Text[] autoRouteTextArray;

    [SerializeField] public GameObject mapValueTextParent; //インスペクター上でTextの親を決めておく
    [SerializeField] public GameObject autoRouteTextParent; //インスペクター上でTextの親を決めておく
    [SerializeField] private Text mapValueText;
    [SerializeField] private Text autoRouteText;


    public void DebugMapValue()
    {
        int x, y;

        if (isFirstDebugMapValue)
        {
            Vector3 textPosition;

            mapValueTextArray = new Text[GameManager.instance.mapMGR.GetMapSize()];

            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                x = i % GameManager.instance.mapMGR.GetMapWidth();
                y = (i - x) / GameManager.instance.mapMGR.GetMapWidth();

                textPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                mapValueTextArray[i] = Instantiate(mapValueText, textPosition, Quaternion.identity, mapValueTextParent.transform);
                mapValueTextArray[i].text = GameManager.instance.mapMGR.GetMapValue(i).ToString();

            }

            isFirstDebugMapValue = false;

        }
        else
        {
            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                mapValueTextArray[i].text = GameManager.instance.mapMGR.GetMapValue(i).ToString();


            }
        }
    }

    public void DebugAutoRouteValue()
    {
        int x, y;

        if (isFirstDebugAutoRouteValue)
        {
            Vector3 textPosition;

           autoRouteTextArray = new Text[GameManager.instance.mapMGR.GetMapSize()];

            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                x = i % GameManager.instance.mapMGR.GetMapWidth();
                y = (i - x) / GameManager.instance.mapMGR.GetMapWidth();

                textPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                autoRouteTextArray[i] = Instantiate(autoRouteText, textPosition, Quaternion.identity,autoRouteTextParent.transform);
                autoRouteTextArray[i].text = GameManager.instance.characterMGRs[0].GetAutoRoute().GetValue(i).ToString();

            }

            isFirstDebugAutoRouteValue = false;

        }
        else
        {
            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                autoRouteTextArray[i].text = GameManager.instance.characterMGRs[0].GetAutoRoute().GetValue(i).ToString();


            }
        }
    }
}

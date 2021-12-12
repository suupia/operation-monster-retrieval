using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DebugMGR : MonoBehaviour
{
    bool isFirstDebugMap=true;
    bool isDebuggingMap = false;

    bool isFirstDebugAutoRoute=true;
    bool isDebuggingAutoRoute = false;

    Text[] mapValueTextArray;
    Text[] autoRouteTextArray;

    [SerializeField] GameObject mapValueTextParent; //インスペクター上でTextの親を決めておく
    [SerializeField] GameObject autoRouteTextParent; //インスペクター上でTextの親を決めておく
    [SerializeField] Text mapValueText;
    [SerializeField] Text autoRouteText;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (!isDebuggingMap)
            {
                isDebuggingMap = true;
                mapValueTextParent.SetActive(true) ;
                DebugMap();
            }
            else
            {
                isDebuggingMap = false;
                mapValueTextParent.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (!isDebuggingAutoRoute)
            {
                isDebuggingAutoRoute = true;
                autoRouteTextParent.SetActive(true);
                DebugAutoRoute();
            }
            else
            {
                isDebuggingAutoRoute = false;
                autoRouteTextParent.SetActive(false);
            }
        }

        if (isDebuggingMap) DebugMap();
        if (isDebuggingAutoRoute) DebugAutoRoute();

    }
    public void DebugMap()
    {
        int x, y;

        if (isFirstDebugMap)
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

            isFirstDebugMap = false;

        }
        else
        {
            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                mapValueTextArray[i].text = GameManager.instance.mapMGR.GetMapValue(i).ToString();


            }
        }
    }

    public void DebugAutoRoute()
    {
        int x, y;

        if (isFirstDebugAutoRoute)
        {
            Vector3 textPosition;

           autoRouteTextArray = new Text[GameManager.instance.mapMGR.GetMapSize()];

            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                x = i % GameManager.instance.mapMGR.GetMapWidth();
                y = (i - x) / GameManager.instance.mapMGR.GetMapWidth();

                textPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                autoRouteTextArray[i] = Instantiate(autoRouteText, textPosition, Quaternion.identity,autoRouteTextParent.transform);
                autoRouteTextArray[i].text = GameManager.instance.autoRouteDatas[0].GetValue(i).ToString();

            }

            isFirstDebugAutoRoute = false;

        }
        else
        {
            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                autoRouteTextArray[i].text = GameManager.instance.autoRouteDatas[0].GetValue(i).ToString();


            }
        }
    }
}

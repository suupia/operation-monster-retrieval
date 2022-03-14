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

    bool isFirstDebugCharacterMGR = true;
    bool isDebuggingCharacterMGR = false;

    bool isFirstDebugFacility = true;
    bool isDebuggingFacility = false;

    Text[] mapValueTextArray;
    Text[] autoRouteTextArray;
    Text[] characterMGRTextArray;
    Text[] facilityTextArray;

    [SerializeField] GameObject mapValueTextParent; //インスペクター上でTextの親を決めておく
    [SerializeField] GameObject autoRouteTextParent; //インスペクター上でTextの親を決めておく
    [SerializeField] GameObject characterMGRTextParent; //インスペクター上でTextの親を決めておく
    [SerializeField] GameObject facilityTextParent; //インスペクター上でTextの親を決めておく


    [SerializeField] Text mapValueText;
    [SerializeField] Text autoRouteText;
    [SerializeField] Text characterMGRText;
    [SerializeField] Text facilityText;

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.F1))
        {
            MakeEverythingTheWay();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (!isDebuggingFacility)
            {
                isDebuggingFacility = true;
                facilityTextParent.SetActive(true);
                DebugFacility();
            }
            else
            {
                isDebuggingFacility = false;
                facilityTextParent.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (!isDebuggingCharacterMGR)
            {
                isDebuggingCharacterMGR = true;
                characterMGRTextParent.SetActive(true);
                DebugCharacterMGR();
            }
            else
            {
                isDebuggingCharacterMGR = false;
                characterMGRTextParent.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (!isDebuggingMap)
            {
                isDebuggingMap = true;
                mapValueTextParent.SetActive(true);
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
        if (Input.GetKeyDown(KeyCode.F7))
        {
            var shortestRoute = Function.SearchShortestRoute(GameManager.instance.mapMGR.GetMapWidth(), GameManager.instance.mapMGR.GetMapHeight(), new Vector2Int(1,3), new Vector2Int(3,3)) ;
            Debug.LogWarning($" Function.SearchShortestRoute={string.Join(",", shortestRoute)}");

        }

        if (isDebuggingMap) DebugMap();
        if (isDebuggingAutoRoute) DebugAutoRoute();
        if (isDebuggingCharacterMGR) DebugCharacterMGR();
        if (isDebuggingFacility) DebugFacility();

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
    public void DebugCharacterMGR()
    {
        int x, y;

        if (isFirstDebugCharacterMGR)
        {
            Vector3 textPosition;

            characterMGRTextArray = new Text[GameManager.instance.mapMGR.GetMapSize()];

            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                x = i % GameManager.instance.mapMGR.GetMapWidth();
                y = (i - x) / GameManager.instance.mapMGR.GetMapWidth();

                textPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                characterMGRTextArray[i] = Instantiate(characterMGRText, textPosition, Quaternion.identity, characterMGRTextParent.transform);
                characterMGRTextArray[i].text = GameManager.instance.mapMGR.GetMap().GetCharacterMGRList(i).Count.ToString(); //そのマスに存在するcharacterMGRの個数を返す

            }

            isFirstDebugCharacterMGR = false;

        }
        else
        {
            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                characterMGRTextArray[i].text = GameManager.instance.mapMGR.GetMap().GetCharacterMGRList(i).Count.ToString(); //そのマスに存在するcharacterMGRの個数を返す


            }
        }
    }
    public void DebugFacility()
    {
        int x, y;

        if (isFirstDebugFacility)
        {
            Vector3 textPosition;

            facilityTextArray = new Text[GameManager.instance.mapMGR.GetMapSize()];

            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {

                x = i % GameManager.instance.mapMGR.GetMapWidth();
                y = (i - x) / GameManager.instance.mapMGR.GetMapWidth();

                textPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                facilityTextArray[i] = Instantiate(facilityText, textPosition, Quaternion.identity, facilityTextParent.transform);
                if (GameManager.instance.mapMGR.GetMap().GetFacility(i) != null)
                {
                    facilityTextArray[i].text = GameManager.instance.mapMGR.GetMap().GetFacility(i).ToString(); //そのマスに存在するfacilityを返す
                }
                else
                {
                    facilityTextArray[i].text = ""; //nullの代わり
                }
               

            }

            isFirstDebugFacility = false;

        }
        else
        {
            for (int i = 0; i < GameManager.instance.mapMGR.GetMapSize(); i++)
            {
                if (GameManager.instance.mapMGR.GetMap().GetFacility(i) != null)
                {
                    facilityTextArray[i].text = GameManager.instance.mapMGR.GetMap().GetFacility(i).ToString(); //そのマスに存在するfacilityを返す
                }
                else
                {
                    facilityTextArray[i].text = ""; //nullの代わり
                }
            }
        }
    }


    public void MakeEverythingTheWay()
    {
        for(int y = 0; y < GameManager.instance.mapMGR.GetMapHeight(); y++)
        {
            for(int x = 0; x < GameManager.instance.mapMGR.GetMapWidth(); x++)
            {
                GameManager.instance.mapMGR.MakeRoadByPointer(x, y);
            }
        }
    }
}

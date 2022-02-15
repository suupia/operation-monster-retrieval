using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMGR : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] bool leftTouchFlag;
    [SerializeField] bool rightTouchFlag;
    [SerializeField] bool arrowKeyInputFlag;

    [SerializeField] int verticalInput;
    [SerializeField] int horizontalInput;

    [SerializeField] GameObject PauseTheGameCanvas; //インスペクター上でセットする
    bool pauseFlag = false; //ポーズ状態ならtrue


    Vector2 mousePos;
    Vector2Int mouseGridPos;

    private int manualRouteNumber = -1; //何も選択していないときは-1にしておく
    private SelectCharacterButtonMGR selectedButtonMGR; //Buttonが選択されたときにそのButtonのスクリプトのインスタンスを受け取る


    //プロパティ
    public int GetManualRouteNumber()
    {
        return manualRouteNumber;
    }

    public void SetManualRouteNumber(int number)
    {
        manualRouteNumber = number;
    }

    public void SetSelectedButtonMGR(SelectCharacterButtonMGR buttonMGR)
    {
        selectedButtonMGR = buttonMGR;
    }

    public SelectCharacterButtonMGR GetSelectedButtonMGR()
    {
        return selectedButtonMGR;
    }
    void Update()
    {
        if (GameManager.instance.state == GameManager.State.MakeTheFirstRoad) //MakeTheFirstRoadのとき、プレイヤーが指定された個数だけ道を作る
        {
            //左クリックのみ有効にする
            //左クリック用
            if (Input.GetMouseButtonDown(0))
            {
                leftTouchFlag = true;
                //Debug.Log("leftTouchFlagをtrueにしました");
            }
            if (Input.GetMouseButtonUp(0))
            {
                leftTouchFlag = false;
            }
        }


        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    foreach (ManualRouteData m in GameManager.instance.manualRouteDatas)
        //    {
        //        Debug.LogWarning(string.Join(",", m.GetManualRoute()));
        //    }
        //}

        if (GameManager.instance.state == GameManager.State.RunningGame || GameManager.instance.state == GameManager.State.PauseTheGame) //戦闘中またはポーズ中の時のみ一時停止用のメニューを操作できる。
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.LogWarning($"pauseFlagは{pauseFlag}です");
                if (pauseFlag == true)
                {
                    pauseFlag = false;
                    Debug.LogWarning("RuuningGameを呼びます");
                    GameManager.instance.RunningGame();
                    PauseTheGameCanvas.SetActive(false);
                }
                else
                {
                    pauseFlag = true;
                    Debug.LogWarning("PauseTheGameを呼びます");
                    GameManager.instance.PauseTheGame();
                    PauseTheGameCanvas.SetActive(true);
                }
            }
        }

        if (GameManager.instance.state != GameManager.State.MakeTheFirstRoad && GameManager.instance.state != GameManager.State.RunningGame) return; //以下の処理はGameManagerがMakeTheFirstRoadまたはRunningGameの時のみ実行される


        //左クリック用
        if (Input.GetMouseButtonDown(0))
        {
            leftTouchFlag = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            leftTouchFlag = false;
        }



        //右クリック用
        if (Input.GetMouseButtonDown(1))
        {
            rightTouchFlag = true;
        }
        if (Input.GetMouseButtonUp(1))     //Pointerの初期化
        {

            if (GameManager.instance.pointerMGR.GetIsOnCastle())      //Castleに到達していた場合、Routeを確定する。ここでManualRouteDataにリストを渡す
            {
                GameManager.instance.pointerMGR.SetFinalManualRoute();
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetManualRoute(GameManager.instance.pointerMGR.GetFinalManualRoute());
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetNonDiaonalPoints(GameManager.instance.pointerMGR.GetNonDiagonalPoints());
                Debug.LogWarning($"ルートを決定しました。 \n" +
                    $"ManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetManualRoute())} \n" +
                    $"NonDiagonalPoints:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetNonDiagonalPoints())}");
                manualRouteNumber = -1; //Reset
                selectedButtonMGR.ResetToNormalColor(); //ButtonのRest
            }
            rightTouchFlag = false;
            GameManager.instance.pointerMGR.ResetPointer();
        }


        if (leftTouchFlag) //道を作る
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
            //Debug.Log($"mousePosは{mousePos}");
            //Debug.Log($"mouseGridPosは{mouseGridPos}");

            GameManager.instance.mapMGR.MakeRoad(mouseGridPos.x, mouseGridPos.y);
        }


        if (rightTouchFlag)     //Pointerを動かす
        {
            if (manualRouteNumber == -1)
            {
                Debug.LogWarning($"Characterを選択してください manualRouteNumber={manualRouteNumber}");
            }
            else
            {
                mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
                //Debug.Log($"mousePosは{mousePos}");
                Debug.Log($"mouseGridPosは{mouseGridPos}");

                GameManager.instance.pointerMGR.MoveByMouse(mouseGridPos);
            }
        }


    }


}

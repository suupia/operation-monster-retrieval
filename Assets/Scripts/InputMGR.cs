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
    //bool pauseFlag = false; //ポーズ状態ならtrue
    //bool isPausingAtMakeTheFirstRoad; //ポーズしたときに前の状態がMakeTheFirstRoadであったことを知るために必要  PauseButonMGRに同値な変数を置いたのでコメントアウトしておく

    [SerializeField] DraggedCharacterThumbnail draggedCharacterThumbnail; //インスペクター上でセットする（SetActiveをfalseにしているため、ここで参照を保持しておく）

    public bool isEditingManualRoute;
    Vector2 mousePos;
    Vector2Int mouseGridPos;

    [SerializeField] private int manualRouteNumber = -1; //何も選択していないときは-1にしておく
    private SelectCharacterButtonMGR selectedButtonMGR; //Buttonが選択されたときにそのButtonのスクリプトのインスタンスを受け取る


    //プロパティ
    public int GetManualRouteNumber()
    {
        return manualRouteNumber;
    }
    public DraggedCharacterThumbnail GetDraggedCharacterThumbnail()
    {
        return draggedCharacterThumbnail;
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

        //以下の処理は、PauseButtonMGRとBackToTheGameButtonMGRで行うためコメントアウトしておく

        //if (GameManager.instance.state == GameManager.State.RunningGame || GameManager.instance.state == GameManager.State.PauseTheGame || GameManager.instance.state == GameManager.State.MakeTheFirstRoad) //戦闘中またはポーズ中の時のみ一時停止用のメニューを操作できる。
        //{
        //    if (Input.GetKeyDown(KeyCode.M))
        //    {
        //        if (pauseFlag == true)
        //        {
        //            if (isPausingAtMakeTheFirstRoad)
        //            {
        //                pauseFlag = false;
        //                isPausingAtMakeTheFirstRoad = false;
        //                Debug.Log("MakeTheFirstRoadを呼びます");
        //                GameManager.instance.MakeTheFirstRoad();
        //                PauseTheGameCanvas.SetActive(false);
        //            }
        //            else
        //            {
        //                pauseFlag = false;
        //                Debug.Log("RuuningGameを呼びます");
        //                GameManager.instance.RunningGame();
        //                PauseTheGameCanvas.SetActive(false);
        //            }

        //        }
        //        else
        //        {

        //            if (GameManager.instance.state == GameManager.State.MakeTheFirstRoad)
        //            {
        //                pauseFlag = true;
        //                isPausingAtMakeTheFirstRoad = true;
        //                Debug.Log("PauseTheGameを呼びます");
        //                GameManager.instance.PauseTheGame();
        //                PauseTheGameCanvas.SetActive(true);
        //            }
        //            else
        //            {
        //                pauseFlag = true;
        //                Debug.Log("PauseTheGameを呼びます");
        //                GameManager.instance.PauseTheGame();
        //                PauseTheGameCanvas.SetActive(true);
        //            }
        //        }
        //    }
        //}

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

            GameManager.instance.copyingManualRoute = false; //PointerDownしたときは常にリセットする
            GameManager.instance.copyingSelectCharacterButtonNum = -1; //PointerDownしたときは常にリセットする
        }
        if (Input.GetMouseButtonUp(1))     //Pointerの初期化
        {
            if (manualRouteNumber == -1)
            {
                Debug.Log($"Characterを選択してください manualRouteNumber={manualRouteNumber}");
            }
            else if(!isEditingManualRoute) //Buttonを選択して最初にPointerUpするまではPointerを動かせないようにしておく
            {
                isEditingManualRoute = true;

                GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].DestroyPointerTails();
                GameManager.instance.curveToMouseMGR.DestroyLineBetweenButtonAndPointer();
            }
            else if (GameManager.instance.pointerMGR.GetIsOnCastle())      //Castleに到達していた場合、そのままRouteを確定する。ここでManualRouteDataにリストを渡す
            {
                GameManager.instance.pointerMGR.SetFinalManualRoute();
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetManualRoute(GameManager.instance.pointerMGR.GetFinalManualRoute());
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetNonDiagonalManualRoute(GameManager.instance.pointerMGR.GetManualRoute());
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetNonDiagonalPoints(GameManager.instance.pointerMGR.GetNonDiagonalPoints());

                Debug.Log($"ルートを決定しました。 \n" +
                    $"ManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetManualRoute())} \n" +
                    $"NonDiagonalManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetNonDiagonalManualRoute())} \n" +
                    $"NonDiagonalPoints:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetNonDiagonalPoints())}");

                manualRouteNumber = -1; //Reset
                selectedButtonMGR.ResetToNormalColor(); //ButtonのRest
            }
            else //Castleに到達していない場合、Routeを補完してからRouteを確定する
            {
                //GameManager.instance.pointerMGR.ComplementManualRoute(GameManager.instance.mapMGR.GetEnemysCastlePos());
                //GameManager.instance.pointerMGR.SetFinalManualRoute();
                //GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetManualRoute(GameManager.instance.pointerMGR.GetFinalManualRoute());
                //GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetNonDiaonalPoints(GameManager.instance.pointerMGR.GetNonDiagonalPoints());
                //Debug.LogWarning($"ルートを決定しました。 \n" +
                //    $"ManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetManualRoute())} \n" +
                //    $"NonDiagonalPoints:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetNonDiagonalPoints())}");
                //manualRouteNumber = -1; //Reset
                //selectedButtonMGR.ResetToNormalColor(); //ButtonのRest
            }

            if (GameManager.instance.copyingManualRoute) //ManualRouteのコピー中にPointerUpした時の処理
            {
                GameManager.instance.copyingManualRoute = false;
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

            GameManager.instance.mapMGR.MakeRoadByPointer(mouseGridPos.x, mouseGridPos.y);
        }


        if (rightTouchFlag)     //Pointerを動かす
        {
            if (manualRouteNumber == -1)
            {
                //Debug.LogWarning($"Characterを選択してください manualRouteNumber={manualRouteNumber}");
            }
            else if (isEditingManualRoute)
            {
                if (!GameManager.instance.pointerMGR.inComplementingManualRoute) 
                {
                    mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
                    //Debug.Log($"mouseGridPosは{mouseGridPos}");

                    GameManager.instance.pointerMGR.MoveByMouse(mouseGridPos);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) //SelectCharacterButtonMGRのPointerDownの左クリックの時の処理の中身のみを記述している
        {
            GameManager.instance.SpawnCharacter(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.instance.SpawnCharacter(1);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.instance.SpawnCharacter(2);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameManager.instance.SpawnCharacter(3);

        }


    }

    public void ClosePauseTheGameCanvas()
    {
        //pauseFlag = false;
        PauseTheGameCanvas.SetActive(false);
    }

}

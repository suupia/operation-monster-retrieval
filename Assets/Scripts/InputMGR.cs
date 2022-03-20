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

    [SerializeField] GameObject PauseTheGameCanvas; //�C���X�y�N�^�[��ŃZ�b�g����
    //bool pauseFlag = false; //�|�[�Y��ԂȂ�true
    //bool isPausingAtMakeTheFirstRoad; //�|�[�Y�����Ƃ��ɑO�̏�Ԃ�MakeTheFirstRoad�ł��������Ƃ�m�邽�߂ɕK�v  PauseButonMGR�ɓ��l�ȕϐ���u�����̂ŃR�����g�A�E�g���Ă���

    [SerializeField] DraggedCharacterThumbnail draggedCharacterThumbnail; //�C���X�y�N�^�[��ŃZ�b�g����iSetActive��false�ɂ��Ă��邽�߁A�����ŎQ�Ƃ�ێ����Ă����j

    public bool isEditingManualRoute;
    Vector2 mousePos;
    Vector2Int mouseGridPos;

    [SerializeField] private int manualRouteNumber = -1; //�����I�����Ă��Ȃ��Ƃ���-1�ɂ��Ă���
    private SelectCharacterButtonMGR selectedButtonMGR; //Button���I�����ꂽ�Ƃ��ɂ���Button�̃X�N���v�g�̃C���X�^���X���󂯎��


    //�v���p�e�B
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
        if (GameManager.instance.state == GameManager.State.MakeTheFirstRoad) //MakeTheFirstRoad�̂Ƃ��A�v���C���[���w�肳�ꂽ�������������
        {
            //���N���b�N�̂ݗL���ɂ���
            //���N���b�N�p
            if (Input.GetMouseButtonDown(0))
            {
                leftTouchFlag = true;
                //Debug.Log("leftTouchFlag��true�ɂ��܂���");
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

        //�ȉ��̏����́APauseButtonMGR��BackToTheGameButtonMGR�ōs�����߃R�����g�A�E�g���Ă���

        //if (GameManager.instance.state == GameManager.State.RunningGame || GameManager.instance.state == GameManager.State.PauseTheGame || GameManager.instance.state == GameManager.State.MakeTheFirstRoad) //�퓬���܂��̓|�[�Y���̎��݈̂ꎞ��~�p�̃��j���[�𑀍�ł���B
        //{
        //    if (Input.GetKeyDown(KeyCode.M))
        //    {
        //        if (pauseFlag == true)
        //        {
        //            if (isPausingAtMakeTheFirstRoad)
        //            {
        //                pauseFlag = false;
        //                isPausingAtMakeTheFirstRoad = false;
        //                Debug.Log("MakeTheFirstRoad���Ăт܂�");
        //                GameManager.instance.MakeTheFirstRoad();
        //                PauseTheGameCanvas.SetActive(false);
        //            }
        //            else
        //            {
        //                pauseFlag = false;
        //                Debug.Log("RuuningGame���Ăт܂�");
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
        //                Debug.Log("PauseTheGame���Ăт܂�");
        //                GameManager.instance.PauseTheGame();
        //                PauseTheGameCanvas.SetActive(true);
        //            }
        //            else
        //            {
        //                pauseFlag = true;
        //                Debug.Log("PauseTheGame���Ăт܂�");
        //                GameManager.instance.PauseTheGame();
        //                PauseTheGameCanvas.SetActive(true);
        //            }
        //        }
        //    }
        //}

        if (GameManager.instance.state != GameManager.State.MakeTheFirstRoad && GameManager.instance.state != GameManager.State.RunningGame) return; //�ȉ��̏�����GameManager��MakeTheFirstRoad�܂���RunningGame�̎��̂ݎ��s�����


        //���N���b�N�p
        if (Input.GetMouseButtonDown(0))
        {
            leftTouchFlag = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            leftTouchFlag = false;
        }



        //�E�N���b�N�p
        if (Input.GetMouseButtonDown(1))
        {
            rightTouchFlag = true;

            GameManager.instance.copyingManualRoute = false; //PointerDown�����Ƃ��͏�Ƀ��Z�b�g����
            GameManager.instance.copyingSelectCharacterButtonNum = -1; //PointerDown�����Ƃ��͏�Ƀ��Z�b�g����
        }
        if (Input.GetMouseButtonUp(1))     //Pointer�̏�����
        {
            if (manualRouteNumber == -1)
            {
                Debug.Log($"Character��I�����Ă������� manualRouteNumber={manualRouteNumber}");
            }
            else if(!isEditingManualRoute) //Button��I�����čŏ���PointerUp����܂ł�Pointer�𓮂����Ȃ��悤�ɂ��Ă���
            {
                isEditingManualRoute = true;

                GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].DestroyPointerTails();
                GameManager.instance.curveToMouseMGR.DestroyLineBetweenButtonAndPointer();
            }
            else if (GameManager.instance.pointerMGR.GetIsOnCastle())      //Castle�ɓ��B���Ă����ꍇ�A���̂܂�Route���m�肷��B������ManualRouteData�Ƀ��X�g��n��
            {
                GameManager.instance.pointerMGR.SetFinalManualRoute();
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetManualRoute(GameManager.instance.pointerMGR.GetFinalManualRoute());
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetNonDiagonalManualRoute(GameManager.instance.pointerMGR.GetManualRoute());
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetNonDiagonalPoints(GameManager.instance.pointerMGR.GetNonDiagonalPoints());

                Debug.Log($"���[�g�����肵�܂����B \n" +
                    $"ManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetManualRoute())} \n" +
                    $"NonDiagonalManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetNonDiagonalManualRoute())} \n" +
                    $"NonDiagonalPoints:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetNonDiagonalPoints())}");

                manualRouteNumber = -1; //Reset
                selectedButtonMGR.ResetToNormalColor(); //Button��Rest
            }
            else //Castle�ɓ��B���Ă��Ȃ��ꍇ�ARoute��⊮���Ă���Route���m�肷��
            {
                //GameManager.instance.pointerMGR.ComplementManualRoute(GameManager.instance.mapMGR.GetEnemysCastlePos());
                //GameManager.instance.pointerMGR.SetFinalManualRoute();
                //GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetManualRoute(GameManager.instance.pointerMGR.GetFinalManualRoute());
                //GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetNonDiaonalPoints(GameManager.instance.pointerMGR.GetNonDiagonalPoints());
                //Debug.LogWarning($"���[�g�����肵�܂����B \n" +
                //    $"ManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetManualRoute())} \n" +
                //    $"NonDiagonalPoints:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetNonDiagonalPoints())}");
                //manualRouteNumber = -1; //Reset
                //selectedButtonMGR.ResetToNormalColor(); //Button��Rest
            }

            if (GameManager.instance.copyingManualRoute) //ManualRoute�̃R�s�[����PointerUp�������̏���
            {
                GameManager.instance.copyingManualRoute = false;
            }
            rightTouchFlag = false;
            GameManager.instance.pointerMGR.ResetPointer();
        }


        if (leftTouchFlag) //�������
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
            //Debug.Log($"mousePos��{mousePos}");
            //Debug.Log($"mouseGridPos��{mouseGridPos}");

            GameManager.instance.mapMGR.MakeRoadByPointer(mouseGridPos.x, mouseGridPos.y);
        }


        if (rightTouchFlag)     //Pointer�𓮂���
        {
            if (manualRouteNumber == -1)
            {
                //Debug.LogWarning($"Character��I�����Ă������� manualRouteNumber={manualRouteNumber}");
            }
            else if (isEditingManualRoute)
            {
                if (!GameManager.instance.pointerMGR.inComplementingManualRoute) 
                {
                    mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
                    //Debug.Log($"mouseGridPos��{mouseGridPos}");

                    GameManager.instance.pointerMGR.MoveByMouse(mouseGridPos);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) //SelectCharacterButtonMGR��PointerDown�̍��N���b�N�̎��̏����̒��g�݂̂��L�q���Ă���
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

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
    bool pauseFlag = false; //�|�[�Y��ԂȂ�true


    Vector2 mousePos;
    Vector2Int mouseGridPos;

    private int manualRouteNumber = -1; //�����I�����Ă��Ȃ��Ƃ���-1�ɂ��Ă���
    private SelectCharacterButtonMGR selectedButtonMGR; //Button���I�����ꂽ�Ƃ��ɂ���Button�̃X�N���v�g�̃C���X�^���X���󂯎��


    //�v���p�e�B
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

        if (GameManager.instance.state == GameManager.State.RunningGame || GameManager.instance.state == GameManager.State.PauseTheGame) //�퓬���܂��̓|�[�Y���̎��݈̂ꎞ��~�p�̃��j���[�𑀍�ł���B
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.LogWarning($"pauseFlag��{pauseFlag}�ł�");
                if (pauseFlag == true)
                {
                    pauseFlag = false;
                    Debug.LogWarning("RuuningGame���Ăт܂�");
                    GameManager.instance.RunningGame();
                    PauseTheGameCanvas.SetActive(false);
                }
                else
                {
                    pauseFlag = true;
                    Debug.LogWarning("PauseTheGame���Ăт܂�");
                    GameManager.instance.PauseTheGame();
                    PauseTheGameCanvas.SetActive(true);
                }
            }
        }

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
        }
        if (Input.GetMouseButtonUp(1))     //Pointer�̏�����
        {

            if (GameManager.instance.pointerMGR.GetIsOnCastle())      //Castle�ɓ��B���Ă����ꍇ�ARoute���m�肷��B������ManualRouteData�Ƀ��X�g��n��
            {
                GameManager.instance.pointerMGR.SetFinalManualRoute();
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetManualRoute(GameManager.instance.pointerMGR.GetFinalManualRoute());
                GameManager.instance.manualRouteDatas[GetManualRouteNumber()].SetNonDiaonalPoints(GameManager.instance.pointerMGR.GetNonDiagonalPoints());
                Debug.LogWarning($"���[�g�����肵�܂����B \n" +
                    $"ManualRoute:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetManualRoute())} \n" +
                    $"NonDiagonalPoints:{string.Join(",", GameManager.instance.manualRouteDatas[GetManualRouteNumber()].GetNonDiagonalPoints())}");
                manualRouteNumber = -1; //Reset
                selectedButtonMGR.ResetToNormalColor(); //Button��Rest
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

            GameManager.instance.mapMGR.MakeRoad(mouseGridPos.x, mouseGridPos.y);
        }


        if (rightTouchFlag)     //Pointer�𓮂���
        {
            if (manualRouteNumber == -1)
            {
                Debug.LogWarning($"Character��I�����Ă������� manualRouteNumber={manualRouteNumber}");
            }
            else
            {
                mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
                //Debug.Log($"mousePos��{mousePos}");
                Debug.Log($"mouseGridPos��{mouseGridPos}");

                GameManager.instance.pointerMGR.MoveByMouse(mouseGridPos);
            }
        }


    }


}

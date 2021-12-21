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

    Vector2 mousePos;
    Vector2Int mouseGridPos;

    private int manualRouteNumber = -1; //�����I�����Ă��Ȃ��Ƃ���-1�ɂ��Ă���
    private ButtonMGR selectedButtonMGR; //Button���I�����ꂽ�Ƃ��ɂ���Button�̃X�N���v�g�̃C���X�^���X���󂯎��

    //�v���p�e�B
    public int GetManualRouteNumber()
    {
        return manualRouteNumber;
    }

    public void SetManualRouteNumber(int number)
    {
        manualRouteNumber = number;
    }

    public void SetSelectedButtonMGR(ButtonMGR buttonMGR)
    {
        selectedButtonMGR = buttonMGR;
    }

    public ButtonMGR GetSelectedButtonMGR()
    {
        return selectedButtonMGR;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            foreach(ManualRouteData m in GameManager.instance.manualRouteDatas)
            {
                Debug.LogWarning(string.Join(",", m.GetManualRoute()));
            }
        }
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

            GameManager.instance.mapMGR.MakeRoad(mouseGridPos.x,mouseGridPos.y);
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

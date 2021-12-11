using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMGR : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] bool canMovePointer;
    [SerializeField] bool leftTouchFlag;
    [SerializeField] bool rightTouchFlag;
    [SerializeField] bool arrowKeyInputFlag;
    Vector2Int arrowKeyVector;
    float arrowKeyTimer = 0;

    Vector2 mousePos;
    Vector2Int mouseGridPos;

    //�v���p�e�B
    public float ArrowKeyTimer
    {
        get { return arrowKeyTimer; }
        set { arrowKeyTimer = value; }
    }

    void Update()
    {
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
        if (Input.GetMouseButtonUp(1))
        {
            rightTouchFlag = false;
        }

        //���L�[�p
        if (Input.GetKey(KeyCode.RightArrow))
        {
            arrowKeyInputFlag = true;
            arrowKeyVector = Vector2Int.right;
        }else if (Input.GetKey(KeyCode.LeftArrow))
        {
            arrowKeyInputFlag = true;
            arrowKeyVector = Vector2Int.left;
        }else if (Input.GetKey(KeyCode.UpArrow))
        {
            arrowKeyInputFlag = true;
            arrowKeyVector = Vector2Int.up;
        }else if (Input.GetKey(KeyCode.DownArrow))
        {
            arrowKeyInputFlag = true;
            arrowKeyVector = Vector2Int.down;
        }
        if ((Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)) && !(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
        {
            arrowKeyInputFlag = false;
            arrowKeyVector = Vector2Int.zero;
            arrowKeyTimer = 0;
            GameManager.instance.pointerMGR.ResetLastMovementTime();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            canMovePointer = !canMovePointer;

            if (!canMovePointer)    //canMovePointer��false�ɂȂ����Ƃ��AlastMovementTime��Timer�����Z�b�g����
            {
                GameManager.instance.pointerMGR.ResetLastMovementTime();
                arrowKeyTimer = 0;
            }
        }

        if (leftTouchFlag) //�������
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
            //Debug.Log($"mousePos��{mousePos}");
            //Debug.Log($"mouseGridPos��{mouseGridPos}");

            GameManager.instance.mapMGR.MakeRoad(mouseGridPos.x,mouseGridPos.y);
        }

        if (canMovePointer && rightTouchFlag) //Pointer�𓮂���
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
            //Debug.Log($"mousePos��{mousePos}");
            Debug.Log($"mouseGridPos��{mouseGridPos}");

            GameManager.instance.pointerMGR.MoveByMouse(mouseGridPos); //�Ƃ肠�����A���0�Ԃ̃L�����N�^�[�̃��[�g�����肷��悤�ɂ���
        }

        if (canMovePointer && arrowKeyInputFlag)
        {
            arrowKeyTimer += Time.fixedDeltaTime;
            GameManager.instance.pointerMGR.MoveByArrowKey(arrowKeyVector);
        }


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMGR : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

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
            mouseDown(0);
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseUp(0);
        }

        //�E�N���b�N�p
        if (Input.GetMouseButtonDown(1))
        {
            mouseDown(1);
        }
        if (Input.GetMouseButtonUp(1))
        {
            mouseUp(1);
        }

        //���L�[�p
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            arrowKeyInputFlag = true;
            arrowKeyVector = Vector2Int.right;
        }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            arrowKeyInputFlag = true;
            arrowKeyVector = Vector2Int.left;
        }else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            arrowKeyInputFlag = true;
            arrowKeyVector = Vector2Int.up;
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            arrowKeyInputFlag = true;
            arrowKeyVector = Vector2Int.down;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            arrowKeyInputFlag = false;
            arrowKeyVector = Vector2Int.zero;
        }

        if (leftTouchFlag) //�������
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
            //Debug.Log($"mousePos��{mousePos}");
            //Debug.Log($"mouseGridPos��{mouseGridPos}");

            GameManager.instance.mapMGR.MakeRoad(mouseGridPos.x,mouseGridPos.y);
        }

        if (rightTouchFlag) //Pointer�𓮂���
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseGridPos = GameManager.instance.ToGridPosition(mousePos);
            //Debug.Log($"mousePos��{mousePos}");
            Debug.Log($"mouseGridPos��{mouseGridPos}");

            GameManager.instance.pointerMGR.MoveByMouse(mouseGridPos); //�Ƃ肠�����A���0�Ԃ̃L�����N�^�[�̃��[�g�����肷��悤�ɂ���
        }

        if (arrowKeyInputFlag)
        {
            arrowKeyTimer += Time.fixedDeltaTime;
            GameManager.instance.pointerMGR.MoveByArrowKey(arrowKeyVector);
        }


    }

    public void mouseDown(int num)
    {
        if (num == 0)
        {
            leftTouchFlag = true;
        }else if (num ==1)
        {
            rightTouchFlag = true;
        }
        else
        {
            Debug.LogError($"mouseDown�ŗ\�����ʈ������^�����܂����Bnum:{num}");
        }
    }
    public void mouseUp(int num)
    {
        if (num == 0)
        {
            leftTouchFlag = false;
        }
        else if (num == 1)
        {
            rightTouchFlag = false;
        }
        else
        {
            Debug.LogError($"mouseUp�ŗ\�����ʈ������^�����܂����Bnum:{num}");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveToMouseMGR : MonoBehaviour
{
    [SerializeField] GameObject curveToMouseCanvas;
    [SerializeField] GameObject curvePointer;

    [SerializeField] List<GameObject> curvePointers;
    private int divisionNum;

    private bool isEditingCurvePointers;

    private void Start()
    {
        curvePointers = new List<GameObject>();
    }
    public void SetCircles(int buttonNum)
    {
        Vector2 startPos = GameManager.instance.selectCharacterButtonMGRs[buttonNum].transform.position;

        Vector2 endPos;
        if(GameManager.instance.mouseEnteredSelectCharacterButtonNum == -1 || GameManager.instance.mouseEnteredSelectCharacterButtonNum == buttonNum)
        {
            endPos = (Vector2)Input.mousePosition;
        }
        else
        {
            endPos = GameManager.instance.selectCharacterButtonMGRs[GameManager.instance.mouseEnteredSelectCharacterButtonNum].transform.position;
        }
        divisionNum = (int)(endPos - startPos).magnitude / 50 + 1;

        if (isEditingCurvePointers) return;
        isEditingCurvePointers = true;

        for (int i = 0; i <= divisionNum; i++)
        {
            if (i < curvePointers.Count)
            {
                curvePointers[i].transform.position = PointerPosInCurve((float)i / (float)divisionNum, startPos, endPos);
            }
            else
            {
                curvePointers.Add(Instantiate(curvePointer, PointerPosInCurve((float)i / (float)divisionNum, startPos, endPos), curvePointer.transform.rotation, curveToMouseCanvas.transform));
            }
        }

        while (curvePointers.Count > divisionNum + 1)
        {
            Destroy(curvePointers[curvePointers.Count - 1]);
            curvePointers.RemoveAt(curvePointers.Count - 1);
        }

        isEditingCurvePointers = false;
    }

    public Vector2 PointerPosInCurve(float r, Vector2 startPos, Vector2 endPos) //���s�ړ����ς܂������W��Ԃ�
    {
        if (r < 0 || r > 1)
        {
            Debug.LogError("Curve�̕ϐ���0�`1�ɂ��Ă������� r = " + r);
        }
        Vector2 deltaPos = endPos - startPos;

        Vector2 returnPos = Curve(deltaPos.x * r, deltaPos) + startPos;

        return returnPos;
    }

    public Vector2 Curve(float x, Vector2 endPos) //�n�_�����_�Ƃ����Ƃ��̊֐� �n�_(=���_)����I�_�܂ł̃x�N�g��(=endPos)�ɂ���Ċ֐������߂�
    {
        float a, b; //y=ax^2+bx�Ƃ���
        float endPosTan = Mathf.Atan(endPos.y / endPos.x);
        float deltaAngle = Mathf.PI / 4 / (Mathf.Abs(endPosTan) * Mathf.Abs(endPosTan) + 1); //���_�ɂ�����ڐ���endPos�x�N�g���ׂ̈��p �ڐ��̌X�����ߑ�ɂȂ�Ȃ��悤�ɁA���X�ɏ��������Ă���

        if (x < 0)
        {
            deltaAngle *= -1; //��ɏ�ɓʂɂ��邽��
        }

        b = Mathf.Tan(deltaAngle + endPosTan);

        if(Mathf.Abs(b) > 2) //�ڐ��̌X�����ߑ�ɂȂ�Ȃ��悤�ɁA���X�ɏ��������Ă���
        {
            deltaAngle *= 1 / (Mathf.Abs(b) - 1); //���W�A�� ���_�ɂ�����������̐ڐ���endPos�x�N�g���ׂ̈��p
        }

        if (deltaAngle > 0 && b < 0) //�E�Ɍ������Ȑ��ŁA�ڐ��̌X�������ɂȂ�Ƃ�
        {
            deltaAngle = 0;
        }
        else if (deltaAngle < 0 && b > 0) //���Ɍ������Ȑ��ŁA�ڐ��̌X�������ɂȂ�Ƃ�
        {
            deltaAngle = 0;
        }

        b = Mathf.Tan(deltaAngle + endPosTan);

        a = (endPos.y - endPos.x * b) / (endPos.x * endPos.x);

        float y = a * x * x + b * x;
        //Debug.LogWarning($"a={a}, b={b}, x={x}, endPos={endPos}, y={y}");

        return new Vector2(x, y);
    }
    public void DestroyLineBetweenButtonAndPointer()
    {
        if (isEditingCurvePointers) return;
        isEditingCurvePointers = true;

        while (curvePointers.Count != 0)
        {
            Destroy(curvePointers[curvePointers.Count - 1]);
            curvePointers.RemoveAt(curvePointers.Count - 1);
        }

        isEditingCurvePointers = false;
    }

    public void ResetCopyingManualRoue() //�ǂ��ɏ����ׂ����킩���`�`
    {
        DestroyLineBetweenButtonAndPointer();
        if (GameManager.instance.copyingSelectCharacterButtonNum != -1)
        {
            GameManager.instance.manualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].DestroyPointerTails();
            GameManager.instance.copyingManualRoute = false;
            GameManager.instance.copyingSelectCharacterButtonNum = -1;
        }
        GameManager.instance.mouseEnteredSelectCharacterButtonNum = -1;
    }


}

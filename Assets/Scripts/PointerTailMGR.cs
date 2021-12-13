using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTailMGR : MonoBehaviour
{
    private int pointerTailIndex;
    // Start is called before the first frame update
    void Start()
    {
        pointerTailIndex = GameManager.instance.pointerMGR.GetPoinerTails().IndexOf(gameObject);    //pointerTails��PointeTail�����ԂɎd�����Ă���̂ŁAIndex���擾���Ă���
    }

    private void LateUpdate()
    {
        if(pointerTailIndex < GameManager.instance.pointerMGR.GetPoinerTails().Count - 3)      //����PointerTail��������x�O�̂��̂������ꍇ�A�����͍s��Ȃ�
        {
            return;
        }
        RotatePointerTail();
    }
    private void RotatePointerTail()
    {
        int angle = 0;

        if (GameManager.instance.pointerMGR.GetManualRoute().Count >= pointerTailIndex + 2 && GameManager.instance.pointerMGR.GetPoinerTails().Count > pointerTailIndex + 1)       //pointerTail�����ꎩ�g�̌��ɑ��݂���ꍇ�̏���
        {
            if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.up + Vector2Int.right == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 2] && 
                GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
            {
                angle = 45;
            }
            else if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.up + Vector2Int.left == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 2] &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
            {
                angle = 135;
            }
            else if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.down + Vector2Int.right == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 2] &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
            {
                angle = -45;
            }
            else if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.down + Vector2Int.left == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 2] &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                     GameManager.instance.mapMGR.GetMapValue(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
            {
                angle = -135;
            }

            //�΂߂������ꍇ�A�����܂ł�angle��0�ȊO�ɒ�܂�
            if (angle != 0)
            {
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.localScale = new Vector3(Mathf.Sqrt(2), 1, 1);                //�΂߂������Ƃ��͒�����sqrt2�ɂ���
                GameManager.instance.pointerMGR.GetPoinerTails()[pointerTailIndex + 1].SetActive(false);      //������΂߂Ɍ�������ꍇ�A���̎���PointerTail�͕\������K�v���Ȃ��̂ŁA��A�N�e�B�u�ɂ���
                                                                                                              //���v���Ǝv�����ǁA���̎���PointerTail��Update����ɌĂ΂��ƍ���C������̂ŁA�ꉞ�����Ƃ���
                return;    //�΂߂��������Ƃ��͂�����return�����
            }
            else
            {
                GameManager.instance.pointerMGR.GetPoinerTails()[pointerTailIndex + 1].SetActive(true);        //���ꂪ�΂߂������Ȃ��Ƃ��́A����PointerTail���A�N�e�B�u�ɂ���
            }
        }


        if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].x == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].x)      //�ȉ��A�c������������
        {
            if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].y < GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].y)
            {
                angle = 90;
            }
            else
            {
                angle = -90;

            }
        }
        else if(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].y == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].y)
        {
            if(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].x < GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].x)
            {
                angle = 0;
            }
            else
            {
                angle = 180;
            }
        }
        transform.localScale = new Vector3(1, 1, 1);        //�΂߂������Ȃ��Ƃ���scale�͒�����1��

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }
}

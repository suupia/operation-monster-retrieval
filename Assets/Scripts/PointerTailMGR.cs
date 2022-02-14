using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTailMGR : MonoBehaviour
{
    private int pointerTailIndex;
    private List<Vector2Int> manualRoute;
    private List<GameObject> pointerTails;
    private MapData map;
    private GameObject[] children;
    private SpriteRenderer[] childRenderers;
    private Vector2[] defaultPointerTailSizes;
    private float pointerTailRate;
    private Color noChangedColor;
    [SerializeField] Color changedColor;
    [SerializeField] Sprite[] firstTailSprites;

    [SerializeField] bool nonDiagonal;     //���ꂪtrue�̂Ƃ��APointerTail�͎΂߂������Ȃ��悤�ɂ���i�ŏ��̉�]�p�j
    [SerializeField] bool isDiagonal;      //�΂߂������Ă�Ƃ��Atrue

    //�v���p�e�B
    public bool NonDiagonal
    {
        get { return nonDiagonal; }
        set { nonDiagonal = value; }
    }

    public bool GetDiagonal()
    {
        return isDiagonal;
    }
    public void SetDiagonal(bool b)
    {
        isDiagonal = b;
    }
    // Start is called before the first frame update
    void Start()
    {
        pointerTails = GameManager.instance.pointerMGR.GetPoinerTails();
        pointerTailIndex = pointerTails.IndexOf(gameObject);    //pointerTails��PointeTail�����ԂɎd�����Ă���̂ŁAIndex���擾���Ă���
        manualRoute = GameManager.instance.pointerMGR.GetManualRoute();
        map = GameManager.instance.mapMGR.GetMap();
        children = new GameObject[2];
        children[0] = transform.GetChild(0).gameObject;
        children[1] = transform.GetChild(1).gameObject;
        childRenderers = new SpriteRenderer[2];
        childRenderers[0] = children[0].GetComponent<SpriteRenderer>();
        childRenderers[1] = children[1].GetComponent<SpriteRenderer>();
        defaultPointerTailSizes = new Vector2[2];
        defaultPointerTailSizes[0] = childRenderers[0].size;
        defaultPointerTailSizes[1] = childRenderers[1].size;
        pointerTailRate = Mathf.Sqrt(2) * 19/20;//��1��19/20�A��2��37/40
        noChangedColor = childRenderers[0].color;

        childRenderers[0].sortingOrder = pointerTailIndex;
        childRenderers[1].sortingOrder = pointerTailIndex;

        if(pointerTailIndex == 0)
        {
            childRenderers[0].sprite = firstTailSprites[0];
            childRenderers[1].sprite = firstTailSprites[1];
        }
    }

    private void LateUpdate()
    {
        if (pointerTailIndex < GameManager.instance.pointerMGR.GetPoinerTails().Count - 4)      //����PointerTail��������x�O�̂��̂������ꍇ�A�����͍s��Ȃ�
        {
            return;
        }
        RotatePointerTail();
        ChangeColor();
    }
    private void RotatePointerTail()
    {
        int angle = 0;

        ManageNonDiagonal();

        if (manualRoute.Count >= pointerTailIndex + 2 && GameManager.instance.pointerMGR.GetPoinerTails().Count > pointerTailIndex + 1)       //pointerTail�����ꎩ�g�̌��ɑ��݂���ꍇ�̏���
        {
            if (!nonDiagonal)
            {
                if (manualRoute[pointerTailIndex] + Vector2Int.up + Vector2Int.right == manualRoute[pointerTailIndex + 2] &&
                    map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                    map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
                {
                    angle = 45;
                }
                else if (manualRoute[pointerTailIndex] + Vector2Int.up + Vector2Int.left == manualRoute[pointerTailIndex + 2] &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.up) % GameManager.instance.groundID == 0 &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
                {
                    angle = 135;
                }
                else if (manualRoute[pointerTailIndex] + Vector2Int.down + Vector2Int.right == manualRoute[pointerTailIndex + 2] &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.right) % GameManager.instance.groundID == 0)
                {
                    angle = -45;
                }
                else if (manualRoute[pointerTailIndex] + Vector2Int.down + Vector2Int.left == manualRoute[pointerTailIndex + 2] &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.down) % GameManager.instance.groundID == 0 &&
                         map.GetValue(manualRoute[pointerTailIndex] + Vector2Int.left) % GameManager.instance.groundID == 0)
                {
                    angle = -135;
                }
            }
            //�΂߂������ꍇ�A�����܂ł�angle��0�ȊO�ɒ�܂�

            if (angle != 0)
            {
                transform.rotation = Quaternion.AngleAxis(angle-45, Vector3.forward);
                //childRenderers[0].size = new Vector2(pointerTailRate, 1)* defaultPointerTailSizes[0];                //�΂߂������Ƃ��͒�����sqrt2�ɂ���
                ChangeToDiagonal(true);
                pointerTails[pointerTailIndex + 1].SetActive(false);      //������΂߂Ɍ�������ꍇ�A���̎���PointerTail�͕\������K�v���Ȃ��̂ŁA��A�N�e�B�u�ɂ���
                                                                          //���v���Ǝv�����ǁA���̎���PointerTail��Update����ɌĂ΂��ƍ���C������̂ŁA�ꉞ�����Ƃ���
                SetDiagonal(true);
                return;    //�΂߂��������Ƃ��͂�����return�����
            }
            else
            {
                pointerTails[pointerTailIndex + 1].SetActive(true);        //���ꂪ�΂߂������Ȃ��Ƃ��́A����PointerTail���A�N�e�B�u�ɂ���
                SetDiagonal(false);
            }
        }


        if (manualRoute[pointerTailIndex].x == manualRoute[pointerTailIndex + 1].x)      //�ȉ��A�c������������
        {
            if (manualRoute[pointerTailIndex].y < manualRoute[pointerTailIndex + 1].y)
            {
                angle = 90;
            }
            else
            {
                angle = -90;

            }
        }
        else if(manualRoute[pointerTailIndex].y == manualRoute[pointerTailIndex + 1].y)
        {
            if(manualRoute[pointerTailIndex].x < manualRoute[pointerTailIndex + 1].x)
            {
                angle = 0;
            }
            else
            {
                angle = 180;
            }
        }
        //childRenderers[0].size = new Vector2(1, 1)* defaultPointerTailSizex[0];        //�΂߂������Ȃ��Ƃ���scale�͒�����1��
        ChangeToDiagonal(false);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    private void ManageNonDiagonal()
    {

        if (pointerTailIndex >= 2 && manualRoute.Count >= pointerTailIndex + 3)           //�ŏ��̉�]���܂ނƂ��̏����@����PointerTail����]�Ɋ܂܂��5�_�̂���3�_�ڂ̂Ƃ��ɏ������s��
        {
            if (manualRoute[pointerTailIndex + 2] == manualRoute[pointerTailIndex - 2])
            {
                for (int i = 0; i < 3; i++)
                {
                    if(i == 2 && !pointerTails[pointerTailIndex - i].GetComponent<PointerTailMGR>().isDiagonal) { break; }    //��Ԗڂ̓_���΂߂ɂȂ��Ă��Ȃ��Ƃ�(=0�Ԗڂ̓_���΂߂̂Ƃ�)�͈�Ԗڂ̓_�͂��̂܂܂ɂ���
                    pointerTails[pointerTailIndex - i].SetActive(true);              //�΂߂ɂȂ��Ă���PointerTail�̎���PointerTail�͔�A�N�e�B�u�ɂ��Ă���̂ŁA�A�N�e�B�u�ɂ���
                    pointerTails[pointerTailIndex - i].GetComponent<PointerTailMGR>().NonDiagonal = true;
                }
            }
        }
        else if (pointerTailIndex >= 2 && manualRoute.Count == pointerTailIndex + 2 && NonDiagonal &&
                 pointerTails[pointerTailIndex - 1].GetComponent<PointerTailMGR>().NonDiagonal &&
                 pointerTails[pointerTailIndex - 2].GetComponent<PointerTailMGR>().NonDiagonal)        //����PointerTail���ŏ��̉�]�Ɋ܂܂��3�_�ڂ��APointer������PointerTail�̎��̃}�X�ɂ���Ƃ�(��]�ɂȂ��Ă��Ȃ��Ƃ�)
        {
            if (pointerTailIndex >= 3 && pointerTails[pointerTailIndex - 3].GetComponent<PointerTailMGR>().NonDiagonal)
            {
                NonDiagonal = false;
                return;
            }
            for (int i = 2; i >= 0; i--)
            {
                pointerTails[pointerTailIndex - i].GetComponent<PointerTailMGR>().NonDiagonal = false;      //��]�ɂȂ��Ă��Ȃ��̂ŁA�΂߂�������悤�ɂ���(�ꉞPointerTail���u���ꂽ���ɏ������s��)
                if (i == 1)
                {
                    pointerTails[pointerTailIndex - i].SetActive(false);              //�΂ߕ\���ɖ߂��ہA����PointerTail�̈�O��PointerTail�͔�A�N�e�B�u�ɂ���
                                                                                      //�ʂ̂Ƃ���ōs���鏈�������A���̏����Ƃ̏��Ԃ����ݍ���Ȃ��������̂ł����ɏ����Ă���
                                                                                      //�������v�����ǁA���}���u�I�ɂ�������̂Ō�X��肪�N���邩��
                }
            }
        }

    }

    private void ChangeToDiagonal(bool b)
    {
        //�΂߂ɂ��� 
        if (b)
        {
            children[0].SetActive(false);
            children[1].SetActive(true);
        }else  //���ɂ���
        {
            children[0].SetActive(true);
            children[1].SetActive(false);
        }

    }

    private void ChangeColor()
    {
        foreach(Vector2Int m in manualRoute)
        {
            int i = manualRoute.IndexOf(m); 
            if(m.x == manualRoute[pointerTailIndex].x && m.y == manualRoute[pointerTailIndex].y && i != pointerTailIndex && 
                pointerTails[i].transform.rotation.Equals(transform.rotation) && pointerTails[i].GetComponent<PointerTailMGR>().GetDiagonal() == GetDiagonal())
            {
                childRenderers[0].color = changedColor;
                childRenderers[1].color = changedColor;
                return;
            }
        }

        childRenderers[0].color = noChangedColor;
        childRenderers[1].color = noChangedColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTailMGR : MonoBehaviour
{
    [SerializeField] int pointerTailIndex;
    private List<Vector2Int> manualRoute;
    private List<GameObject> pointerTails;
    private MapData map;
    private int numOfChildren;
    private GameObject[] children;
    private SpriteRenderer[] childRenderers;
    private Vector2[] defaultPointerTailSizes;
    private Color noChangedColor;
    [SerializeField] Color changedColor;
    [SerializeField] Sprite[] firstTailSprites;
    [SerializeField] bool isCrossing;             //���ꂪtrue�̂Ƃ��͏I�_��Sprite��changedColor�ɂ���

    private int horizontal1Index = 0;
    private int horizontal2Index = 1;
    private int diagonal1Index = 2;
    private int diagonal2Index = 3;

    [SerializeField] bool nonDiagonal;     //���ꂪtrue�̂Ƃ��APointerTail�͎΂߂������Ȃ��悤�ɂ���i�ŏ��̉�]�p�j
    [SerializeField] bool isDiagonal;      //�΂߂������Ă�Ƃ��Atrue

    //�v���p�e�B
    public bool NonDiagonal
    {
        get { return nonDiagonal; }
        set { nonDiagonal = value; }
    }

    public bool GetIsDiagonal()
    {
        return isDiagonal;
    }
    public void SetIsDiagonal(bool b)
    {
        isDiagonal = b;
    }

    public bool GetIsCrossing()
    {
        return isCrossing;
    }

    public void SetIsCrossing(bool b)
    {
        isCrossing = b;
    }
    // Start is called before the first frame update
    void Start()
    {
        pointerTails = GameManager.instance.pointerMGR.GetPoinerTails();
        pointerTailIndex = pointerTails.IndexOf(gameObject);    //pointerTails��PointeTail�����ԂɎd�����Ă���̂ŁAIndex���擾���Ă���
        manualRoute = GameManager.instance.pointerMGR.GetManualRoute();
        map = GameManager.instance.mapMGR.GetMap();

        numOfChildren = 4;
        children = new GameObject[numOfChildren];
        childRenderers = new SpriteRenderer[numOfChildren];
        defaultPointerTailSizes = new Vector2[numOfChildren];
        for (int i = 0; i < numOfChildren; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            childRenderers[i] = children[i].GetComponent<SpriteRenderer>();
            defaultPointerTailSizes[i] = childRenderers[i].size;
            childRenderers[i].sortingOrder = pointerTailIndex;
        }
        noChangedColor = childRenderers[0].color;      //Inspector�̕��őS�q�I�u�W�F�N�g�̐F�𓝈ꂵ�Ēu��

        if(pointerTailIndex == 0)        //��ԍŏ���PointerTail�͎n�_��\������̂�Sprite��ς���
        {
            childRenderers[horizontal1Index].sprite = firstTailSprites[0];
            childRenderers[diagonal1Index].sprite = firstTailSprites[1];
            children[horizontal2Index].SetActive(false);
            children[diagonal2Index].SetActive(false);
        }
    }

    private void LateUpdate()
    {
        ChangeColor();
        if (pointerTailIndex < GameManager.instance.pointerMGR.GetPoinerTails().Count - 4)      //����PointerTail��������x�O�̂��̂������ꍇ�A�����͍s��Ȃ�
        {
            return;
        }
        RotatePointerTail();
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
                ChangeToDiagonal(true);                                   //�΂߂�Sprite��\������
                pointerTails[pointerTailIndex + 1].SetActive(false);      //������΂߂Ɍ�������ꍇ�A���̎���PointerTail�͕\������K�v���Ȃ��̂ŁA��A�N�e�B�u�ɂ���
                                                                          //���v���Ǝv�����ǁA���̎���PointerTail��Update����ɌĂ΂��ƍ���C������̂ŁA�ꉞ�����Ƃ���
                SetIsDiagonal(true);
                return;    //�΂߂��������Ƃ��͂�����return�����
            }
            else
            {
                pointerTails[pointerTailIndex + 1].SetActive(true);        //���ꂪ�΂߂������Ȃ��Ƃ��́A����PointerTail���A�N�e�B�u�ɂ���
                SetIsDiagonal(false);
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
        ChangeToDiagonal(false);                 //Sprite��Horizontal�ɂ���
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
            children[horizontal1Index].SetActive(false);
            children[horizontal2Index].SetActive(false);
            children[diagonal1Index].SetActive(true);
            children[diagonal2Index].SetActive(true);
        }else  //���ɂ���
        {
            children[horizontal1Index].SetActive(true);
            children[horizontal2Index].SetActive(true);
            children[diagonal1Index].SetActive(false);
            children[diagonal2Index].SetActive(false);
        }

    }

    private void ChangeColor()
    {
        ManageIsCrossing();
        for (int i = 0; i < manualRoute.Count; i++)
        {
            Vector2Int m = manualRoute[i];
            if(m.x == manualRoute[pointerTailIndex].x && m.y == manualRoute[pointerTailIndex].y && i < pointerTailIndex && pointerTails[i].activeSelf)       //PointerTail�����������Ƃ�
            {
                if (Vector3.Angle(pointerTails[i].transform.rotation.eulerAngles, transform.rotation.eulerAngles) % 180 == 0 && pointerTails[i].GetComponent<PointerTailMGR>().GetIsDiagonal() == GetIsDiagonal())    //PointerTail���d�Ȃ����Ƃ�
                {
                    foreach (SpriteRenderer sr in childRenderers)        //�������g���d�Ȃ��Ă���Ƃ��͑S�q�I�u�W�F�N�g��changedColor�ɂ���
                    {
                        sr.color = changedColor;
                    }
                    return;
                }
            }
        }

        childRenderers[horizontal1Index].color = noChangedColor;            //isCrossing�̂Ƃ���Horizotal2, Diagonal2��changedColor�ɂ������̂ŁA�����ł͂�����Ȃ�
        childRenderers[diagonal1Index].color = noChangedColor;
    }

    private void ManageIsCrossing()
    {
        if (GetIsCrossing())
        {
            childRenderers[horizontal2Index].color = changedColor;
            childRenderers[diagonal2Index].color = changedColor;
        }
        else          //Horizontal2, Diagonal2��noChangedColor�ɂ���̂͂�������
        {
            childRenderers[horizontal2Index].color = noChangedColor;
            childRenderers[diagonal2Index].color = noChangedColor;
        }

        for (int i = 0; i < manualRoute.Count; i++)
        {
            if (i >= pointerTailIndex) break;    //This����O��PointerTail�Əd�Ȃ��Ă��邩���m�肽���̂�

            Vector2Int m = manualRoute[i];
            if (m.x == manualRoute[pointerTailIndex].x && m.y == manualRoute[pointerTailIndex].y && pointerTails[i].activeSelf)       //PointerTail�����������Ƃ�
            {
                Debug.LogWarning("isCrossing:" + transform.position);
                if (pointerTails[pointerTailIndex - 1].activeSelf)       //��O��PointerTail��active�̂Ƃ�
                {
                    pointerTails[pointerTailIndex - 1].GetComponent<PointerTailMGR>().SetIsCrossing(true);
                }
                else         //��O��PointerTail��active�łȂ��Ƃ��i����O�̓_�Ǝ΂߂Ōq�����Ă���Ƃ��j
                {
                    pointerTails[pointerTailIndex - 2].GetComponent<PointerTailMGR>().SetIsCrossing(true);
                }
                return;
            }
        }
        if (pointerTailIndex > 0)
        {
            pointerTails[pointerTailIndex - 1].GetComponent<PointerTailMGR>().SetIsCrossing(false);
        }

    }
}

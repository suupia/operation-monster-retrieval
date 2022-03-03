using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PointerMGR : MonoBehaviour
{
    //private float moveInterval = 0.3f;
    //private float lastMovementTime;
    [SerializeField] private GameObject pointerTailPrefab;
    private List<GameObject> pointerTails;
    private List<PointerTailMGR> pointerTailMGRs;
    private List<int> nonDiagonalPoints;

    [SerializeField] List<Vector2Int> manualRoute;  //�΂߂ɐi�ނƂ�����΂߂ɂ��Ȃ���Ԃ�Route����������
    [SerializeField] List<Vector2Int> finalManualRoute;  //�΂߂ɐi�ނƂ���͎΂߂ɂ�����Ԃ�Route����������
    private bool isOnCastle;

    private Vector3 startPos;

    private void Start()
    {
        manualRoute = new List<Vector2Int>();
        finalManualRoute = new List<Vector2Int>();
        pointerTails = new List<GameObject>();
        nonDiagonalPoints = new List<int>();
        startPos = GameManager.instance.ToWorldPosition(GameManager.instance.mapMGR.GetAllysCastlePos());

        manualRoute.Add(GameManager.instance.mapMGR.GetAllysCastlePos());

    }

    private void Update()
    {
        if (transform.position != startPos && GameManager.instance.state == GameManager.State.ShowingResults)      //�퓬�I�����̏���
        {
            ResetPointer();
        }
    }

    //public void MoveByArrowKey(Vector2Int directionVector) //���L�[�ňړ�
    //{
    //    //position���O���b�h�ɍ��킹��
    //    Vector2Int gridPos = GameManager.instance.ToGridPosition(transform.position); //Update�ŌĂяo�����̂Ŗ���X�V�����
    //    transform.position = GameManager.instance.ToWorldPosition(gridPos);


    //    //arrowKeyFlag��ύX���鎞�ɖ���x�N�g�������蓖�ĂĂ���̂ŁA�����ł�������x�N�g���œ��͂̏󋵂𔻒�ł���B
    //    if (directionVector != Vector2Int.zero) //���L�[���������Ƃ�
    //    {
    //        if (GameManager.instance.inputMGR.ArrowKeyTimer - lastMovementTime >= moveInterval)
    //        {
    //            if (!(manualRoute.Count(pos => pos == gridPos + directionVector) < 2) && manualRoute[manualRoute.Count - 2] != gridPos + directionVector) //�i�����Ƃ��Ă���}�X������2��ȏ�ʂ��Ă��邩�A��������߂�Ȃ��ꍇ�͂Ȃɂ����Ȃ�
    //            {
    //                Debug.LogWarning("�����}�X��ʂ��̂�2��܂łł�");
    //                return;
    //            }

    //            if (GameManager.instance.mapMGR.GetMapValue(gridPos + directionVector) % GameManager.instance.groundID == 0)
    //            {
    //                transform.position = transform.position + new Vector3(directionVector.x, directionVector.y, 0);
    //                lastMovementTime = GameManager.instance.inputMGR.ArrowKeyTimer;
    //            }

    //        }
    //    }
    //    else //���L�[�𗣂����Ƃ�
    //    {
    //        //GameManager.instance.inputMGR.ArrowKeyTimer = 0;

    //        //ResetLastMovementTime();

    //    }

    //    ManagePlayerRouteList();
    //    ManageMouseTrails();
    //}

    //Getter
    public bool GetIsOnCastle()
    {
        return isOnCastle;
    }
    public List<Vector2Int> GetManualRoute()
    {
        return manualRoute;
    }

    public List<GameObject> GetPoinerTails()
    {
        return pointerTails;
    }
    public List<int> GetNonDiagonalPoints()
    {
        return nonDiagonalPoints;
    }

    public List<Vector2Int> GetFinalManualRoute()
    {
        return finalManualRoute;
    }
    public void SetFinalManualRoute()
    {
        finalManualRoute.Clear();
        nonDiagonalPoints.Clear();
        for (int i = 0; i < manualRoute.Count; i++)
        {
            if (i + 1 != manualRoute.Count)
            {
                if (pointerTails[i].activeSelf)
                {
                    finalManualRoute.Add(manualRoute[i]);

                    if (pointerTails[i].GetComponent<PointerTailMGR>().NonDiagonal)
                    {
                        nonDiagonalPoints.Add(i);
                    }
                }
            }
            else
            {
                finalManualRoute.Add(manualRoute[i]);
            }
        }
    }

    public void MoveByMouse(Vector2Int mouseGridPos) //�}�E�X�ňړ�
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position); //Update�ŌĂяo�����̂Ŗ���X�V�����

        SetFinalManualRoute();
        if ((mouseGridPos - pointerGridPos).magnitude > 1)       //mouse��pointer���אڂ��Ă��Ȃ��ꍇ�͂Ȃɂ����Ȃ�
        {
            Debug.Log($"�c���̂����ꂩ�ŗאڂ��Ă���}�X��I��ł��������@(mouseGridPos - pointerGridPos).magnitude:{(mouseGridPos - pointerGridPos).magnitude}");
            return;
        }

        if (finalManualRoute.Count(pos => pos == mouseGridPos) >= 2 && manualRoute[manualRoute.Count - 2] != mouseGridPos) //�i�����Ƃ��Ă���}�X������2��ȏ�ʂ��Ă��āA��������߂�Ȃ��ꍇ��return
        {
            Debug.Log("�����}�X��ʂ��̂�2��܂łł�");
            return;
        }

        if (finalManualRoute.Count(pos => pos == pointerGridPos) >= 2 && finalManualRoute[0] != pointerGridPos && finalManualRoute[finalManualRoute.IndexOf(pointerGridPos) - 1] == mouseGridPos && manualRoute[manualRoute.Count - 2] != mouseGridPos)    //�O�ɒʂ�����(�cor��)���t�����ɐi�����Ƃ��Ă��āA��������߂�Ȃ��ꍇ��return
        {
            Debug.LogWarning("����PointerTail���\������Ă���}�X���t����(�cor��)�ɐi�ނ��Ƃ͂ł��܂���");
            return;
        }

        if (manualRoute.Count >= 2 && finalManualRoute.Count(pos => pos == manualRoute[manualRoute.Count - 2]) >= 2 && finalManualRoute[0] != manualRoute[manualRoute.Count - 2] && finalManualRoute[finalManualRoute.IndexOf(manualRoute[manualRoute.Count - 2]) - 1] == mouseGridPos && manualRoute[manualRoute.Count - 2] != mouseGridPos)    //�O�ɒʂ�����(�cor��)���t�����ɐi�����Ƃ��Ă��āA��������߂�Ȃ��ꍇ��return
        {
            Debug.LogWarning("����PointerTail���\������Ă���}�X���t����(�΂�)�ɐi�ނ��Ƃ͂ł��܂���");
            return;
        }

        if (GameManager.instance.mapMGR.GetMapValue(mouseGridPos) % GameManager.instance.groundID != 0)
        {
            Debug.Log("ground����Ȃ��̂ňړ��s��");
            return;           //ground����Ȃ��̂ňړ��s��
        }
        transform.position = GameManager.instance.ToWorldPosition(mouseGridPos);       //Pointer���ړ�


        IsOnCastle();
        ManageManualRouteList();
        ManageMPointerTails();
        ManageIsCrossing();
    }
    private void ManageManualRouteList()
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position);

        if (manualRoute.Count >= 2 && manualRoute[manualRoute.Count - 2] == pointerGridPos)     //Pointer����߂�����route������ɍ��킹��
        {
            manualRoute.RemoveAt(manualRoute.Count - 1);
            Debug.Log($"manualRoute:{string.Join(",", manualRoute)}");
        }
        else if (manualRoute.Count == 0 || (manualRoute[manualRoute.Count - 1] != pointerGridPos && finalManualRoute.Count(pos => pos == pointerGridPos) < 2))           //finalManualRoute�Ɍ��݂�gridPos��2�ȏ�����Ă��Ȃ��ꍇ�AmanualRoute�ɒǉ�����
        {
            manualRoute.Add(pointerGridPos);
            Debug.Log($"manualRoute:{string.Join(",", manualRoute)}");
        }
    }


    private void ManageMPointerTails()
    {
        while (manualRoute.Count - 1 > pointerTails.Count)
        {
            pointerTails.Add(Instantiate(pointerTailPrefab, GameManager.instance.ToWorldPosition(manualRoute[pointerTails.Count]), pointerTailPrefab.transform.rotation));
            //pointerTailMGRs.Add(pointerTails[pointerTails.Count - 1].GetComponent<PointerTailMGR>());
            SetOrder();
        }

        while (manualRoute.Count - 1 < pointerTails.Count)
        {
            Destroy(pointerTails[pointerTails.Count - 1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
            //pointerTailMGRs.RemoveAt(pointerTails.Count - 1);
        }
    }

    private void IsOnCastle()
    {
        if ((int)transform.position.x >= GameManager.instance.mapMGR.GetEnemysCastlePos().x - 1 && (int)transform.position.y <= GameManager.instance.mapMGR.GetEnemysCastlePos().y + 1)
        {
            Debug.LogWarning("Pointer��Castle�ɓ��B���܂���");
            isOnCastle = true;
        }
        else
        {
            isOnCastle = false;
        }
        return;
    }

    public void ResetPointer()
    {
        manualRoute.Clear();
        finalManualRoute = new List<Vector2Int>(); //ManualRoute�ɎQ�Ƃ�n���Ă���̂ŁA���Ɏg�����߂�new����K�v������
        nonDiagonalPoints = new List<int>(); //����
        isOnCastle = false;
        transform.position = startPos;
        while (pointerTails.Count != 0)
        {
            Destroy(pointerTails[pointerTails.Count - 1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
        }
        manualRoute.Add(GameManager.instance.mapMGR.GetAllysCastlePos());
        Debug.Log($"Pointer��������:manualRoute={string.Join(",", manualRoute)}, isOnCastle={isOnCastle}, pointerTails={string.Join(",", pointerTails)}");
    }

    private void SetOrder()
    {
        this.GetComponent<SpriteRenderer>().sortingOrder = pointerTails.Count + 1;
    }

    private void ManageIsCrossing()
    {
        if (pointerTails.Count < 1) return;      //�ŏ��̓_�͕K�v�Ȃ�
        int lastPointerTailIndex = pointerTails.Count - 1;
        if (!pointerTails[lastPointerTailIndex].activeSelf)
        {
            lastPointerTailIndex--;
        }

        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position);

        for (int i = 0; i < manualRoute.Count; i++)
        {
            if (i >= manualRoute.Count - 1) break;    //�O�ɒʂ����_���ǂ����m�肽���̂�

            Vector2Int m = manualRoute[i];
            if (m.x == pointerGridPos.x && m.y == pointerGridPos.y)       //Poniter���APointerTail�����݂���_�ɂ���Ƃ�
            {
                if (pointerTails[i].activeSelf)       //���̓_�ɂ���PointerTail��active�̂Ƃ�
                {
                    pointerTails[lastPointerTailIndex].GetComponent<PointerTailMGR>().SetIsCrossing(true);
                    //Debug.LogWarning("isCrossing,Index:" + lastPointerTailIndex + ",manualIndex:" + i);
                    return;
                }
            }
        }
        pointerTails[lastPointerTailIndex].GetComponent<PointerTailMGR>().SetIsCrossing(false);
    }
}

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

    [SerializeField] List<Vector2Int> manualRoute;
    private bool isOnCastle;

    private Vector3 startPos;

    private void Start()
    {
        manualRoute = new List<Vector2Int>();
        pointerTails = new List<GameObject>();
        startPos = new Vector3(1.5f, 1.5f, 0);

        manualRoute.Add(new Vector2Int(1, 1));

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
    public void MoveByMouse(Vector2Int mouseGridPos) //�}�E�X�ňړ�
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position); //Update�ŌĂяo�����̂Ŗ���X�V�����


        if ((mouseGridPos - pointerGridPos).magnitude > 1)       //mouse��pointer���אڂ��Ă��Ȃ��ꍇ�͂Ȃɂ����Ȃ�
        {
            Debug.Log($"�c���̂����ꂩ�ŗאڂ��Ă���}�X��I��ł��������@(mouseGridPos - pointerGridPos).magnitude:{(mouseGridPos - pointerGridPos).magnitude}");
            return;
        }

        if (manualRoute.Count(pos => pos == mouseGridPos) >= 2 && manualRoute[manualRoute.Count - 2] != mouseGridPos)           //�i�����Ƃ��Ă���}�X������2��ȏ�ʂ��Ă��āA��������߂�Ȃ��ꍇ�͂Ȃɂ����Ȃ�
        {
            Debug.Log("�����}�X��ʂ��̂�2��܂łł�");
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
    }
    private void ManageManualRouteList()
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position);

        if (manualRoute.Count >= 2 && manualRoute[manualRoute.Count - 2] == pointerGridPos)     //Pointer����߂�����route������ɍ��킹��
        {
            manualRoute.RemoveAt(manualRoute.Count - 1);
            Debug.Log($"manualRoute:{string.Join(",", manualRoute)}");
        }
        else if (manualRoute.Count == 0 || (manualRoute[manualRoute.Count - 1] != pointerGridPos && manualRoute.Count(pos => pos == pointerGridPos) < 2))           //playerRoute�Ɍ��݂�gridPos��2�ȏ�����Ă��Ȃ��ꍇ�A�ǉ�����
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
        if(transform.position.x >= GameManager.instance.mapMGR.GetMapWidth()-3 && transform.position.y >= GameManager.instance.mapMGR.GetMapHeight()-3)
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
        manualRoute = new List<Vector2Int>();
        isOnCastle = false;
        transform.position = startPos;
        while(pointerTails.Count != 0)
        {
            Destroy(pointerTails[pointerTails.Count -1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
        }
        manualRoute.Add(new Vector2Int(1, 1));
        Debug.Log($"Pointer��������:manualRoute={string.Join(",", manualRoute)}, isOnCastle={isOnCastle}, pointerTails={string.Join(",", pointerTails)}");
    }
}

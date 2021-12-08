using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PointerMGR : MonoBehaviour
{
    private float moveInterval = 0.15f;
    private float lastMovingTime;
    [SerializeField] private GameObject pointerTailPrefab;
    private List<GameObject> pointerTails;

    [System.NonSerialized] public List<Vector2Int> playerRoute;


    private void Start()
    {
        lastMovingTime = -moveInterval;
        playerRoute = new List<Vector2Int>();
        pointerTails = new List<GameObject>();

    }

    public void MoveByArrowKey(Vector2Int directionVector) //���L�[�ňړ�
    {

        //position���O���b�h�ɍ��킹��
        Vector2Int gridPos = GameManager.instance.ToGridPosition(transform.position); //Update�ŌĂяo�����̂Ŗ���X�V�����
        transform.position = GameManager.instance.ToWorldPosition(gridPos);

        //arrowKeyFlag��ύX���鎞�ɖ���x�N�g�������蓖�ĂĂ���̂ŁA�����ł�������x�N�g���œ��͂̏󋵂𔻒�ł���B
        if (directionVector != Vector2Int.zero) //���L�[���������Ƃ�
        {
            if (GameManager.instance.inputMGR.ArrowKeyTimer - lastMovingTime >= moveInterval)
            {
                if (!(playerRoute.Count(pos => pos == gridPos + directionVector) < 2) && playerRoute[playerRoute.Count - 2] != gridPos + directionVector) //�i�����Ƃ��Ă���}�X������2��ȏ�ʂ��Ă��邩�A��������߂�Ȃ��ꍇ�͂Ȃɂ����Ȃ�
                {
                    Debug.LogWarning("�����}�X��ʂ��̂�2��܂łł�");
                    return;
                }

                if (GameManager.instance.mapMGR.GetMapValue(gridPos + directionVector) % GameManager.instance.groundID == 0)
                {
                    transform.position = transform.position + new Vector3(directionVector.x, directionVector.y, 0);
                    lastMovingTime = GameManager.instance.inputMGR.ArrowKeyTimer;
                }

            }
        }
        else //���L�[�𗣂����Ƃ�
        {
            GameManager.instance.inputMGR.ArrowKeyTimer = 0;

            lastMovingTime = -moveInterval;

        }

        ManagePlayerRouteList();
        ManageMouseTrails();
    }

    public void MoveByMouse(Vector2Int mouseGridPos) //�}�E�X�ňړ�
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position); //Update�ŌĂяo�����̂Ŗ���X�V�����


        if ((mouseGridPos - pointerGridPos).magnitude > 1)       //mouse��pointer���אڂ��Ă��Ȃ��ꍇ�͂Ȃɂ����Ȃ�
        {
            Debug.LogWarning($"�c���̂����ꂩ�ŗאڂ��Ă���}�X��I��ł��������@(mouseGridPos - pointerGridPos).magnitude:{(mouseGridPos - pointerGridPos).magnitude}");
            return;
        }

        if (playerRoute.Count(pos => pos == mouseGridPos) >= 2 && playerRoute[playerRoute.Count - 2] != mouseGridPos)           //�i�����Ƃ��Ă���}�X������2��ȏ�ʂ��Ă��āA��������߂�Ȃ��ꍇ�͂Ȃɂ����Ȃ�
        {
            Debug.LogWarning("�����}�X��ʂ��̂�2��܂łł�");
            return;
        }

        if (GameManager.instance.mapMGR.GetMapValue(mouseGridPos) % GameManager.instance.groundID != 0)
        {
            Debug.LogWarning("ground����Ȃ��̂ňړ��s��");
            return;           //ground����Ȃ��̂ňړ��s��
        }
        transform.position = GameManager.instance.ToWorldPosition(mouseGridPos);

        ManagePlayerRouteList();
        ManageMouseTrails();
    }
    private void ManagePlayerRouteList()
    {
        Vector2Int pointerGridPos = GameManager.instance.ToGridPosition(transform.position);


        if (playerRoute.Count >= 2 && playerRoute[playerRoute.Count - 2] == pointerGridPos)     //MouseTrailer����߂�����route������ɍ��킹��
        {
            playerRoute.RemoveAt(playerRoute.Count - 1);
            Debug.Log(ListToString(playerRoute, "playerRoute"));

        }
        else if (playerRoute.Count == 0 || (playerRoute[playerRoute.Count - 1] != pointerGridPos && playerRoute.Count(pos => pos == pointerGridPos) < 2))           //playerRoute�Ɍ��݂�gridPos�������Ă��Ȃ��ꍇ�A�ǉ�����
        {
            playerRoute.Add(pointerGridPos);
            Debug.Log(ListToString(playerRoute, "playerRoute"));
        }
    }


    private void ManageMouseTrails()
    {
        while (playerRoute.Count > pointerTails.Count)
        {
            pointerTails.Add(Instantiate(pointerTailPrefab, GameManager.instance.ToWorldPosition(playerRoute[pointerTails.Count]), pointerTailPrefab.transform.rotation));
        }

        while (playerRoute.Count < pointerTails.Count)
        {
            Destroy(pointerTails[pointerTails.Count - 1]);
            pointerTails.RemoveAt(pointerTails.Count - 1);
        }
    }
    private string ListToString(List<Vector2Int> list, string listName)
    {
        string sentece = listName;
        foreach (Vector2Int i in list)
        {
            sentece += i;
        }
        return sentece;
    }
}

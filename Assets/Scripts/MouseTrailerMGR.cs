using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrailerMGR : MonoBehaviour
{
    private bool mouse;

    private float count = 0;
    private float moveInterval = 0.15f;
    private float lastMovingTime;
    private bool pressingArrowKey;
    [SerializeField] private GameObject mouseTrailPrefab;

    private List<GameObject> mouseTrails;

    [System.NonSerialized] public List<Vector2Int> playerRoute;
    // Start is called before the first frame update
    void Start()
    {
        lastMovingTime = -moveInterval;
        playerRoute = new List<Vector2Int>();
        mouseTrails = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mouse)
        {
            MoveByMouse();
        }
        else
        {
            MoveByArrowKey();
        }

        //���L�[�������Ă���ԁA���Ԃ𑪂�
        if (pressingArrowKey)
        {
            count += Time.deltaTime;
        }

        ManagePlayerRouteList();
        ManageMouseTrails();
    }

    void MoveByArrowKey()            //���L�[�ňړ�
    {

        //position���O���b�h�ɍ��킹��
        Vector2Int gridPos = GameManager.instance.ToGridPosition(transform.position);
        transform.position = GameManager.instance.ToWorldPosition(gridPos);

        //1�}�X���ړ�
        if (count - lastMovingTime >= moveInterval)
        {
            if (Input.GetKey(KeyCode.DownArrow) && GameManager.instance.mapMGR.GetMapValue(gridPos + Vector2Int.down) % GameManager.instance.groundID == 0)
            {
                transform.position = transform.position + Vector3.down;
                pressingArrowKey = true;
                lastMovingTime = count;
            }
            else if (Input.GetKey(KeyCode.UpArrow) && GameManager.instance.mapMGR.GetMapValue(gridPos + Vector2Int.up) % GameManager.instance.groundID == 0)
            {
                transform.position = transform.position + Vector3.up;
                pressingArrowKey = true;
                lastMovingTime = count;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && GameManager.instance.mapMGR.GetMapValue(gridPos + Vector2Int.left) % GameManager.instance.groundID == 0)
            {
                transform.position = transform.position + Vector3.left;
                pressingArrowKey = true;
                lastMovingTime = count;
            }
            else if (Input.GetKey(KeyCode.RightArrow) && GameManager.instance.mapMGR.GetMapValue(gridPos + Vector2Int.right) % GameManager.instance.groundID == 0)
            {
                transform.position = transform.position + Vector3.right;
                pressingArrowKey = true;
                lastMovingTime = count;
            }
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            count = 0;
            lastMovingTime = -moveInterval;

            pressingArrowKey = false;
        }
    }

    void MoveByMouse()               //�}�E�X�ňړ�
    {
        if (!Input.GetMouseButton(1))     //�E�N���b�N���Ă��Ȃ��ꍇ�͂Ȃɂ����Ȃ�
        {
            return;
        }            
        Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);

        if((GameManager.instance.ToGridPosition(mousePos) - GameManager.instance.ToGridPosition(transform.position)).magnitude > Mathf.Sqrt(2))       //MouseTrailer��Mouse���אڂ��Ă��Ȃ��ꍇ�͂Ȃɂ����Ȃ�
        {
            Debug.LogWarning("�c���΂߂̂����ꂩ�ŗאڂ��Ă���}�X��I��ł�������");
            return;
        }
        transform.position = GameManager.instance.ToWorldPosition(GameManager.instance.ToGridPosition(mousePos));


    }
    private void ManagePlayerRouteList()
    {
        Vector2Int trailerGridPos = GameManager.instance.ToGridPosition(transform.position);

        //playerRoute�Ɍ��݂�gridPos�������Ă��Ȃ��ꍇ�A�ǉ�����
        if (!playerRoute.Contains(trailerGridPos))
        {
            playerRoute.Add(trailerGridPos);

            Debug.LogWarning(ListToString(playerRoute, "playerRoute"));
        }
        else if (playerRoute.Count >= 2 && playerRoute[playerRoute.Count - 2] == trailerGridPos)     //MouseTrailer����߂�����route������ɍ��킹��
        {
            playerRoute.RemoveAt(playerRoute.Count - 1);
            Debug.LogWarning(ListToString(playerRoute, "playerRoute"));

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

    private void ManageMouseTrails()
    {
        while(playerRoute.Count > mouseTrails.Count)
        {
            mouseTrails.Add(Instantiate(mouseTrailPrefab, GameManager.instance.ToWorldPosition(playerRoute[mouseTrails.Count]), mouseTrailPrefab.transform.rotation));
        }

        while (playerRoute.Count < mouseTrails.Count)
        {
            Destroy(mouseTrails[mouseTrails.Count - 1]);
            mouseTrails.RemoveAt(mouseTrails.Count - 1);
        }
    }
}

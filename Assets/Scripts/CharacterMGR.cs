using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterMGR : MonoBehaviour
{
    Rigidbody2D rd2D;
    BoxCollider2D boxCollider2D;
    Animator animator;

    [SerializeField] Vector2Int gridPos;

    [SerializeField] int thisCharacterID;

    public bool isAttacking = false;
    public bool isMoving = false;

    [SerializeField] int level;
    [SerializeField] int maxHp;
    [SerializeField] int hp;
    [SerializeField] int atk;
    [SerializeField] float attackInterval;  
    [SerializeField] float attackRange;
    [SerializeField] int spd;
    [SerializeField] int coolTime;

    private GameObject damageTextGO;
    [SerializeField] private Text damageText;
    [SerializeField] private int drawDamageTime;

    private Direction direction;

    private enum Direction
    {
        Front,
        Back,
        Right,
        Left,
        DiagRightFront,
        DiagLeftFront,
        DiagRightBack,
        DiagLeftBack
    }

    private void Start()
    {
        gridPos = new Vector2Int(Mathf.FloorToInt(transform.position.x - 0.5f), Mathf.FloorToInt(transform.position.y - 0.5f)) ;
    }

    //public bool CanMove(int horizontalInput, int verticalInput)
    //{
    //    Vector2Int endGridPos = gridPos + new Vector2Int(horizontalInput, verticalInput);

    //    if (GameManager.instance.mapMGR.GetMapValue(endGridPos) % DateMGR.instance.wallID == 0
    //        || GameManager.instance.mapMGR.GetMapValue(endGridPos) % DateMGR.instance.wallID == 0
    //        || GameManager.instance.mapMGR.GetMapValue(endGridPos) % GameManager.instance.enemyID == 0)
    //    {
    //        return false;
    //    }

    //    //�΂߈ړ��̎��Ƀu���b�N�̊p���ړ����邱�Ƃ͂ł��Ȃ�
    //    if (horizontalInput != 0 && verticalInput != 0)
    //    {
    //        //���������̔���
    //        if (GameManager.instance.internalData.GetValue(gridPos.x + horizontalInput, gridPos.y) % GameManager.instance.wallID == 0)
    //        {
    //            return false;
    //        }

    //        //���������̔���
    //        if (GameManager.instance.internalData.GetValue(gridPos.x, gridPos.y + verticalInput) % GameManager.instance.wallID == 0)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //public void Move()
    //{
    //    Vector2Int directionVector = GetDirectionVector();

    //    hMovementAmount += directionVector.x;
    //    vMovementAmount += directionVector.y;

    //    MoveDate(directionVector);

    //    if (!isMoving)
    //    {
    //        StartCoroutine(MoveCoroutine());
    //    }

    //}

    //public void MoveDate(Vector2Int directionVector)
    //{
    //    //internalDate���ړ�������
    //    if (this is PlayerController)
    //    {
    //        GameManager.instance.internalData.MoveInternalData(gridPos, gridPos + directionVector, GameManager.instance.playerID);

    //    }
    //    else
    //    {
    //        GameManager.instance.internalData.MoveInternalData(gridPos, gridPos + directionVector, GameManager.instance.enemyID);
    //    }

    //    //gridPos���ړ�������B����͍Ō�ɍs�����Ƃɒ��ӁI
    //    gridPos += directionVector;

    //}

    //IEnumerator MoveCoroutine()  //Player��������蓮�����֐�
    //{
    //    Vector2 startPos;
    //    Vector2 endPos;
    //    int horizontalInput;
    //    int verticalInput;

    //    if (isAttacking)
    //    {
    //        yield return null;
    //    }

    //    isMoving = true;

    //    while (hMovementAmount != 0 || vMovementAmount != 0)
    //    {
    //        horizontalInput = 0;
    //        verticalInput = 0;

    //        if (hMovementAmount != 0)
    //        {
    //            horizontalInput = hMovementAmount / Mathf.Abs(hMovementAmount);
    //        }

    //        if (vMovementAmount != 0)
    //        {
    //            verticalInput = vMovementAmount / Mathf.Abs(vMovementAmount);
    //        }

    //        startPos = transform.position;
    //        endPos = GetTransformPosFromGridPos();


    //        float remainingDistance = (endPos - startPos).sqrMagnitude;

    //        while (remainingDistance > float.Epsilon)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, endPos, 1f / moveTime * Time.deltaTime);
    //            //3�ڂ̈�����"1�t���[���̍ő�ړ�����"�@�P�ʂ͎���[m/s](�R���[�`����1�t���[��������Ă��邩��Time.deltaTime��������BmoveTime�o��������1�}�X�i�ށB)

    //            remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

    //            yield return null;  //1�t���[����~������B
    //        }
    //        transform.position = endPos;//���[�v�𔲂������͂�������ړ�������B

    //        hMovementAmount -= horizontalInput;
    //        vMovementAmount -= verticalInput;
    //    }


    //    isMoving = false;
    //    //Debug.Log($"MoveCoroutine()�I������endPos��{endPos}");
    //}

}

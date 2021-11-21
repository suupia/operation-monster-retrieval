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

    //    //斜め移動の時にブロックの角を移動することはできない
    //    if (horizontalInput != 0 && verticalInput != 0)
    //    {
    //        //水平方向の判定
    //        if (GameManager.instance.internalData.GetValue(gridPos.x + horizontalInput, gridPos.y) % GameManager.instance.wallID == 0)
    //        {
    //            return false;
    //        }

    //        //垂直方向の判定
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
    //    //internalDateを移動させる
    //    if (this is PlayerController)
    //    {
    //        GameManager.instance.internalData.MoveInternalData(gridPos, gridPos + directionVector, GameManager.instance.playerID);

    //    }
    //    else
    //    {
    //        GameManager.instance.internalData.MoveInternalData(gridPos, gridPos + directionVector, GameManager.instance.enemyID);
    //    }

    //    //gridPosを移動させる。これは最後に行うことに注意！
    //    gridPos += directionVector;

    //}

    //IEnumerator MoveCoroutine()  //Playerをゆっくり動かす関数
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
    //            //3つ目の引数は"1フレームの最大移動距離"　単位は実質[m/s](コルーチンが1フレームずつ回っているからTime.deltaTimeが消える。moveTime経った時に1マス進む。)

    //            remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

    //            yield return null;  //1フレーム停止させる。
    //        }
    //        transform.position = endPos;//ループを抜けた時はきっちり移動させる。

    //        hMovementAmount -= horizontalInput;
    //        vMovementAmount -= verticalInput;
    //    }


    //    isMoving = false;
    //    //Debug.Log($"MoveCoroutine()終了時のendPosは{endPos}");
    //}

}

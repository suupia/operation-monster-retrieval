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
    [SerializeField] Vector2Int targetPos;

    [SerializeField] int thisCharacterID;

    public bool isAttacking = false;
    public bool isMoving = false;
    Vector2Int movementVector;

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
    private State state;
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
    private enum State
    {
        Marching,
        InBattle
    }
    //Getter
    public Vector2Int GetDirectionVector()
    {
        Vector2Int resultVector2Int = new Vector2Int(0, 0);
        switch (direction)
        {
            case Direction.Back:
                resultVector2Int = new Vector2Int(0, 1);
                break;

            case Direction.DiagLeftBack:
                resultVector2Int = new Vector2Int(-1, 1);
                break;

            case Direction.Left:
                resultVector2Int = new Vector2Int(-1, 0);
                break;

            case Direction.DiagLeftFront:
                resultVector2Int = new Vector2Int(-1, -1);
                break;

            case Direction.Front:
                resultVector2Int = new Vector2Int(0, -1);
                break;

            case Direction.DiagRightFront:
                resultVector2Int = new Vector2Int(1, -1);
                break;

            case Direction.Right:
                resultVector2Int = new Vector2Int(1, 0);
                break;

            case Direction.DiagRightBack:
                resultVector2Int = new Vector2Int(1, 1);
                break;
        }
        if (resultVector2Int == new Vector2Int(0, 0))
        {
            Debug.LogError("GetDirectionVector()の戻り値が(0,0)になっています");
        }
        return resultVector2Int;
    }

    //Setter
    public void SetDirection(Vector2 directionVector)
    {
        if (directionVector == Vector2.zero) //引数の方向ベクトルがゼロベクトルの時は何もしない
        {
            return;
        }

        float angle = Vector2.SignedAngle(Vector2.right, directionVector);
        //Debug.Log($"SetDirectionのangleは{angle}です");


        //先に画像の向きを決定する
        if (directionVector.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); //元の画像が左向きのため
        }
        else if (directionVector.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        //directionとanimationを決定する
        if (-22.5f <= angle && angle < 22.5f)
        {
            direction = Direction.Right;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", 0);
        }
        else if (22.5f <= angle && angle < 67.5f)
        {
            direction = Direction.DiagRightBack;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", 1);
        }
        else if (67.5f <= angle && angle < 112.5f)

        {
            direction = Direction.Back;
            animator.SetBool("Horizontal", false);
            animator.SetInteger("Vertical", 1);
        }
        else if (112.5f <= angle && angle < 157.5f)
        {
            direction = Direction.DiagLeftBack;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", 1);
        }
        else if (-157.5f <= angle && angle < -112.5f)
        {
            direction = Direction.DiagLeftFront;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", -1);
        }
        else if (-112.5f <= angle && angle < -67.5f)
        {
            direction = Direction.Front;
            animator.SetBool("Horizontal", false);
            animator.SetInteger("Vertical", -1);
        }
        else if (-67.5f <= angle && angle < -22.5f)
        {
            direction = Direction.DiagRightFront;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", -1);
        }
        else //角度は-180から180までで端点は含まないらしい。そのため、Direction.Leftはelseで処理することにした。
        {
            direction = Direction.Left;
            animator.SetBool("Horizontal", true);
            animator.SetInteger("Vertical", 0);
        }
    }


    private void Start()
    {
        gridPos = new Vector2Int(Mathf.FloorToInt(transform.position.x - 0.5f), Mathf.FloorToInt(transform.position.y - 0.5f)) ;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Marching:
                March();
                break;
            case State.InBattle:
                Battle();
                break;
        }
    }

    //public void March()
    //{
    //    TargetNearestTower();

    //    if (true) //キャラクターの進行するモードによって行動が変わる
    //    {
    //        MoveShortestRoute();
    //    }
    //    else
    //    {
    //        //プレイヤーが進路を選択する
    //    }
    //}

    //public void TargetNearestTower()
    //{
    //    //
    //    //最も近いタワーの座標を取得する
    //    //

    //    targetPos = new Vector2Int(8,3); //仮
    //}
    //public void MoveShortestRoute()
    //{

    //}

    //public bool CanMove(Vector2Int vector)
    //{
    //    if (GameManager.instance.mapMGR.GetMapValue(gridPos + vector) % GameManager.instance.wallID == 0)
    //    {
    //        Debug.LogWarning("移動先にwallIDがあるため、移動できません");
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //}
    //public void Move()
    //{
    //    Vector2Int directionVector = GetDirectionVector();

    //    movementVector += directionVector;

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

    //    if (!(GameManager.instance.mapMGR.GetMapValue(gridPos) % GameManager.instance.characterID == 0))
    //    {
    //        Debug.LogError("MoveDateで移動前のmapValueにcharacterIDが含まれていません");
    //    }
    //    GameManager.instance.mapMGR.MultiplySetMapValue(gridPos,);


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

    //public void ChasePlayerAvoidingWall()
    //{
    //    //前に動けないことは確定していることに注意
    //    Vector2Int leftFrontGridPos = GameManager.instance.ToGridPosition(GetTransformPosFromGridPos() + GameManager.instance.RotateVector(GetDirectionVector(), 45));
    //    Vector2Int rightFrontGridPos = GameManager.instance.ToGridPosition(GetTransformPosFromGridPos() + GameManager.instance.RotateVector(GetDirectionVector(), -45));

    //    int horizontalInput;
    //    int verticalInput;

    //    switch (direction)
    //    {
    //        case Direction.Back:
    //        case Direction.Left:
    //        case Direction.Front:
    //        case Direction.Right:
    //            if (GameManager.instance.internalData.GetValue(leftFrontGridPos) % GameManager.instance.groundID == 0 ||
    //                GameManager.instance.internalData.GetValue(leftFrontGridPos) % GameManager.instance.aisleID == 0)
    //            {
    //                Vector2Int leftGridPos = GameManager.instance.ToGridPosition(GetTransformPosFromGridPos() + GameManager.instance.RotateVector(GetDirectionVector(), 90));
    //                if (GameManager.instance.internalData.GetValue(leftGridPos) % GameManager.instance.groundID == 0 ||
    //                GameManager.instance.internalData.GetValue(leftGridPos) % GameManager.instance.aisleID == 0)
    //                {
    //                    horizontalInput = Mathf.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), 90).x);
    //                    verticalInput = Mathf.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), 90).y);

    //                    if (!CanMove(horizontalInput, verticalInput))
    //                    {
    //                        return;
    //                    }
    //                    SetDirection(new Vector2(horizontalInput, verticalInput));
    //                    Move();
    //                    //Debug.Log($"Move({horizontalInput},{verticalInput})を実行しました");
    //                }
    //            }

    //            if (GameManager.instance.internalData.GetValue(rightFrontGridPos) % GameManager.instance.groundID == 0 ||
    //                GameManager.instance.internalData.GetValue(rightFrontGridPos) % GameManager.instance.aisleID == 0)
    //            {
    //                Vector2Int rightGridPos = GameManager.instance.ToGridPosition(GetTransformPosFromGridPos() + GameManager.instance.RotateVector(GetDirectionVector(), -90));
    //                if (GameManager.instance.internalData.GetValue(rightGridPos) % GameManager.instance.groundID == 0 ||
    //                GameManager.instance.internalData.GetValue(rightFrontGridPos) % GameManager.instance.aisleID == 0)
    //                {
    //                    horizontalInput = Mathf.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), -90).x);
    //                    verticalInput = Mathf.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), -90).y);

    //                    if (!CanMove(horizontalInput, verticalInput))
    //                    {
    //                        return;
    //                    }
    //                    SetDirection(new Vector2(horizontalInput, verticalInput));
    //                    Move();
    //                    //Debug.Log($"Move({horizontalInput},{verticalInput})を実行しました");
    //                }
    //            }

    //            break;

    //        case Direction.DiagLeftBack:
    //        case Direction.DiagLeftFront:
    //        case Direction.DiagRightFront:
    //        case Direction.DiagRightBack:
    //            if (GameManager.instance.internalData.GetValue(leftFrontGridPos) % GameManager.instance.groundID == 0 ||
    //                GameManager.instance.internalData.GetValue(leftFrontGridPos) % GameManager.instance.aisleID == 0)
    //            {
    //                horizontalInput = Mathf.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), 45).x);
    //                verticalInput = Mathf.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), 45).y);
    //                if (!CanMove(horizontalInput, verticalInput))
    //                {
    //                    return;
    //                }
    //                SetDirection(new Vector2(horizontalInput, verticalInput));
    //                Move();
    //                //Debug.Log($"Move({horizontalInput},{verticalInput})を実行しました");
    //            }
    //            if (GameManager.instance.internalData.GetValue(rightFrontGridPos) % GameManager.instance.groundID == 0 ||
    //                GameManager.instance.internalData.GetValue(rightFrontGridPos) % GameManager.instance.aisleID == 0)
    //            {
    //                horizontalInput = Mathf.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), -45).x);
    //                verticalInput = Mathf.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), -45).y);
    //                if (!CanMove(horizontalInput, verticalInput))
    //                {
    //                    return;
    //                }
    //                SetDirection(new Vector2(horizontalInput, verticalInput));
    //                Move();
    //                //Debug.Log($"Move({horizontalInput},{verticalInput})を実行しました");
    //            }

    //            break;
    //    }

    //}


    public void Battle()
    {
        Debug.LogWarning("Battleを実行します");
    }
}

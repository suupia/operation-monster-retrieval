using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterMGR : MonoBehaviour
{
    Animator animator;

    [SerializeField] Vector2Int gridPos;

    [SerializeField] Vector2Int targetGridPos;
    [SerializeField] Vector2Int directionVectorToTarget;

    [SerializeField] int thisCharacterID;

    public bool isAttacking = false;
    public bool isMoving = false;

    [SerializeField] int level;
    [SerializeField] int maxHp;
    [SerializeField] int hp;
    [SerializeField] int atk;
    [SerializeField] float attackInterval;
    [SerializeField] float attackRange;
    [SerializeField] int spd; //1秒間に進むマスの数 [マス/s]  とりあえず１にしておく
    float moveTime; // movetime = 1/spd [s]
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
    public Vector2Int GetGridPos()
    {
        return gridPos;
    }
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
    public Vector2 GetTransformPosFromGridPos()
    {
        return GameManager.instance.ToWorldPosition(gridPos);
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
        animator = GetComponent<Animator>();

        moveTime = 1 / spd;
        gridPos = GameManager.instance.ToGridPosition(transform.position);
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

    public void March()
    {

        if (true) //キャラクターの進行するモードによって行動が変わる
        {
            //オートモード

            TargetNearestTower();


            if (!isMoving) //向きは立ち止まっているときのみ、変わる
            {
                TurnToTarget();
                if (CalcDistanceToTarget() > Mathf.Sqrt(2))
                {
                    TurnToTheDirectionCharacterCanMove();
                }
            }

            if (CanMove(GetDirectionVector()) && CalcDistanceToTarget() > Mathf.Sqrt(2))
            {
                MoveForward();
            }
            else
            {
                return;
            }
        }
        else
        {
            //プレイヤーが進路を選択する
        }
    }

    public void TargetNearestTower()
    {
        //最も近いタワーの座標を取得する

        int lookingForValue =1; //索敵範囲
        int notLookingForValue =0; //索敵範囲外
        int centerValue=0; //原点


        int[,] searchRangeArray;
        int maxRange =5; //今は仮に５としておく

        for (int k = 0; k < maxRange; k++)
        {
            searchRangeArray = CalcSearchRangeArray(k,lookingForValue,notLookingForValue,centerValue);
            for(int j = 0; j < searchRangeArray.GetLength(0); j++)
            {
                for(int i = 0; i < searchRangeArray.GetLength(1); i++)
                {
                    //lookingForValueのとき、タワーがあるかどうか判定する
                }
            }
        }


        targetGridPos = new Vector2Int(8, 3); //仮
    }

    public bool CanMove(Vector2Int vector)
    {
        if (GameManager.instance.mapMGR.GetMapValue(gridPos + vector) % GameManager.instance.wallID == 0)
        {
            Debug.Log($"移動先にwallIDがあるため、移動できません(gridPos:{gridPos}vector:{vector})\nGameManager.instance.mapMGR.GetMapValue(gridPos + vector)={GameManager.instance.mapMGR.GetMapValue(gridPos + vector)} GetDirectionVector={GetDirectionVector()}");
            return false;
        }

        //斜め移動の時にブロックの角を移動することはできない
        if (vector.x != 0 && vector.y != 0)
        {
            //水平方向の判定
            if (GameManager.instance.mapMGR.GetMapValue(gridPos.x + vector.x, gridPos.y) % GameManager.instance.wallID == 0)
            {
                return false;
            }

            //垂直方向の判定
            if (GameManager.instance.mapMGR.GetMapValue(gridPos.x, gridPos.y + vector.y) % GameManager.instance.wallID == 0)
            {
                return false;
            }
        }

        return true;
    }
    public void MoveForward()
    {
        Debug.Log("MoveForwardを実行します");
        if (!isMoving)
        {
            StartCoroutine(MoveForwardCoroutine());
            
        }
    }

    IEnumerator MoveForwardCoroutine()  //Characterをゆっくり動かす関数
    {
        Debug.Log("MoveCoroutineを実行します");
        Vector2 startPos;
        Vector2 endPos;


        if (isAttacking)
        {
            yield return null;
        }

        isMoving = true;


        MoveDate(GetDirectionVector()); //先にMoveDateを行う

        startPos = transform.position;
        endPos = GetTransformPosFromGridPos();


        float remainingDistance = (endPos - startPos).sqrMagnitude;

        while (remainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, 1f / moveTime * Time.deltaTime);
            //3つ目の引数は"1フレームの最大移動距離"　単位は実質[m/s](コルーチンが1フレームずつ回っているからTime.deltaTimeが消える。moveTime経った時に1マス進む。)

            remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

            yield return null;  //1フレーム停止させる。
        }
        transform.position = endPos;//ループを抜けた時はきっちり移動させる。



        isMoving = false;
        //Debug.Log($"MoveCoroutine()終了時のendPosは{endPos}");
    }
    public void MoveDate(Vector2Int directionVector)
    {

        if (!(GameManager.instance.mapMGR.GetMapValue(gridPos) % GameManager.instance.characterID == 0))
        {
            Debug.LogError("MoveDateで移動前のmapValueにcharacterIDが含まれていません");
            return;
        }

        GameManager.instance.mapMGR.DivisionalSetMapValue(gridPos, GameManager.instance.characterID);
        GameManager.instance.mapMGR.MultiplySetMapValue(gridPos + directionVector, GameManager.instance.characterID);


        //gridPosを移動させる。これは最後に行うことに注意！
        gridPos += directionVector;

    }


    public void TurnToTarget() //ターゲットの方を向く
    {
        //Debug.Log($"TurnToTheTarget()を開始します");
        directionVectorToTarget = targetGridPos - gridPos;
        SetDirection(directionVectorToTarget);
    }
    public void TurnToTheDirectionCharacterCanMove() //動ける方向に向きを変える
    {
        if (CanMove(GetDirectionVector()))
        {
            return; //前に動ける時は、特に何もしない
        }

        //前に動けないことは確定していることに注意
        Vector2Int leftFrontGridPos = GameManager.instance.ToGridPosition(GetTransformPosFromGridPos() + GameManager.instance.RotateVector(GetDirectionVector(), 45));
        Vector2Int rightFrontGridPos = GameManager.instance.ToGridPosition(GetTransformPosFromGridPos() + GameManager.instance.RotateVector(GetDirectionVector(), -45));

        Vector2Int vectorInput;

        switch (direction)
        {
            case Direction.Back:
            case Direction.Left:
            case Direction.Front:
            case Direction.Right:
                if (GameManager.instance.mapMGR.GetMapValue(leftFrontGridPos) % GameManager.instance.groundID == 0)
                {
                    Vector2Int leftGridPos = GameManager.instance.ToGridPosition(GetTransformPosFromGridPos() + GameManager.instance.RotateVector(GetDirectionVector(), 90));
                    if (GameManager.instance.mapMGR.GetMapValue(leftGridPos) % GameManager.instance.groundID == 0)
                    {
                        vectorInput = Vector2Int.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), 90));

                        if (!CanMove(vectorInput))
                        {
                            return;
                        }
                        SetDirection(vectorInput);
                        //Move();
                        //Debug.Log($"Move({horizontalInput},{verticalInput})を実行しました");
                    }
                }

                if (GameManager.instance.mapMGR.GetMapValue(rightFrontGridPos) % GameManager.instance.groundID == 0)
                {
                    Vector2Int rightGridPos = GameManager.instance.ToGridPosition(GetTransformPosFromGridPos() + GameManager.instance.RotateVector(GetDirectionVector(), -90));
                    if (GameManager.instance.mapMGR.GetMapValue(rightGridPos) % GameManager.instance.groundID == 0)
                    {
                        vectorInput = Vector2Int.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), -90));

                        if (!CanMove(vectorInput))
                        {
                            return;
                        }
                        SetDirection(vectorInput);
                        //Move();
                        //Debug.Log($"Move({horizontalInput},{verticalInput})を実行しました");
                    }
                }

                break;

            case Direction.DiagLeftBack:
            case Direction.DiagLeftFront:
            case Direction.DiagRightFront:
            case Direction.DiagRightBack:
                if (GameManager.instance.mapMGR.GetMapValue(leftFrontGridPos) % GameManager.instance.groundID == 0)
                {
                    vectorInput = Vector2Int.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), 45));

                    if (!CanMove(vectorInput))
                    {
                        return;
                    }
                    SetDirection(vectorInput);
                    //Move();
                    //Debug.Log($"Move({horizontalInput},{verticalInput})を実行しました");
                }
                if (GameManager.instance.mapMGR.GetMapValue(rightFrontGridPos) % GameManager.instance.groundID == 0)
                {
                    vectorInput = Vector2Int.RoundToInt(GameManager.instance.RotateVector(GetDirectionVector(), -45));

                    if (!CanMove(vectorInput))
                    {
                        return;
                    }
                    SetDirection(vectorInput);
                    //Move();
                    //Debug.Log($"Move({horizontalInput},{verticalInput})を実行しました");
                }

                break;
        }

    }

    public float CalcDistanceToTarget()
    {
        return (gridPos - targetGridPos).magnitude;
    }


    public void Battle()
    {
        Debug.LogWarning("Battleを実行します");
    }

    public int[,] CalcSearchRangeArray(int advancingDistance,int lookingForValue,int notLookingForValue, int centerValue)
    {
        int t = lookingForValue; //索敵範囲
        int f = notLookingForValue; //索敵範囲外
        int o = centerValue; //原点

        int size = 2*(advancingDistance + 1) + 1;
        int[,] resultArray = new int[size, size];

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (i + j == advancingDistance || i + j == advancingDistance + 1 //左下
                   || i - j == advancingDistance + 1 || i - j == advancingDistance + 2 //右下
                   || -i + j == advancingDistance + 1 || -i + j == advancingDistance + 2 //左下
                   || i + j == 3 * (advancingDistance + 1) || i + j == 3 * (advancingDistance + 1) + 1 //右上
                    )
                {
                    resultArray[i, j] = t;
                }
                else if (i == advancingDistance + 1 && j == advancingDistance + 1)
                {
                    resultArray[i, j] = o;
                }
                else
                {
                    resultArray[i, j] = f;
                }
            }
        }
        return resultArray;
    }
}
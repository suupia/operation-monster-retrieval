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
            Debug.LogError("GetDirectionVector()�̖߂�l��(0,0)�ɂȂ��Ă��܂�");
        }
        return resultVector2Int;
    }

    //Setter
    public void SetDirection(Vector2 directionVector)
    {
        if (directionVector == Vector2.zero) //�����̕����x�N�g�����[���x�N�g���̎��͉������Ȃ�
        {
            return;
        }

        float angle = Vector2.SignedAngle(Vector2.right, directionVector);
        //Debug.Log($"SetDirection��angle��{angle}�ł�");


        //��ɉ摜�̌��������肷��
        if (directionVector.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); //���̉摜���������̂���
        }
        else if (directionVector.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        //direction��animation�����肷��
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
        else //�p�x��-180����180�܂łŒ[�_�͊܂܂Ȃ��炵���B���̂��߁ADirection.Left��else�ŏ������邱�Ƃɂ����B
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

    //    if (true) //�L�����N�^�[�̐i�s���郂�[�h�ɂ���čs�����ς��
    //    {
    //        MoveShortestRoute();
    //    }
    //    else
    //    {
    //        //�v���C���[���i�H��I������
    //    }
    //}

    //public void TargetNearestTower()
    //{
    //    //
    //    //�ł��߂��^���[�̍��W���擾����
    //    //

    //    targetPos = new Vector2Int(8,3); //��
    //}
    //public void MoveShortestRoute()
    //{

    //}

    //public bool CanMove(Vector2Int vector)
    //{
    //    if (GameManager.instance.mapMGR.GetMapValue(gridPos + vector) % GameManager.instance.wallID == 0)
    //    {
    //        Debug.LogWarning("�ړ����wallID�����邽�߁A�ړ��ł��܂���");
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
    //    //internalDate���ړ�������
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
    //        Debug.LogError("MoveDate�ňړ��O��mapValue��characterID���܂܂�Ă��܂���");
    //    }
    //    GameManager.instance.mapMGR.MultiplySetMapValue(gridPos,);


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

    //public void ChasePlayerAvoidingWall()
    //{
    //    //�O�ɓ����Ȃ����Ƃ͊m�肵�Ă��邱�Ƃɒ���
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
    //                    //Debug.Log($"Move({horizontalInput},{verticalInput})�����s���܂���");
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
    //                    //Debug.Log($"Move({horizontalInput},{verticalInput})�����s���܂���");
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
    //                //Debug.Log($"Move({horizontalInput},{verticalInput})�����s���܂���");
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
    //                //Debug.Log($"Move({horizontalInput},{verticalInput})�����s���܂���");
    //            }

    //            break;
    //    }

    //}


    public void Battle()
    {
        Debug.LogWarning("Battle�����s���܂�");
    }
}

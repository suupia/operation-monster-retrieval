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
    [SerializeField] int spd; //1�b�Ԃɐi�ރ}�X�̐� [�}�X/s]  �Ƃ肠�����P�ɂ��Ă���
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
            Debug.LogError("GetDirectionVector()�̖߂�l��(0,0)�ɂȂ��Ă��܂�");
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

        if (true) //�L�����N�^�[�̐i�s���郂�[�h�ɂ���čs�����ς��
        {
            //�I�[�g���[�h

            TargetNearestTower();


            if (!isMoving) //�����͗����~�܂��Ă���Ƃ��̂݁A�ς��
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
            //�v���C���[���i�H��I������
        }
    }

    public void TargetNearestTower()
    {
        //�ł��߂��^���[�̍��W���擾����

        int lookingForValue =1; //���G�͈�
        int notLookingForValue =0; //���G�͈͊O
        int centerValue=0; //���_


        int[,] searchRangeArray;
        int maxRange =5; //���͉��ɂT�Ƃ��Ă���

        for (int k = 0; k < maxRange; k++)
        {
            searchRangeArray = CalcSearchRangeArray(k,lookingForValue,notLookingForValue,centerValue);
            for(int j = 0; j < searchRangeArray.GetLength(0); j++)
            {
                for(int i = 0; i < searchRangeArray.GetLength(1); i++)
                {
                    //lookingForValue�̂Ƃ��A�^���[�����邩�ǂ������肷��
                }
            }
        }


        targetGridPos = new Vector2Int(8, 3); //��
    }

    public bool CanMove(Vector2Int vector)
    {
        if (GameManager.instance.mapMGR.GetMapValue(gridPos + vector) % GameManager.instance.wallID == 0)
        {
            Debug.Log($"�ړ����wallID�����邽�߁A�ړ��ł��܂���(gridPos:{gridPos}vector:{vector})\nGameManager.instance.mapMGR.GetMapValue(gridPos + vector)={GameManager.instance.mapMGR.GetMapValue(gridPos + vector)} GetDirectionVector={GetDirectionVector()}");
            return false;
        }

        //�΂߈ړ��̎��Ƀu���b�N�̊p���ړ����邱�Ƃ͂ł��Ȃ�
        if (vector.x != 0 && vector.y != 0)
        {
            //���������̔���
            if (GameManager.instance.mapMGR.GetMapValue(gridPos.x + vector.x, gridPos.y) % GameManager.instance.wallID == 0)
            {
                return false;
            }

            //���������̔���
            if (GameManager.instance.mapMGR.GetMapValue(gridPos.x, gridPos.y + vector.y) % GameManager.instance.wallID == 0)
            {
                return false;
            }
        }

        return true;
    }
    public void MoveForward()
    {
        Debug.Log("MoveForward�����s���܂�");
        if (!isMoving)
        {
            StartCoroutine(MoveForwardCoroutine());
            
        }
    }

    IEnumerator MoveForwardCoroutine()  //Character��������蓮�����֐�
    {
        Debug.Log("MoveCoroutine�����s���܂�");
        Vector2 startPos;
        Vector2 endPos;


        if (isAttacking)
        {
            yield return null;
        }

        isMoving = true;


        MoveDate(GetDirectionVector()); //���MoveDate���s��

        startPos = transform.position;
        endPos = GetTransformPosFromGridPos();


        float remainingDistance = (endPos - startPos).sqrMagnitude;

        while (remainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, 1f / moveTime * Time.deltaTime);
            //3�ڂ̈�����"1�t���[���̍ő�ړ�����"�@�P�ʂ͎���[m/s](�R���[�`����1�t���[��������Ă��邩��Time.deltaTime��������BmoveTime�o��������1�}�X�i�ށB)

            remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

            yield return null;  //1�t���[����~������B
        }
        transform.position = endPos;//���[�v�𔲂������͂�������ړ�������B



        isMoving = false;
        //Debug.Log($"MoveCoroutine()�I������endPos��{endPos}");
    }
    public void MoveDate(Vector2Int directionVector)
    {

        if (!(GameManager.instance.mapMGR.GetMapValue(gridPos) % GameManager.instance.characterID == 0))
        {
            Debug.LogError("MoveDate�ňړ��O��mapValue��characterID���܂܂�Ă��܂���");
            return;
        }

        GameManager.instance.mapMGR.DivisionalSetMapValue(gridPos, GameManager.instance.characterID);
        GameManager.instance.mapMGR.MultiplySetMapValue(gridPos + directionVector, GameManager.instance.characterID);


        //gridPos���ړ�������B����͍Ō�ɍs�����Ƃɒ��ӁI
        gridPos += directionVector;

    }


    public void TurnToTarget() //�^�[�Q�b�g�̕�������
    {
        //Debug.Log($"TurnToTheTarget()���J�n���܂�");
        directionVectorToTarget = targetGridPos - gridPos;
        SetDirection(directionVectorToTarget);
    }
    public void TurnToTheDirectionCharacterCanMove() //����������Ɍ�����ς���
    {
        if (CanMove(GetDirectionVector()))
        {
            return; //�O�ɓ����鎞�́A���ɉ������Ȃ�
        }

        //�O�ɓ����Ȃ����Ƃ͊m�肵�Ă��邱�Ƃɒ���
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
                        //Debug.Log($"Move({horizontalInput},{verticalInput})�����s���܂���");
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
                        //Debug.Log($"Move({horizontalInput},{verticalInput})�����s���܂���");
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
                    //Debug.Log($"Move({horizontalInput},{verticalInput})�����s���܂���");
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
                    //Debug.Log($"Move({horizontalInput},{verticalInput})�����s���܂���");
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
        Debug.LogWarning("Battle�����s���܂�");
    }

    public int[,] CalcSearchRangeArray(int advancingDistance,int lookingForValue,int notLookingForValue, int centerValue)
    {
        int t = lookingForValue; //���G�͈�
        int f = notLookingForValue; //���G�͈͊O
        int o = centerValue; //���_

        int size = 2*(advancingDistance + 1) + 1;
        int[,] resultArray = new int[size, size];

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (i + j == advancingDistance || i + j == advancingDistance + 1 //����
                   || i - j == advancingDistance + 1 || i - j == advancingDistance + 2 //�E��
                   || -i + j == advancingDistance + 1 || -i + j == advancingDistance + 2 //����
                   || i + j == 3 * (advancingDistance + 1) || i + j == 3 * (advancingDistance + 1) + 1 //�E��
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
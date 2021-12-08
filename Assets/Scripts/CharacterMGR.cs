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

    [SerializeField] int characterTypeID;
    [SerializeField] int level;
    [SerializeField] int maxHp;
    [SerializeField] int hp;
    [SerializeField] int atk;
    [SerializeField] float attackInterval;
    [SerializeField] float attackRange;
    [SerializeField] float spd; //1�b�Ԃɐi�ރ}�X�̐� [�}�X/s]  �Ƃ肠�����P�ɂ��Ă���
    float moveTime; // movetime = 1/spd [s]
    [SerializeField] int coolTime;

    public bool isAttacking = false;
    public bool isMoving = false;

    bool isFristMarch = true;
    bool isFristBattle = true;

    AutoRouteData autoRoute;
    Vector2Int[] autoRouteArray;



    private GameObject damageTextGO;
    [SerializeField] private Text damageText;
    [SerializeField] private int drawDamageTime;

    private Direction direction;
    private State _state;
    private State state
    {
        get { return _state; }
        set
        {
            switch (value)
            {
                case State.Marching:
                    isFristMarch = true;
                    break;
                case State.InBattle:
                    isFristBattle = true;
                    break;
            }
            _state = value;
        }
    }

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
    public void SetCharacterData(int characterTypeID)  //hp��atk�Ȃǂ̏��������ŃZ�b�g����B
    {
        autoRoute = GameManager.instance.autoRouteDatas[characterTypeID];
    }
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
        state = State.Marching;
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

    public void March() //Update()�ŌĂ΂�邱�Ƃɒ���
    {

        if (true) //�L�����N�^�[�̐i�s���郂�[�h�ɂ���čs�����ς��
        {
            //�I�[�g���[�h
            if (isFristMarch)
            {
                isFristMarch = false;

                TargetNearestTower();
                Debug.Log($"targetGridPos={targetGridPos}");
                SearchAutoRoute();
            }

            MoveAlongWith(autoRouteArray);

        }
        else
        {
            //�v���C���[���i�H��I������
            //���̓e�X�g�p�̔z��
            Vector2Int[] route = { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(1, 3), new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4) };
            MoveAlongWith(route);
        }
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


        MoveData(GetDirectionVector()); //���MoveDate���s��

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
    public void MoveData(Vector2Int directionVector)
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

    public void TargetNearestTower() //�ł��߂��^���[�̍��W���擾����
    {


        int lookingForValue = 1; //���G�͈͂̒l
        int notLookingForValue = 0; //���G�͈͊O�̒l
        int centerValue = 0; //���_�̒l

        Vector2Int vector; //���[�v���Ŏg���A(i,j)�����[���h���W�ɒ���������
        List<Vector2Int> nearestTowerList = new List<Vector2Int>();

        int[,] searchRangeArray;
        int maxRange = System.Math.Max(GameManager.instance.mapMGR.GetMapWidth(), GameManager.instance.mapMGR.GetMapHeight()); //�T������͈͂�map�̏c���̍ő�l�܂Œ��ׂ�Ώ\��


        //Tower�̈ʒu��List�ɒǉ�����
        for (int k = 0; k < maxRange; k++) //k�͒��S�̃}�X���牽�}�X�܂ŕ����邩��\��
        {
            //Debug.Log($"{k}��ڂ̃��[�v���J�n���܂�");

            searchRangeArray = CalcSearchRangeArray(k, lookingForValue, notLookingForValue, centerValue);
            for (int j = 0; j < searchRangeArray.GetLength(0); j++)
            {
                for (int i = 0; i < searchRangeArray.GetLength(1); i++)
                {
                    vector = new Vector2Int(gridPos.x - (k + 1) + i, gridPos.y - (k + 1) + j); //���[���h���W�ɕϊ�����

                    if (vector.x < 0 || vector.y < 0 || vector.x > GameManager.instance.mapMGR.GetMapWidth() || vector.y > GameManager.instance.mapMGR.GetMapHeight())
                    {
                        continue;
                    }

                    if (searchRangeArray[i, j] == lookingForValue && GameManager.instance.mapMGR.GetMapValue(vector) % GameManager.instance.towerID == 0)
                    {
                        nearestTowerList.Add(vector);
                    }
                }
            }

            if (nearestTowerList.Count > 0)
            {
                Debug.Log($"nearestTowerList[0]={nearestTowerList[0]}");
                break;
            }
        }

        //List�̒��g���\�[�g����
        nearestTowerList.Sort((a, b) => b.y - a.y); //�܂�y���W�Ɋւ��č~���Ń\�[�g����
        nearestTowerList.Sort((a, b) => b.x - a.x); //����x���W�Ɋւ��č~���Ń\�[�g����

        targetGridPos = nearestTowerList[0];
    }
    public int[,] CalcSearchRangeArray(int advancingDistance, int lookingForValue, int notLookingForValue, int centerValue)
    {
        int t = lookingForValue; //���G�͈�
        int f = notLookingForValue; //���G�͈͊O
        int o = centerValue; //���_

        int size = 2 * (advancingDistance + 1) + 1;
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

    public void SearchAutoRoute()
    {
        autoRouteArray = autoRoute.SearchShortestRoute(gridPos, targetGridPos); ; //�Q�Ɠn��
    }


    public void MoveAlongWith(Vector2Int[] route) //�z��Ŏw�肵�����[�g�ɉ����Ă̈ړ�
    {
        Vector2Int prePos, nextPos,nextNextPos;

        if (isMoving)
        {
            return;
        }

        prePos = GetGridPos();

        for(int i = 0; i < route.Length; i++)
        {
            if (prePos != route[i])
            {
                if (i == route.Length - 1)
                {
                    Debug.Log("�w�肵�����[�g�Ɍ��݂�gridPos���܂܂�Ă��܂���B");
                }
                continue;
            }
            else if (i == route.Length - 1)
            {
                Debug.Log("�w�肵�����[�g�̏I�_�ɂ��܂��B");
                return;
            }

            nextPos = route[i + 1];

            if (i < route.Length - 2)  //���΂߈ړ��ł���Ƃ��͂�������B
            {
                nextNextPos = route[i + 2];
                if(((prePos-nextPos).x ==0 && (nextPos - nextNextPos).y == 0) || ((prePos - nextPos).y == 0 && (nextPos - nextNextPos).x == 0)) //nextPos���p�}�X�̂Ƃ�true
                {
                    if (CanMove(nextNextPos - prePos))
                    {
                        nextPos = nextNextPos;
                    }
                }
            }

            if ((prePos.x - nextPos.x > 1)||(prePos.x - nextPos.x < -1)|| (prePos.y - nextPos.y > 1) || (prePos.y - nextPos.y < -1))
            {
                Debug.Log("���݂̃}�X�ƈړ���̃}�X���אڂ��Ă��܂���Bpre:" + prePos + ",next:" + nextPos);
                return;
            }

            if (!isMoving)
            {
                SetDirection(nextPos - prePos);
            }

            if (CanMove(nextPos - prePos))
            {
                MoveForward();
            }
            return;
        }
    }

    public void Battle()
    {
        Debug.LogWarning("Battle�����s���܂�");
    }
}



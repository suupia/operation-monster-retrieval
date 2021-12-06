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

    public AutoRouteData GetAutoRoute()
    {
        return autoRoute;
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

        autoRoute = new AutoRouteData(GameManager.instance.mapMGR.GetMapWidth(), GameManager.instance.mapMGR.GetMapHeight()); ;

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
                SearchAutoRoute();

            }

            Debug.Log($"targetGridPos={targetGridPos}");


            //if (!isMoving) //�����͗����~�܂��Ă���Ƃ��̂݁A�ς��
            //{
            //    TurnToTarget();
            //    if (CalcDistanceToTarget() > Mathf.Sqrt(2))
            //    {
            //        TurnToTheDirectionCharacterCanMove();
            //    }
            //    else
            //    {
            //        TurnToTarget();
            //        state = State.InBattle;
            //        return;
            //    }
            //}

            



            //if (CanMove(GetDirectionVector()) && CalcDistanceToTarget() > Mathf.Sqrt(2))
            //{
            //    MoveForward();
            //}


        }
        else
        {
            //�v���C���[���i�H��I������
            //���̓e�X�g�p�̔z��
            Vector2Int[] route = { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(1, 3), new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4) };
            MoveAlongWith(route);
        }
    }

    public void TargetNearestTower() //�ł��߂��^���[�̍��W���擾����
    {
        

        int lookingForValue = 1; //���G�͈͂̒l
        int notLookingForValue = 0; //���G�͈͊O�̒l
        int centerValue = 0; //���_�̒l

        Vector2Int vector; //���[�v���Ŏg���A(i,j)�����[���h���W�ɒ���������
        List<Vector2Int> nearestTowerList = new List<Vector2Int>();

        int[,] searchRangeArray;
        int maxRange = System.Math.Max(GameManager.instance.mapMGR.GetMapWidth(),GameManager.instance.mapMGR.GetMapHeight()); //�T������͈͂�map�̏c���̍ő�l�܂Œ��ׂ�Ώ\��


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

                    if (vector.x < 0 || vector.y<0 || vector.x > GameManager.instance.mapMGR.GetMapWidth() || vector.y > GameManager.instance.mapMGR.GetMapHeight())
                    {
                        continue;
                    }

                    if (searchRangeArray[i, j] == lookingForValue && GameManager.instance.mapMGR.GetMapValue(vector) % GameManager.instance.towerID ==0)
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
        nearestTowerList.Sort((a,b) => b.y - a.y); //�܂�y���W�Ɋւ��č~���Ń\�[�g����
        nearestTowerList.Sort((a, b) => b.x - a.x); //����x���W�Ɋւ��č~���Ń\�[�g����

        targetGridPos = nearestTowerList[0];
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
        //�܂�map���R�s�[���āA�����Ȃ��ꏊ��-1�ɂ���B
        for(int y = 0; y < autoRoute.Height; y++)
        {
            for(int x=0;x<autoRoute.Width; x++)
            {
                if (GameManager.instance.mapMGR.GetMap().GetValue(x,y) % GameManager.instance.wallID == 0)
                {
                    autoRoute.SetWall(x,y);
                }
            }
        }

      autoRoute.SearchShortestRoute(gridPos,targetGridPos);
        
    }

    public void MoveAlongWith(Vector2Int[] route) //�z��Ŏw�肵�����[�g�ɉ����Ă̈ړ�
    {
        Vector2Int prePos, nextPos,nextNextPos;
        for(int i = 0; i < route.Length; i++)
        {
            prePos = GetGridPos();
            if (prePos != route[i])
            {
                if (i == route.Length)
                {
                    Debug.Log("�w�肵�����[�g�Ɍ��݂�gridPos���܂܂�Ă��܂���B");
                }
                continue;
            }
            else if (i == route.Length)
            {
                Debug.Log("�w�肵�����[�g�̏I�_�ɂ��܂��B");
                return;
            }

            nextPos = route[i + 1];

            if (i < route.Length - 1)  //���΂߈ړ��ł���Ƃ��͂�������B
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

public class AutoRouteData
{
    int _width;
    int _height;
    int[] _values = null;
    int _initiValue = -10;
    int _wallValue = -1; 
    int _errorValue = -88;


    //�R���X�g���N�^
    public AutoRouteData(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("RouteSearchData�̕��܂��͍�����0�ȉ��ɂȂ��Ă��܂�");
            return;
        }
        _width = width;
        _height = height;
        _values = new int[width * height];

        FillAll(_initiValue); //map�̏�������_initiValue�ōs��
    }

    //�v���p�e�B
    public int Width { get { return _width; } } 
    public int Height { get { return _height; } }

    //Getter
    public int GetLength()
    {
        return _values.Length;
    }
    public int GetValue(int x, int y)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"�̈�O�̒l���擾���悤�Ƃ��܂���(x,y)=({x},{y})");
            return _errorValue;
        }
        if (IsOnTheEdge(x, y))
        {
            Debug.Log($"IsOnTheEdge({x},{y})��true�ł�");
            return _wallValue;
        }
        return _values[ToSubscript(x, y)];
    }
    public int GetValue(Vector2Int vector)
    {
        return GetValue(vector.x, vector.y);
    }
    public int GetValue(int index)
    {
        if (index < 0 || index > _values.Length)
        {
            Debug.LogError("�̈�O�̒l���K�����悤�Ƃ��܂���");
            return _errorValue;
        }
        return _values[index];
    }

    //Setter
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError("�̈�O�ɒl��ݒ肵�悤�Ƃ��܂���");
            return;
        }
        _values[ToSubscript(x, y)] = value;
    }

    public void SetValue(Vector2Int vector, int value)
    {
        SetValue(vector.x, vector.y, value);
    }
    public void SetWall(int x,int y)
    {
        SetValue(x,y,_wallValue);
    }
    //�Y������ϊ�����
    int ToSubscript(int x, int y)
    {
        return x + (y * _width);
    }

    public Vector2Int DivideSubscript(int subscript)
    {
        int xSub = subscript % _width;
        int ySub = (subscript - xSub) / _width; //�����͊���Z
        return new Vector2Int(xSub, ySub);
    }

    bool IsOutOfRange(int x, int y)
    {
        if (x < -1 || x > _width) { return true; }
        if (y < -1 || y > _height) { return true; }

        //map�̒�
        return false;
    }

    bool IsOnTheEdge(int x, int y)
    {
        if (x == -1 || x == _width) { return true; }
        if (y == -1 || y == _height) { return true; }
        return false;
    }
    
    public void FillAll(int value) //edgeValue�܂ł͏����������Ȃ����Ƃɒ���
    {
        for (int j = 0; j < _height; j++)
        {
            for (int i = 0; i < _width; i++)
            {
                _values[ToSubscript(i, j)] = value;
            }
        }
    }

    public void SearchShortestRoute(Vector2Int startPos, Vector2Int endPos)
    {
        Queue<Vector2Int> que = new Queue<Vector2Int>();
        int i = 1; //1����n�܂邱�Ƃɒ���
        bool isComplete = false;

        SetValue(startPos,0); //startPos�̕����������͂̔�����s��Ȃ����߁A�����Ōʂɐݒ肷��
        que.Enqueue(startPos);

        while (!isComplete)
        {
            int loopNum = que.Count; //�O�̃��[�v�ŃL���[�ɒǉ����ꂽ���𐔂���
            Debug.LogWarning($"i:{i}�̂Ƃ�loopNum:{loopNum}");
            for (int k = 0; k < loopNum; k++) 
            {
                Debug.LogWarning($"PlaceNum({que.Peek()})�����s���܂�");
                PlaceNumAround(que.Dequeue());
            }
            i++; //�O�̃��[�v�ŃL���[�ɒǉ����ꂽ�������������ꂽ��A�C���f�b�N�X�𑝂₵�Ď��̃��[�v�Ɉڂ�

            if (i > 100) //�������[�v�h���p
            {
                isComplete = true;
                Debug.LogError("SearchShortestRoute��while���Ń��[�v��100��s���Ă��܂��܂���");
            }
        }

        void PlaceNumAround(Vector2Int centerPos)
        {
            Vector2Int inspectPos;

            //9�}�X���肷��i�^�񒆂̃}�X�̔���͕K�v�Ȃ��j
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    inspectPos = centerPos + new Vector2Int(x, y);
                    if (GetValue(inspectPos) == _initiValue && CanMove(centerPos,inspectPos))
                    {
                        SetValue(inspectPos, i);
                        que.Enqueue(inspectPos);
                        Debug.LogWarning($"({inspectPos})��{i}�ɂ��A�L���[�ɒǉ����܂����B");
                    }
                    if(inspectPos == endPos)
                    {
                        isComplete = true;
                    }
                }
            }
        }

        bool CanMove(Vector2Int prePos,Vector2Int afterPos)
        {
            Vector2Int directionVector = afterPos - prePos;
            if (GetValue(afterPos) != _initiValue)
            {
                return false;
            }

            //�΂߈ړ��̎��Ƀu���b�N�̊p���ړ����邱�Ƃ͂ł��Ȃ�
            if (directionVector.x != 0 && directionVector.y != 0)
            {
                //���������̔���
                if (GetValue(prePos.x + directionVector.x, prePos.y) != _initiValue)
                {
                    return false;
                }

                //���������̔���
                if (GetValue(prePos.x, prePos.y + directionVector.y) != _initiValue)
                {
                    return false;
                }
            }

            return true;
        }

    }
}

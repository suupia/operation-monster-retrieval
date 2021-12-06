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
    [SerializeField] float spd; //1秒間に進むマスの数 [マス/s]  とりあえず１にしておく
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
            Debug.LogError("GetDirectionVector()の戻り値が(0,0)になっています");
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

    public void March() //Update()で呼ばれることに注意
    {

        if (true) //キャラクターの進行するモードによって行動が変わる
        {
            //オートモード
            if (isFristMarch)
            {
                isFristMarch = false;
                TargetNearestTower();
                SearchAutoRoute();

            }

            Debug.Log($"targetGridPos={targetGridPos}");


            //if (!isMoving) //向きは立ち止まっているときのみ、変わる
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
            //プレイヤーが進路を選択する
            //↓はテスト用の配列
            Vector2Int[] route = { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(1, 3), new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4) };
            MoveAlongWith(route);
        }
    }

    public void TargetNearestTower() //最も近いタワーの座標を取得する
    {
        

        int lookingForValue = 1; //索敵範囲の値
        int notLookingForValue = 0; //索敵範囲外の値
        int centerValue = 0; //原点の値

        Vector2Int vector; //ループ内で使い、(i,j)をワールド座標に直したもの
        List<Vector2Int> nearestTowerList = new List<Vector2Int>();

        int[,] searchRangeArray;
        int maxRange = System.Math.Max(GameManager.instance.mapMGR.GetMapWidth(),GameManager.instance.mapMGR.GetMapHeight()); //探索する範囲はmapの縦横の最大値まで調べれば十分


        //Towerの位置をListに追加する
        for (int k = 0; k < maxRange; k++) //kは中心のマスから何マスまで歩けるかを表す
        {
            //Debug.Log($"{k}回目のループを開始します");

            searchRangeArray = CalcSearchRangeArray(k, lookingForValue, notLookingForValue, centerValue);
            for (int j = 0; j < searchRangeArray.GetLength(0); j++)
            {
                for (int i = 0; i < searchRangeArray.GetLength(1); i++)
                {
                    vector = new Vector2Int(gridPos.x - (k + 1) + i, gridPos.y - (k + 1) + j); //ワールド座標に変換する

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

        //Listの中身をソートする
        nearestTowerList.Sort((a,b) => b.y - a.y); //まずy座標に関して降順でソートする
        nearestTowerList.Sort((a, b) => b.x - a.x); //次にx座標に関して降順でソートする

        targetGridPos = nearestTowerList[0];
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


        MoveData(GetDirectionVector()); //先にMoveDateを行う

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
    public void MoveData(Vector2Int directionVector)
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

    public int[,] CalcSearchRangeArray(int advancingDistance, int lookingForValue, int notLookingForValue, int centerValue)
    {
        int t = lookingForValue; //索敵範囲
        int f = notLookingForValue; //索敵範囲外
        int o = centerValue; //原点

        int size = 2 * (advancingDistance + 1) + 1;
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

    public void SearchAutoRoute()
    {
        //まずmapをコピーして、動けない場所を-1にする。
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

    public void MoveAlongWith(Vector2Int[] route) //配列で指定したルートに沿っての移動
    {
        Vector2Int prePos, nextPos,nextNextPos;
        for(int i = 0; i < route.Length; i++)
        {
            prePos = GetGridPos();
            if (prePos != route[i])
            {
                if (i == route.Length)
                {
                    Debug.Log("指定したルートに現在のgridPosが含まれていません。");
                }
                continue;
            }
            else if (i == route.Length)
            {
                Debug.Log("指定したルートの終点にいます。");
                return;
            }

            nextPos = route[i + 1];

            if (i < route.Length - 1)  //↓斜め移動できるときはそうする。
            {
                nextNextPos = route[i + 2];
                if(((prePos-nextPos).x ==0 && (nextPos - nextNextPos).y == 0) || ((prePos - nextPos).y == 0 && (nextPos - nextNextPos).x == 0)) //nextPosが角マスのときtrue
                {
                    if (CanMove(nextNextPos - prePos))
                    {
                        nextPos = nextNextPos;
                    }
                }
            }

            if ((prePos.x - nextPos.x > 1)||(prePos.x - nextPos.x < -1)|| (prePos.y - nextPos.y > 1) || (prePos.y - nextPos.y < -1))
            {
                Debug.Log("現在のマスと移動先のマスが隣接していません。pre:" + prePos + ",next:" + nextPos);
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
        Debug.LogWarning("Battleを実行します");
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


    //コンストラクタ
    public AutoRouteData(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("RouteSearchDataの幅または高さが0以下になっています");
            return;
        }
        _width = width;
        _height = height;
        _values = new int[width * height];

        FillAll(_initiValue); //mapの初期化は_initiValueで行う
    }

    //プロパティ
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
            Debug.LogError($"領域外の値を取得しようとしました(x,y)=({x},{y})");
            return _errorValue;
        }
        if (IsOnTheEdge(x, y))
        {
            Debug.Log($"IsOnTheEdge({x},{y})がtrueです");
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
            Debug.LogError("領域外の値を習得しようとしました");
            return _errorValue;
        }
        return _values[index];
    }

    //Setter
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError("領域外に値を設定しようとしました");
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
    //添え字を変換する
    int ToSubscript(int x, int y)
    {
        return x + (y * _width);
    }

    public Vector2Int DivideSubscript(int subscript)
    {
        int xSub = subscript % _width;
        int ySub = (subscript - xSub) / _width; //ここは割り算
        return new Vector2Int(xSub, ySub);
    }

    bool IsOutOfRange(int x, int y)
    {
        if (x < -1 || x > _width) { return true; }
        if (y < -1 || y > _height) { return true; }

        //mapの中
        return false;
    }

    bool IsOnTheEdge(int x, int y)
    {
        if (x == -1 || x == _width) { return true; }
        if (y == -1 || y == _height) { return true; }
        return false;
    }
    
    public void FillAll(int value) //edgeValueまでは書き換えられないことに注意
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
        int i = 1; //1から始まることに注意
        bool isComplete = false;

        SetValue(startPos,0); //startPosの部分だけ周囲の判定を行わないため、ここで個別に設定する
        que.Enqueue(startPos);

        while (!isComplete)
        {
            int loopNum = que.Count; //前のループでキューに追加された個数を数える
            Debug.LogWarning($"i:{i}のときloopNum:{loopNum}");
            for (int k = 0; k < loopNum; k++) 
            {
                Debug.LogWarning($"PlaceNum({que.Peek()})を実行します");
                PlaceNumAround(que.Dequeue());
            }
            i++; //前のループでキューに追加された文を処理しきれたら、インデックスを増やして次のループに移る

            if (i > 100) //無限ループ防ぐ用
            {
                isComplete = true;
                Debug.LogError("SearchShortestRouteのwhile文でループが100回行われてしまいました");
            }
        }

        void PlaceNumAround(Vector2Int centerPos)
        {
            Vector2Int inspectPos;

            //9マス判定する（真ん中のマスの判定は必要ない）
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    inspectPos = centerPos + new Vector2Int(x, y);
                    if (GetValue(inspectPos) == _initiValue && CanMove(centerPos,inspectPos))
                    {
                        SetValue(inspectPos, i);
                        que.Enqueue(inspectPos);
                        Debug.LogWarning($"({inspectPos})を{i}にし、キューに追加しました。");
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

            //斜め移動の時にブロックの角を移動することはできない
            if (directionVector.x != 0 && directionVector.y != 0)
            {
                //水平方向の判定
                if (GetValue(prePos.x + directionVector.x, prePos.y) != _initiValue)
                {
                    return false;
                }

                //垂直方向の判定
                if (GetValue(prePos.x, prePos.y + directionVector.y) != _initiValue)
                {
                    return false;
                }
            }

            return true;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterMGR : MonoBehaviour
{
    Animator animator;

    [SerializeField] Vector2Int gridPos;

    [SerializeField] Vector2Int targetCastlePos;
    [SerializeField] Vector2Int targetTowerPos;
    [SerializeField] Vector2Int directionVectorToTarget;

    [SerializeField] Facility targetFacility;

    [SerializeField] int characterTypeID;
    [SerializeField] int level;
    [SerializeField] int maxHp;
    [SerializeField] int hp;
    [SerializeField] int atk;
    [SerializeField] float attackInterval;
    [SerializeField] int attackRange;
    [SerializeField] float spd; //1秒間に進むマスの数 [マス/s]  とりあえず１にしておく
    float moveTime; // movetime = 1/spd [s]
    [SerializeField] int coolTime;

    bool isMarching = false;
    bool isAttacking = false;
    bool isMoving = false;

    bool isFristMarch = true;
    bool isFristBattle = true;

    AutoRouteData autoRoute;
    List<Vector2Int> routeList;

    int moveAlongWithCounter;



    GameObject damageTextGO;
    [SerializeField] private Text damageText;
    [SerializeField] private int drawDamageTime;

    Direction direction;
    State _state;

    //プロパティ
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

    //Setter
    public void SetCharacterData(int characterTypeID)  //hpやatkなどの情報もここでセットする。
    {
        autoRoute = GameManager.instance.autoRouteDatas[characterTypeID];
    }
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
        state = State.Marching;

        targetCastlePos = new Vector2Int(GameManager.instance.mapMGR.GetMapWidth() - 2, GameManager.instance.mapMGR.GetMapHeight() - 2);
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

                //TargetNearestTower();
                SearchAutoRoute();
                InitiMoveAlongWith();
            }
        }
        else
        {
            //プレイヤーが進路を選択する
            //↓はテスト用の配列
            Vector2Int[] route = { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(1, 3), new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4) };
        }

        MoveAlongWith();
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
        if (!isMoving) StartCoroutine(MoveForwardCoroutine());
    }

    IEnumerator MoveForwardCoroutine()  //Characterをゆっくり動かす関数
    {
        Debug.Log("MoveCoroutineを実行します");
        Vector2 startPos;
        Vector2 endPos;


        isMoving = true;

        MoveData(GetDirectionVector()); //先にMoveDateを行う


        startPos = transform.position;
        endPos = GetTransformPosFromGridPos();

        Debug.Log($"MoveForwardCoroutineにおいてstartPos:{startPos},endPos{endPos}");


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

    public bool CanAttackTower() //最も近いタワーの座標を取得する
    {
        int lookingForValue = 1; //索敵範囲の値
        int notLookingForValue = 0; //索敵範囲外の値
        int centerValue = 0; //原点の値

        Vector2Int vector; //ループ内で使い、(i,j)をワールド座標に直したもの
        List<Vector2Int> nearestTowerList = new List<Vector2Int>();

        int[,] searchRangeArray;
        int maxRange = attackRange;


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

        //Listの中身をソートする
        nearestTowerList.Sort((a, b) => b.y - a.y); //まずy座標に関して降順でソートする
        nearestTowerList.Sort((a, b) => b.x - a.x); //次にx座標に関して降順でソートする

        if (nearestTowerList.Count >0)
        {
            targetTowerPos = nearestTowerList[0];
            Debug.Log("nearestTowerList:" + string.Join(",", nearestTowerList) + $"\ntargetGridPos:{targetTowerPos}");
            return true;
        }
        else
        {
            targetTowerPos = Vector2Int.zero; //nullの代わり
            Debug.Log("nearestTowerList:" + string.Join(",", nearestTowerList) + $"\ntargetGridPos:{targetTowerPos}");
            return false;
        }
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
        Debug.Log("SearchAutoRouteを実行します");
        routeList = autoRoute.SearchShortestRoute(gridPos, targetCastlePos); //参照渡し
        Debug.Log("routeList:"+string.Join(",",routeList));
    }

    public void InitiMoveAlongWith()
    {
        moveAlongWithCounter = 0; //カウンターの初期化

        if (!routeList.Contains(gridPos)) //キャラクターの位置を含むかどうか確認する
        {
            Debug.LogError($"routeListがgridPos:{gridPos}を含みません");
        }

    }

    public void MoveAlongWith()
    {
        Vector2Int nextPos, nextNextPos;

        if (isMoving) return;

        if (moveAlongWithCounter == routeList.Count -1) //ルートの終点にいるときの処理
        {
            Debug.Log("ルートの終点にいるのでInBatteleに切り替えます");
            SetDirection(targetCastlePos - gridPos);
            state = State.InBattle;
            return;
        }

        if (CanAttackTower()) //ルートに沿って移動しているときに、攻撃範囲内にタワーがあるとき
        {
            Debug.Log("攻撃範囲内にタワーがあるのでInBatteleに切り替えます");
            SetDirection(targetTowerPos - gridPos);
            state = State.InBattle;
            return;
        }

        nextPos = routeList[moveAlongWithCounter + 1];

        if (moveAlongWithCounter < routeList.Count - 2)  //↓斜め移動できるときはそうする。
        {
            nextNextPos = routeList[moveAlongWithCounter + 2];
            if (((nextPos-gridPos).x == 0 && (nextNextPos-nextPos).y == 0) || ((nextPos-gridPos).y == 0 && (nextNextPos-nextPos).x == 0)) //nextPosが角マスのときtrue
            {
                if (CanMove(nextNextPos - gridPos))
                {
                    Debug.Log($"斜め移動が可能なため、nextPos:{nextPos}をnextNextPos:{nextNextPos}で置き換えます gridPos:{gridPos}");
                    nextPos = nextNextPos;
                    moveAlongWithCounter++;
                }
            }
        }

        SetDirection(nextPos - gridPos);

        if (CanMove(nextPos - gridPos))
        {
            MoveForward();
            moveAlongWithCounter++;
        }
        else
        {
            Debug.LogError($"CanMove({nextPos - gridPos})がfalseなので移動できません");
        }

    }

    public void Battle()
    {
        //Debug.LogWarning($"Battleを実行します targetTowerPos:{targetTowerPos}");

        if (isFristBattle)
        {
            isFristBattle = false;

            targetFacility = GameManager.instance.mapMGR.GetMap().GetFacility(targetTowerPos);
        }

        Attack();
    }

    public void Attack()
    {
        Debug.LogWarning($"Attackを実行します");
        //Debug.LogWarning($"GetFacility({targetTowerPos}):{GameManager.instance.mapMGR.GetMap().GetFacility(targetTowerPos)}");


        if (GameManager.instance.mapMGR.GetMap().GetFacility(targetTowerPos) == null) { //towerMGRがないということはタワーを破壊したということなので、Marchingに切り替える
            Debug.LogWarning("タワーを破壊したのでMarchingに切り替えます");
            state = State.Marching;
            return;
        }

        if (!isAttacking) StartCoroutine(AttackCoroutine());

    }

    IEnumerator AttackCoroutine()
    {
        float timer = 0;

        Debug.LogWarning($"AttackCoroutineを実行します");
         
        isAttacking = true;

        Debug.LogWarning($"Facility({targetTowerPos})に{atk}のダメージを与えた");
        targetFacility.HP -= CalcDamage(atk);

        while(timer < attackInterval){
            timer += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    public int CalcDamage(int atk)
    {
        return atk; //とりあえず、今は何もしないで攻撃力をそのまま返す
    }

}



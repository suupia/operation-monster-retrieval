using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterMGR : MonoBehaviour
{
    Animator animator;

    [SerializeField] Vector2Int gridPos;

    [SerializeField] Vector2Int targetCastlePos;
    [SerializeField] Vector2Int targetFacilityPos;
    [SerializeField] Vector2Int directionVectorToTarget;

    [SerializeField] Facility targetFacility;

    GameObject damageTextParent; //Findで取得する
    [SerializeField] GameObject damageTextPrefab; //インスペクター上でセットする
    [SerializeField] float heightToDisplayDamage; //ダメージテキストをどのくらい高く表示するかを決める


    [SerializeField] int characterTypeID;
    [SerializeField] int level;
    [SerializeField] int maxHp;
    [SerializeField] int hp;
    public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <= 0)
            {
                Die();
            }

        }
    } //プロパティ
    [SerializeField] int atk;
    [SerializeField] float attackInterval;
    [SerializeField] int attackRange;
    [SerializeField] float spd; //1秒間に進むマスの数 [マス/s]  とりあえず１にしておく
    float moveTime; // movetime = 1/spd [s]
    [SerializeField] int coolTime;

    bool isMarching = false;
    bool isAttacking = false;
    bool isMoving = false;

    bool isFristBattle = true;

    AutoRouteData autoRoute;
    ManualRouteData manualRoute;
    List<Vector2Int> routeList;
    List<int> nonDiagonalPoints;

    int moveAlongWithCounter=0;

    State _state;
    private State state
    {
        get { return _state; }
        set
        {
            switch (value)
            {
                case State.Marching:
                    break;
                case State.InBattle:
                    isFristBattle = true;
                    break;
            }
            _state = value;
        }
    } //プロパティ
    private enum State
    {
        Marching,
        InBattle
    }

    Mode _mode;
    public Mode mode
    {
        get { return _mode; }
        set
        {
            _mode = value;
        }
    } //プロパティ
    public enum Mode
    {
        Auto,
        Manual
    }

    Direction direction;
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
        manualRoute = GameManager.instance.manualRouteDatas[characterTypeID];
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

    public void SetMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Auto:
                this.mode = Mode.Auto;
                break;
            case Mode.Manual:
                this.mode = Mode.Manual;
                break;
        }

    }
    private void Start()
    {
        animator = GetComponent<Animator>();

        damageTextParent = GameObject.Find("DamageTextParent");

        HP = maxHp;
        moveTime = 1 / spd;
        gridPos = GameManager.instance.ToGridPosition(transform.position);
        state = State.Marching;

        targetCastlePos = new Vector2Int(GameManager.instance.mapMGR.GetMapWidth() - 2, GameManager.instance.mapMGR.GetMapHeight() - 2);

        //routeに関する処理
        //mode = Mode.Manual;
        switch (mode)
        {
            case Mode.Auto:
                SetAutoRoute();
                break;
            case Mode.Manual:
                SetManualRoute();
                break;
        }
        InitiMoveAlongWith();
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
        endPos = startPos + GetDirectionVector();

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

        //数値データの移動
        GameManager.instance.mapMGR.DivisionalSetMapValue(gridPos, GameManager.instance.characterID);
        GameManager.instance.mapMGR.MultiplySetMapValue(gridPos + directionVector, GameManager.instance.characterID);

        //スクリプトの移動
        GameManager.instance.mapMGR.GetMap().RemoveCharacterMGR(gridPos, this);
        GameManager.instance.mapMGR.GetMap().AddCharacterMGR(gridPos+directionVector, this.gameObject.GetComponent<CharacterMGR>());



        //gridPosを移動させる。これは最後に行うことに注意！
        gridPos += directionVector;

    }
    public void SetAutoRoute()
    {
        Debug.Log("SetAutoRouteを実行します");
        routeList = autoRoute.SearchShortestRoute(gridPos, targetCastlePos); //参照渡し
        nonDiagonalPoints = new List<int>(); //Autoのときは使わないと思うが、Manualと揃えるためにnewしておく
        Debug.Log("routeList:"+string.Join(",",routeList));
    }

    public void SetManualRoute()
    {
        Debug.Log("SetManualRouteを実行します");
        routeList = manualRoute.GetManualRoute();
        nonDiagonalPoints = manualRoute.GetNonDiagonalPoints();
        Debug.Log("routeList:" + string.Join(",", routeList));
    }

    public void InitiMoveAlongWith()
    {
        //moveAlongWithCounter = 0; //カウンターの初期化

        if (!routeList.Contains(gridPos)) //キャラクターの位置を含むかどうか確認する
        {
            Debug.LogError($"routeListがgridPos:{gridPos}を含みません");
        }

    }

    public void MoveAlongWith()
    {
        Vector2Int nextPos, nextNextPos;

        if (isMoving) return;

        //if (moveAlongWithCounter == routeList.Count -1) //ルートの終点にいるときの処理
        //{
        //    Debug.Log("ルートの終点にいるのでInBatteleに切り替えます");
        //    SetDirection(targetCastlePos - gridPos);
        //    state = State.InBattle;
        //    return;
        //}

        if (GameManager.instance. CanAttackTarget(gridPos,attackRange,GameManager.instance.facilityID,out targetFacilityPos)) //ルートに沿って移動しているときに、攻撃範囲内にタワーがあるとき
        {
            Debug.LogWarning($"攻撃範囲内にタワーがあるのでInBatteleに切り替えます targetFacilityPos:{targetFacilityPos}");
            SetDirection(targetFacilityPos - gridPos);
            state = State.InBattle;
            return;
        }

        nextPos = routeList[moveAlongWithCounter + 1];

        if (moveAlongWithCounter < routeList.Count - 2 && !nonDiagonalPoints.Contains(moveAlongWithCounter))  //↓斜め移動できるときはそうする。
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

            targetFacility = GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos);
        }

        Attack();
    }

    public void Attack()
    {
        Debug.Log($"Attackを実行します");

        if (GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos) == null) { //towerMGRがないということはタワーを破壊したということなので、Marchingに切り替える
            Debug.LogWarning($"タワーを破壊したのでMarchingに切り替えます targetFacilityPos:{targetFacilityPos}");
            state = State.Marching;
            return;
        }

        if (!isAttacking) StartCoroutine(AttackCoroutine());

    }

    IEnumerator AttackCoroutine()
    {
        float timer = 0;
        int damage;

        Debug.Log($"AttackCoroutineを実行します");
         
        isAttacking = true;

        damage = CalcDamage(atk);

        Debug.Log($"Facility({targetFacilityPos})に{damage}のダメージを与えた");
        targetFacility.HP -= damage;
        DrawDamage(damage);

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


    public void DrawDamage(int damage)
    {
        GameObject damageTextGO;
        Text damageText;
        Vector3 drawPos = this.transform.position + new Vector3(0, heightToDisplayDamage, 0);
        damageTextGO = Instantiate(damageTextPrefab, RectTransformUtility.WorldToScreenPoint(Camera.main, drawPos), Quaternion.identity, damageTextParent.transform);
        damageText = damageTextGO.GetComponent<Text>();
        damageText.text = damage.ToString();
    }

    public void Die()
    {
        Debug.Log($"HPが0以下になったので、キャラクターを消去します gridPos:{gridPos}のキャラクター");

        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.characterID); //数値データをを消去する
        GameManager.instance.mapMGR.GetMap().RemoveCharacterMGR(gridPos,this);
        //GameManager.instance.mapMGR.GetMap().SetCharacterMGR(gridPos,null); //スクリプトをを消去する

        
        Destroy(this.gameObject);
    }
}



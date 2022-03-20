using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class Unit : MonoBehaviour
{

    protected Animator animator;

    [SerializeField] protected Vector2Int gridPos;

    [SerializeField] protected Vector2Int targetCastlePos;
    protected int targetCastleID; //キャラクターとロボットで標的の城のIDが違う
    [SerializeField] protected Vector2Int targetFacilityPos;
    [SerializeField] protected Vector2Int directionVectorToTarget;
    [SerializeField] protected Facility targetFacility;

    protected GameObject damageTextParent; //Findで取得する
    [SerializeField] protected GameObject damageTextPrefab; //インスペクター上でセットする
    protected float heightToDisplayDamage = 0.6f; //ダメージテキストをどのくらい高く表示するかを決める

    [SerializeField] protected int maxHp;
    [SerializeField] int hp;
    public int HP
    {
        get { return hp; }
        set
        {
            int beforeHp = hp;
            hp = value;
            if (hp <= 0 && isAlive)
            {
                isAlive = false; //すぐにfalseにして、Die()が2回以上呼ばれないようにする
                Die();
                return;
            }
            if (value - beforeHp < 0)
            {
                Debug.Log("SetDamagedAnimationを呼びます");
                StartCoroutine(SetDamageAnimation());
            }

        }
    } //プロパティ
    [SerializeField] protected int atk;
    [SerializeField] protected float atkInterval;
    [SerializeField] protected int atkRange;
    [SerializeField] protected float spd; //1秒間に進むマスの数 [マス/s]  とりあえず１にしておく
    protected float moveTime; // movetime = 1/spd [s]

    protected Direction direction;
    protected enum Direction
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
    protected State _state;
    protected State state
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
    protected enum State
    {
        Marching,
        InBattle
    }
    protected bool isAttacking = false;
    protected bool isMoving = false;
    protected bool isFristBattle = true;
    bool isAlive = true;  //HPが0になったときにDie()が2回以上呼ばれるのを防ぐために必要


    protected AutoRouteData autoRoute;

    protected List<Vector2Int> routeList;
    protected int moveAlongWithCounter = 0;

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
    public Vector2 GetTransformPos() //リアルタイムで更新されるtransformPosを返す
    {
        return transform.position;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }
    public int GetAtk()
    {
        return atk;
    }
    public void SetAtk(int atkNum)
    {
        atk = atkNum;
    }
    public float GetAtkInterval()
    {
        return atkInterval;
    }
    public int GetAtkRange()
    {
        return atkRange;
    }
    public float GetSpd()
    {
        return spd;
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
        else if (directionVector.x <= 0)
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

    //MonoBehaviour専用
    protected void Start()
    {
        animator = GetComponent<Animator>();

        damageTextParent = GameObject.Find("DamageTextParent");

        HP = maxHp;
        moveTime = 1 / spd;
        gridPos = GameManager.instance.ToGridPosition(transform.position);
        state = State.Marching;

        if (this is CharacterMGR)
        {
            targetCastlePos = GameManager.instance.mapMGR.GetEnemysCastlePos();
            targetCastleID = GameManager.instance.enemyCastleID;
        }
        else
        {
            targetCastlePos = GameManager.instance.mapMGR.GetAllysCastlePos();
            targetCastleID = GameManager.instance.allyCastleID;

        }

        //敵のタイプに分けてrouteListの初期化を変える(とりあえず、SetAutoRouteだけにしておく)
        SetAutoRoute();


        InitiMoveAlongWith();
    }

    protected void Update()
    {
        if (GameManager.instance.state == GameManager.State.SelectingStage)
        {
            Destroy(this.gameObject);
        }

        if (GameManager.instance.state == GameManager.State.PauseTheGame) //ポーズ中の時はUpdateの処理をしない
        {
            return;
        }


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


    protected void SetAutoRoute()
    {
        Debug.Log("SetAutoRouteを実行します");
        routeList = autoRoute.SearchShortestRoute(gridPos, targetCastlePos); //参照渡し
        Debug.Log("routeList:" + string.Join(",", routeList));
    }

    private void InitiMoveAlongWith()
    {
        //moveAlongWithCounter = 0; //カウンターの初期化

        if (!routeList.Contains(gridPos)) //キャラクターの位置を含むかどうか確認する
        {
            Debug.LogError($"routeListがgridPos:{gridPos}を含みません");
        }

    }

    private void March() //Update()で呼ばれることに注意
    {
        MoveAlongWith();
    }



    private bool CanMove(Vector2Int vector)
    {

        if (GameManager.instance.mapMGR.GetMapValue(gridPos + vector) % GameManager.instance.wallID == 0)
        {
            Debug.LogError($"移動先にwallIDがあるため、移動できません(gridPos:{gridPos}vector:{vector})\nGameManager.instance.mapMGR.GetMapValue(gridPos + vector)={GameManager.instance.mapMGR.GetMapValue(gridPos + vector)} GetDirectionVector={GetDirectionVector()}");
            return false;
        }

        //斜め移動の時にブロックの角を移動することはできない
        if (vector.x != 0 && vector.y != 0)
        {
            //水平方向の判定
            if (GameManager.instance.mapMGR.GetMapValue(gridPos.x + vector.x, gridPos.y) % GameManager.instance.wallID == 0)
            {
                Debug.LogError($"水平方向に壁があるため、移動できません。");

                return false;
            }

            //垂直方向の判定
            if (GameManager.instance.mapMGR.GetMapValue(gridPos.x, gridPos.y + vector.y) % GameManager.instance.wallID == 0)
            {
                Debug.LogError($"鉛直方向に壁があるため、移動できません。");
                return false;
            }
        }

        return true;
    }
    private void MoveForward()
    {
        Debug.Log("MoveForwardを実行します");
        if (!isMoving) StartCoroutine(MoveForwardCoroutine());
    }

    private IEnumerator MoveForwardCoroutine()  //Characterをゆっくり動かす関数
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
            transform.position = Vector3.MoveTowards(transform.position, endPos, 1f / moveTime * Time.deltaTime * GameManager.instance.gameSpeed);
            //3つ目の引数は"1フレームの最大移動距離"　単位は実質[m/s](コルーチンが1フレームずつ回っているからTime.deltaTimeが消える。moveTime経った時に1マス進む。)

            remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;


            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める


            yield return null;  //1フレーム停止させる。
        }
        transform.position = endPos;//ループを抜けた時はきっちり移動させる。



        isMoving = false;
        //Debug.Log($"MoveCoroutine()終了時のendPosは{endPos}");
    }
    private void MoveData(Vector2Int directionVector)
    {

        if (this is CharacterMGR)
        {
            //CharacterMGRのとき
            if (!(GameManager.instance.mapMGR.GetMapValue(gridPos) % GameManager.instance.characterID == 0))
            {
                Debug.LogError("MoveDateで移動前のmapValueにcharacterIDが含まれていません");
                return;
            }

            //数値データの移動
            GameManager.instance.mapMGR.DivisionalSetMapValue(gridPos, GameManager.instance.characterID);
            GameManager.instance.mapMGR.MultiplySetMapValue(gridPos + directionVector, GameManager.instance.characterID);

            //スクリプトの移動
            GameManager.instance.mapMGR.GetMap().RemoveUnit(gridPos, this);
            GameManager.instance.mapMGR.GetMap().AddUnit(gridPos + directionVector, this.gameObject.GetComponent<CharacterMGR>());
        }
        else
        {
            //RobotMGRのとき
            if (!(GameManager.instance.mapMGR.GetMapValue(gridPos) % GameManager.instance.robotID == 0))
            {
                Debug.LogError("MoveDateで移動前のmapValueにrobotIDが含まれていません");
                return;
            }

            //数値データの移動
            GameManager.instance.mapMGR.DivisionalSetMapValue(gridPos, GameManager.instance.robotID);
            GameManager.instance.mapMGR.MultiplySetMapValue(gridPos + directionVector, GameManager.instance.robotID);

            //スクリプトの移動
            GameManager.instance.mapMGR.GetMap().RemoveUnit(gridPos, this);
            GameManager.instance.mapMGR.GetMap().AddUnit(gridPos + directionVector, this.gameObject.GetComponent<RobotMGR>());
        }

        //gridPosを移動させる。これは最後に行うことに注意！
        gridPos += directionVector;

    }
    private void MoveAlongWith()
    {
        Vector2Int nextPos;
        //Vector2Int nextNextPos;  RouteListは既に斜めに進めるところは斜めにした状態になっているので、この変数と下の方にあるのif文の処理はいらない。コメントアウトしておく

        if (isMoving) return;

        CheckIfCauseSkill();

        if (Function.isWithinTheAttackRange(gridPos, atkRange, GameManager.instance.towerID, out targetFacilityPos) || Function.isWithinTheAttackRange(gridPos, atkRange, GameManager.instance.enemyCastleID, out targetFacilityPos)) //ルートに沿って移動しているときに、攻撃範囲内にタワー（城を除く）があるとき
        {
            Debug.Log($"攻撃範囲内にタワーがあるのでInBattleに切り替えます targetFacilityPos:{targetFacilityPos}");
            SetDirection(targetFacilityPos - gridPos);
            state = State.InBattle;
            return;
        }

        if (moveAlongWithCounter == routeList.Count - 1)  //ルートの最終地点に到達したら城への攻撃を開始する
        {
            Debug.Log($"ルートの最終地点にいるためInBattleに切り替えます");
            SetDirection(GameManager.instance.mapMGR.GetEnemysCastlePos() - gridPos);
            state = State.InBattle;
            return;
        }


        nextPos = routeList[moveAlongWithCounter + 1];

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
    protected abstract void CheckIfCauseSkill(); //CharacterMGRでのみ処理が書かれる


    private void Battle()
    {
        //Debug.LogWarning($"Battleを実行します targetTowerPos:{targetTowerPos}");

        if (isFristBattle)
        {
            isFristBattle = false;

            targetFacility = GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos);
        }

        Attack();
    }

    private void Attack()
    {
        //Debug.Log($"Attackを実行します");

        if (GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos) == null)
        { //towerMGRがないということはタワーを破壊したということなので、Marchingに切り替える
            Debug.Log($"タワーを破壊したのでMarchingに切り替えます targetFacilityPos:{targetFacilityPos}");
            state = State.Marching;
            return;
        }

        if (!isAttacking) StartCoroutine(AttackCoroutine());

    }

    private IEnumerator AttackCoroutine()
    {
        float timer = 0;
        int damage;

        Debug.Log($"AttackCoroutineを実行します");

        isAttacking = true;

        damage = CalcDamage(atk);

        Debug.Log($"Facility({targetFacilityPos})に{damage}のダメージを与えた");
        targetFacility.HP -= damage;
        DrawDamage(damage);

        while (timer < atkInterval)
        {
            timer += Time.deltaTime * GameManager.instance.gameSpeed;

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める

            yield return null;
        }

        isAttacking = false;
    }

    private int CalcDamage(int atk)
    {

        int result = 0;

        if (GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos) is CastleMGR) //今ターゲットとしている施設が城であった場合、タワーの数を考慮した計算式に変える
        {
            result = (int)Mathf.Ceil(atk * Mathf.Pow((float)(GameManager.instance.MaxTowerNum - GameManager.instance.CurrentTowerNum) / GameManager.instance.MaxTowerNum, 2)); //切り上げ 2乗して滑らかにする

            if (result == 0) result = 1; //タワーを一つも破壊していない場合、計算結果が0になるが、1ダメージは入るようにする

            //Debug.Log($"CalcDamageのresult:{result}");
        }
        else
        {
            //とりあえず、タワーへの攻撃は何もしないでそのまま値を返す
            result = atk;
        }

        return result;
    }

    private void DrawDamage(int damage)
    {
        GameObject damageTextGO;
        Text damageText;
        Vector3 drawPos = this.transform.position + new Vector3(0, heightToDisplayDamage, 0);
        damageTextGO = Instantiate(damageTextPrefab, RectTransformUtility.WorldToScreenPoint(Camera.main, drawPos), Quaternion.identity, damageTextParent.transform);
        damageText = damageTextGO.GetComponent<Text>();
        damageText.text = damage.ToString();
    }

    IEnumerator SetDamageAnimation()
    {
        yield return new WaitForSeconds(Facility.GetTimeToImpact() * (1.0f / GameManager.instance.gameSpeed));

        switch (direction)
        {
            case Direction.Back:
                Debug.Log($"{direction}のdamageアニメーションを設定します");
                animator.SetTrigger("BackDamage");
                break;

            case Direction.DiagLeftBack:
            case Direction.DiagRightBack:
                Debug.Log($"{direction}のdamageアニメーションを設定します");

                animator.SetTrigger("DiagonalBackDamage");
                break;

            case Direction.Left:
            case Direction.Right:
                Debug.Log($"{direction}のdamageアニメーションを設定します");

                animator.SetTrigger("SideDamage");
                break;

            case Direction.DiagLeftFront:
            case Direction.DiagRightFront:
                Debug.Log($"{direction}のdamageアニメーションを設定します");

                animator.SetTrigger("DiagonalFrontDamage");
                break;

            case Direction.Front:
                Debug.Log($"{direction}のdamageアニメーションを設定します");

                animator.SetTrigger("FrontDamage");
                break;

        }

    }


    public abstract void Die();
    
}

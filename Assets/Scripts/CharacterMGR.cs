using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterSkillMGR))]
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
    float heightToDisplayDamage = 0.6f; //ダメージテキストをどのくらい高く表示するかを決める

    [SerializeField] CharacterSkillMGR characterSkillMGR;
    [SerializeField] ParticleSystem skillIconParticle;
    Material skillAuraMaterial; 

    [SerializeField] string characterName;
    [SerializeField] int characterTypeID;
    [SerializeField] int level; //0からスタートすることに注意
    int maxLevel=9; //最大レベルは10
    [SerializeField] int maxHp;
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
    [SerializeField] int atk;
    [SerializeField] float atkInterval;
    [SerializeField] int atkRange;
    [SerializeField] float spd; //1秒間に進むマスの数 [マス/s]  とりあえず１にしておく
    float moveTime; // movetime = 1/spd [s]
    [SerializeField] float coolTime;
    [SerializeField] int cost;
    [SerializeField] Sprite sprite; //立ち絵
    [SerializeField] Sprite thumbnailSprite; //ボタンに表示するスプライト
    [SerializeField] string introduction; //モンスターの紹介文

    [SerializeField] int[] hpGrowthRate; //成長率をインスペクター上で決めておく ex) 0,1,0 なら、最初のレベルアップでは変化せず、次のレベルアップで1上がる
    [SerializeField] int[] atkGrowthRate;
    [SerializeField] int[] spdGrowthRate;
    [SerializeField] int[] atkIntervalGrowthRate;
    [SerializeField] int[] atkRangeGrowthRate;
    [SerializeField] int[] coolTimeGrowthRate;


    //bool isMarching = false;  今は使っていないからコメントアウト
    bool isAlive = true;  //HPが0になったときにDie()が2回以上呼ばれるのを防ぐために必要
    bool isAttacking = false;
    bool isMoving = false;

    bool isFristBattle = true;

    AutoRouteData autoRoute;
    ManualRouteData manualRoute;
    List<Vector2Int> routeList;
    List<int> nonDiagonalPoints;

    int moveAlongWithCounter = 0;
    List<int> causeSkillPoints;  //moceAlongWithCounterがcauseSkillPointと等しい時にskillを発動する

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

    public CharacterMGR Clone() //ディープコピー用
    {
        return (CharacterMGR)MemberwiseClone();
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
    public Vector2 GetTransformPos() //リアルタイムで更新されるtransformPosを返す
    {
        return transform.position;
    }
    public string GetCharacterName()
    {
        return characterName;
    }
    public int GetCharacterTypeID()
    {
        return characterTypeID;
    }
    public int GetLevel()
    {
        return level;
    }
    public int GetMaxLevel()
    {
        return maxLevel;
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
    public void SetSpd(float spdNum)
    {
        spd = spdNum;
        moveTime = 1 / spd;
    }

    public float GetCoolTime()
    {
        return coolTime;
    }
    public int GetCost()
    {
        return cost;
    }
    public Sprite GetSprite()
    {
        return sprite;
    }
    public Sprite GetThumbnailSprite()
    {
        return thumbnailSprite;
    }
    public string GetIntroduction()
    {
        return introduction;
    }

    public int GetSkillNum()
    {
        return characterSkillMGR.GetSkillNum();
    }
    public CharacterSkillMGR GetCharacterSkillMGR()
    {
        return characterSkillMGR;
    }

    //Setter
    public void SetCharacterData(int buttonNum, int characterTypeID)  //hpやatkなどの情報もここでセットする。
    {
        autoRoute = GameManager.instance.autoRouteDatas[buttonNum];
        manualRoute = GameManager.instance.manualRouteDatas[buttonNum];

        level = GameManager.instance.GetCharacterDatabase(characterTypeID).GetLevel();
        maxHp = GameManager.instance.GetCharacterDatabase(characterTypeID).GetMaxHp();
        atk = GameManager.instance.GetCharacterDatabase(characterTypeID).GetAtk();
        atkInterval = GameManager.instance.GetCharacterDatabase(characterTypeID).GetAtkInterval();
        atkRange = GameManager.instance.GetCharacterDatabase(characterTypeID).GetAtkRange();
        spd = GameManager.instance.GetCharacterDatabase(characterTypeID).GetSpd();
        coolTime = GameManager.instance.GetCharacterDatabase(characterTypeID).GetCoolTime();

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
    IEnumerator SetDamageAnimation()
    {
        yield return new WaitForSeconds(Facility.GetTimeToImpact() * (1.0f/GameManager.instance.gameSpeed));

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

    public void SetInitiLevel(int initiLevel)
    {
        this.level = initiLevel;
    }
    public void LevelUp()
    {
        if (level == maxLevel)
        {
            Debug.LogError("最大レベルなのでレベルアップできません"); //LevelUp()はレベルアップできることが確定しているときにのみ呼ばれるので、このログはエラー扱いにする
            return;
        }

        Debug.Log($"{characterName}のレベルを上げます");

        maxHp += hpGrowthRate[level];
        atk += atkGrowthRate[level];
        spd += spdGrowthRate[level];
        atkInterval += atkIntervalGrowthRate[level];
        atkRange += atkRangeGrowthRate[level];
        coolTime += coolTimeGrowthRate[level];

        level++;   

    }
    private void Start()
    {
        animator = GetComponent<Animator>();

        damageTextParent = GameObject.Find("DamageTextParent");

        GetComponent<Renderer>().material = GameManager.instance.characterSkillsDataMGR.skillAuraMaterials[characterSkillMGR.GetSkillNum()];
        skillAuraMaterial = GetComponent<Renderer>().material;  //「レンダラーに割り当てられている最初にインスタンス化されたMaterialを返します」by リファレンス


        HP = maxHp;
        moveTime = 1 / spd;
        gridPos = GameManager.instance.ToGridPosition(transform.position);
        state = State.Marching;

        targetCastlePos = GameManager.instance.mapMGR.GetEnemysCastlePos();


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
        causeSkillPoints = characterSkillMGR.JudgeSkillTrigger(routeList); //skillが発動する点を判定し、そのIndexを受け取る
        InitiMoveAlongWith();
    }

    private void Update()
    {
        if (GameManager.instance.state == GameManager.State.SelectingStage)
        {
            Destroy(this.gameObject);
        }

        if (GameManager.instance.state == GameManager.State.PauseTheGame) //ポーズ中の時はUpdateの処理をしない
        {
            return;
        }

        //if (GameManager.instance.state != GameManager.State.RunningGame) return; //RunningGame意外だったらUpdateの処理をしない

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
        GameManager.instance.mapMGR.GetMap().AddCharacterMGR(gridPos + directionVector, this.gameObject.GetComponent<CharacterMGR>());



        //gridPosを移動させる。これは最後に行うことに注意！
        gridPos += directionVector;

    }
    public void SetAutoRoute()
    {
        Debug.Log("SetAutoRouteを実行します");
        routeList = autoRoute.SearchShortestRoute(gridPos, targetCastlePos); //参照渡し
        nonDiagonalPoints = new List<int>(); //Autoのときは使わないと思うが、Manualと揃えるためにnewしておく
        Debug.Log("routeList:" + string.Join(",", routeList));
    }

    public void SetManualRoute()
    {
        Debug.Log("SetManualRouteを実行します");
        if (manualRoute.GetManualRoute().Count == 0)
        {
            Debug.Log("manualRouteの長さが0のためAutoModeに切り替えます");
            SetMode(Mode.Auto);
            SetAutoRoute();

        }
        else
        {
            routeList = manualRoute.GetManualRoute();
        }
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
        Vector2Int nextPos;
        //Vector2Int nextNextPos;  RouteListは既に斜めに進めるところは斜めにした状態になっているので、この変数と下の方にあるのif文の処理はいらない。コメントアウトしておく

        if (isMoving) return;

        if(causeSkillPoints.Contains(moveAlongWithCounter))
        {
            characterSkillMGR.CauseSkill(this, skillIconParticle,skillAuraMaterial);
        }
        //if (moveAlongWithCounter == routeList.Count -1) //ルートの終点にいるときの処理
        //{
        //    Debug.Log("ルートの終点にいるのでInBatteleに切り替えます");
        //    SetDirection(targetCastlePos - gridPos);
        //    state = State.InBattle;
        //    return;
        //}

        if (GameManager.instance.CanAttackTarget(gridPos, atkRange, GameManager.instance.facilityID, out targetFacilityPos)) //ルートに沿って移動しているときに、攻撃範囲内にタワーがあるとき
        {
            Debug.Log($"攻撃範囲内にタワーがあるのでInBatteleに切り替えます targetFacilityPos:{targetFacilityPos}");
            SetDirection(targetFacilityPos - gridPos);
            state = State.InBattle;
            return;
        }

        nextPos = routeList[moveAlongWithCounter + 1];

        //if (moveAlongWithCounter < routeList.Count - 2 && !nonDiagonalPoints.Contains(moveAlongWithCounter))  //↓斜め移動できるときはそうする。
        //{
        //    nextNextPos = routeList[moveAlongWithCounter + 2];
        //    if (((nextPos - gridPos).x == 0 && (nextNextPos - nextPos).y == 0) || ((nextPos - gridPos).y == 0 && (nextNextPos - nextPos).x == 0)) //nextPosが角マスのときtrue
        //    {
        //        if (CanMove(nextNextPos - gridPos))
        //        {
        //            Debug.Log($"斜め移動が可能なため、nextPos:{nextPos}をnextNextPos:{nextNextPos}で置き換えます gridPos:{gridPos}");
        //            nextPos = nextNextPos;
        //            moveAlongWithCounter++;
        //        }
        //    }
        //}

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
        //Debug.Log($"Attackを実行します");

        if (GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos) == null)
        { //towerMGRがないということはタワーを破壊したということなので、Marchingに切り替える
            Debug.Log($"タワーを破壊したのでMarchingに切り替えます targetFacilityPos:{targetFacilityPos}");
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

        while (timer < atkInterval)
        {
            timer += Time.deltaTime * GameManager.instance.gameSpeed;

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める

            yield return null;
        }

        isAttacking = false;
    }

    public int CalcDamage(int atk)
    {

        int result =0;

        if(GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos) is CastleMGR) //今ターゲットとしている施設が城であった場合、タワーの数を考慮した計算式に変える
        {
            result = (int)Mathf.Ceil(atk * Mathf.Pow((float)(GameManager.instance.MaxTowerNum-GameManager.instance.CurrentTowerNum)/GameManager.instance.MaxTowerNum,2)); //切り上げ 2乗して滑らかにする

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
        Debug.Log($"HPが0以下になったので、キャラクターを消去します gridPos:{gridPos},transform.pos{transform.position}のキャラクター");

        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.characterID); //数値データをを消去する
        GameManager.instance.CurrentCharacterNum--;
        GameManager.instance.mapMGR.GetMap().RemoveCharacterMGR(gridPos, this);
        //GameManager.instance.mapMGR.GetMap().SetCharacterMGR(gridPos,null); //スクリプトをを消去する


        Destroy(this.gameObject);
    }
}


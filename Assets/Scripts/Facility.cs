using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Facility : MonoBehaviour
{
    Animator animator;

    [SerializeField] protected Vector2Int gridPos;

    [SerializeField] protected Vector2Int targetCharacterPos;

    [SerializeField] Unit targetCharacter;

    GameObject damageTextParent; //Findで取得する
    [SerializeField] GameObject damageTextPrefab; //インスペクター上でセットする
    [SerializeField] float heightToDisplayDamage; //ダメージテキストをどのくらい高く表示するかを決める

    [SerializeField] GameObject cannonballPrefab; //弾丸のプレハブ　インスペクタ上でセットする
    static float timeToImpact = 0.2f;     //着弾までの時間
    public static float GetTimeToImpact() //被弾アニメーションにディレイを掛けるために必要
    {
        return timeToImpact;
    }

    [SerializeField] protected int level;
    [SerializeField] protected int maxHp;
    [SerializeField] protected int hp;
    [SerializeField] protected int atk;
    [SerializeField] protected float attackInterval;
    [SerializeField] protected int attackRange;

    protected bool isAttacking = false;

    protected bool isFristBattle = true;

    protected bool isAlive = true; //HPが0になったときにDie()が2回以上呼ばれるのを防ぐために必要

    State _state; //プロパティを定義してある

    protected enum State
    {
        Idle,
        InBattle
    }

    //プロパティ
    private State state
    {
        get { return _state; }
        set
        {
            switch (value)
            {
                case State.Idle:
                    break;
                case State.InBattle:
                    isFristBattle = true;
                    break;
            }
            _state = value;
        }
    }
     public virtual int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <=0 && isAlive)
            {
                isAlive = false;  //すぐにfalseにして、Die()が2回以上呼ばれないようにする
                Die();
            }

        }
    }
    public abstract void SetDirection(Vector2 directionVector); //TowerMGRでのみ処理が必要


    protected void Start()
    {
        animator = GetComponent<Animator>();

        damageTextParent = GameObject.Find("DamageTextParent");

        HP = maxHp;

        gridPos = GameManager.instance.ToGridPosition(transform.position);

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
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.InBattle:
                Battle();
                break;
        }
    }

    protected void Idle() //Update()で呼ばれることに注意
    {
        //Debug.Log("FacilityのIdleを実行します");

        if (Function.isWithinTheAttackRange(gridPos,attackRange,GameManager.instance.characterID,out targetCharacterPos))
        {
            Debug.Log("攻撃範囲内にキャラクターがいるのでInBatteleに切り替えます");
            targetCharacter = GameManager.instance.mapMGR.GetMap().GetUnitList(targetCharacterPos)[0]; //とりあえず単体攻撃
            Debug.Log($"targetCharacter:{targetCharacter}");
            state = State.InBattle;
            return;
        }
    }
    protected void Battle()
    {
        //Debug.Log($"FacilityのBattleを実行します");

        if (isFristBattle)
        {
            isFristBattle = false;

        }

        if (!Function.isWithinTheAttackRange(gridPos, attackRange, GameManager.instance.characterID, out targetCharacterPos))
        {
            Debug.Log($"キャラクターが攻撃範囲外に出たのでIdleに切り替えます");
            state = State.Idle;
            return;
        }


        if (targetCharacter == null)
        { 
            Debug.Log($"キャラクターを倒したのでIdleに切り替えます targetCharacterPos:{targetCharacterPos}");
            state = State.Idle;
            return;
        }

        FacilityAttack();
    }
    protected void FacilityAttack()
    {
        //Debug.Log($"FacilityAttackを実行します");

        if (!isAttacking) StartCoroutine(FacilityAttackCoroutine());
    }

    IEnumerator FacilityAttackCoroutine()
    {
        float timer = 0;
        int damage;

        Debug.Log($"FacilityAttackCoroutineを実行します");
         
        isAttacking = true;

        damage = GameManager.instance.CalcDamage(atk);

        //キャラクターの方を向く
        SetDirection(targetCharacter.GetGridPos() - gridPos);

        //ダメージの表示や砲丸の表示を行う（targetCharacterの情報を使うため、先に処理する）
        DrawCannonBall();

        //yield return new WaitForSeconds(timeToImpact);
        //DrawDamage(damage);
        ////キャラクターにダメージを与える（データの処理が後）
        //targetCharacter.HP -= damage;
        //Debug.Log($"Character({targetCharacterPos})に{damage}のダメージを与えた");

        targetCharacter.HP -= damage;

        Debug.LogWarning("BeHitCoroutineを開始します");
        StartCoroutine(BeHitCoroutine(damage));


        while (timer < attackInterval){
            timer += Time.deltaTime * GameManager.instance.gameSpeed;

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める

            yield return null;
        }

        isAttacking = false;
    }
    IEnumerator BeHitCoroutine(int damage)
    {
        yield return new WaitForSeconds(timeToImpact* (1.0f/ GameManager.instance.gameSpeed));

        Debug.LogWarning("DrawDamageとtargetCharacter.HP -= damageを実行します");
        DrawDamage(damage);
        Debug.LogWarning($"Character({targetCharacterPos})に{damage}のダメージを与えた");
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

    public void  DrawCannonBall()
    {
        Vector3 firingPos = this.transform.position + new Vector3(0, heightToDisplayDamage, 0); //砲弾の初期位置

        //砲弾を生成
        //Debug.Log("砲丸を生成します");
        GameObject cannonballGO = Instantiate(cannonballPrefab, firingPos, Quaternion.identity);
        cannonballGO.GetComponent<CannonballMGR>().FiringCannonball(gridPos,timeToImpact,targetCharacter);

    }
    public abstract void Die(); //抽象メソッド

}

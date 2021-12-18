using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Facility : MonoBehaviour
{
    Animator animator;

    [SerializeField] protected Vector2Int gridPos;

    [SerializeField] protected Vector2Int targetCharacterPos;

    [SerializeField] CharacterMGR targetCharacter;

    GameObject damageTextParent; //Findで取得する
    [SerializeField] GameObject damageTextPrefab; //インスペクター上でセットする
    [SerializeField] float heightToDisplayDamage; //ダメージテキストをどのくらい高く表示するかを決める

    [SerializeField] protected int level;
    [SerializeField] protected int maxHp;
    protected int hp;
    [SerializeField] protected int atk;
    [SerializeField] protected float attackInterval;
    [SerializeField] protected int attackRange;

    protected bool isAttacking = false;

    protected bool isFristBattle = true;

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
     public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <=0)
            {
                Die();
            }

        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        damageTextParent = GameObject.Find("DamageTextParent");

        HP = maxHp;

        gridPos = GameManager.instance.ToGridPosition(transform.position);

    }

    private void Update()
    {
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
        Debug.Log("FacilityのIdleを実行します");

        if (GameManager.instance.CanAttackTarget(gridPos,attackRange,GameManager.instance.characterID,out targetCharacterPos))
        {
            Debug.Log("攻撃範囲内にキャラクターがいるのでInBatteleに切り替えます");
            targetCharacter = GameManager.instance.mapMGR.GetMap().GetCharacterMGRList(targetCharacterPos)[0]; //とりあえず単体攻撃
            Debug.Log($"targetCharacter:{targetCharacter}");
            state = State.InBattle;
            return;
        }
    }
    protected void Battle()
    {
        Debug.Log($"FacilityのBattleを実行します");

        if (isFristBattle)
        {
            isFristBattle = false;

        }

        if (!GameManager.instance.CanAttackTarget(gridPos, attackRange, GameManager.instance.characterID, out targetCharacterPos))
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
        Debug.Log($"FacilityAttackを実行します");

        if (!isAttacking) StartCoroutine(FacilityAttackCoroutine());
    }

    IEnumerator FacilityAttackCoroutine()
    {
        float timer = 0;
        int damage;

        Debug.Log($"FacilityAttackCoroutineを実行します");
         
        isAttacking = true;

        damage = GameManager.instance.CalcDamage(atk);

        Debug.Log($"Character({targetCharacterPos})に{damage}のダメージを与えた");
        targetCharacter.HP -= damage;
        DrawDamage(damage);

        while (timer < attackInterval){
            timer += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
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

    public abstract void Die(); //抽象メソッド

}

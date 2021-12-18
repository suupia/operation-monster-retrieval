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

    GameObject damageTextParent; //Find�Ŏ擾����
    [SerializeField] GameObject damageTextPrefab; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] float heightToDisplayDamage; //�_���[�W�e�L�X�g���ǂ̂��炢�����\�����邩�����߂�

    [SerializeField] protected int level;
    [SerializeField] protected int maxHp;
    protected int hp;
    [SerializeField] protected int atk;
    [SerializeField] protected float attackInterval;
    [SerializeField] protected int attackRange;

    protected bool isAttacking = false;

    protected bool isFristBattle = true;

    State _state; //�v���p�e�B���`���Ă���

    protected enum State
    {
        Idle,
        InBattle
    }

    //�v���p�e�B
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

    protected void Idle() //Update()�ŌĂ΂�邱�Ƃɒ���
    {
        Debug.Log("Facility��Idle�����s���܂�");

        if (GameManager.instance.CanAttackTarget(gridPos,attackRange,GameManager.instance.characterID,out targetCharacterPos))
        {
            Debug.Log("�U���͈͓��ɃL�����N�^�[������̂�InBattele�ɐ؂�ւ��܂�");
            targetCharacter = GameManager.instance.mapMGR.GetMap().GetCharacterMGRList(targetCharacterPos)[0]; //�Ƃ肠�����P�̍U��
            Debug.Log($"targetCharacter:{targetCharacter}");
            state = State.InBattle;
            return;
        }
    }
    protected void Battle()
    {
        Debug.Log($"Facility��Battle�����s���܂�");

        if (isFristBattle)
        {
            isFristBattle = false;

        }

        if (!GameManager.instance.CanAttackTarget(gridPos, attackRange, GameManager.instance.characterID, out targetCharacterPos))
        {
            Debug.Log($"�L�����N�^�[���U���͈͊O�ɏo���̂�Idle�ɐ؂�ւ��܂�");
            state = State.Idle;
            return;
        }


        if (targetCharacter == null)
        { 
            Debug.Log($"�L�����N�^�[��|�����̂�Idle�ɐ؂�ւ��܂� targetCharacterPos:{targetCharacterPos}");
            state = State.Idle;
            return;
        }

        FacilityAttack();
    }
    protected void FacilityAttack()
    {
        Debug.Log($"FacilityAttack�����s���܂�");

        if (!isAttacking) StartCoroutine(FacilityAttackCoroutine());
    }

    IEnumerator FacilityAttackCoroutine()
    {
        float timer = 0;
        int damage;

        Debug.Log($"FacilityAttackCoroutine�����s���܂�");
         
        isAttacking = true;

        damage = GameManager.instance.CalcDamage(atk);

        Debug.Log($"Character({targetCharacterPos})��{damage}�̃_���[�W��^����");
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

    public abstract void Die(); //���ۃ��\�b�h

}

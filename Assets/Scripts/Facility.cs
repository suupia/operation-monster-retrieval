using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Facility : MonoBehaviour
{
    Animator animator;

    [SerializeField]private bool isEnemySide = true; //�G���̎{�݂��ǂ����𔻒肷�邽�߂ɕK�v �����l�̓C���X�y�N�^�[��Ō��߂Ă���
    public bool IsEnemySide //�v���p�e�B
    {
        get { return isEnemySide; }
        set { isEnemySide = value;
            if (isEnemySide)
            {
                targetUnitID = GameManager.instance.characterID;
            }
            else
            {
                targetUnitID = GameManager.instance.robotID;
            }
        }
    }
    [SerializeField] protected Vector2Int gridPos;

    [SerializeField] protected Vector2Int targetCharacterPos;

    [SerializeField] Unit targetCharacter;

    GameObject damageTextParent; //Find�Ŏ擾����
    [SerializeField] GameObject damageTextPrefab; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] float heightToDisplayDamage; //�_���[�W�e�L�X�g���ǂ̂��炢�����\�����邩�����߂�

    [SerializeField] GameObject cannonballPrefab; //�e�ۂ̃v���n�u�@�C���X�y�N�^��ŃZ�b�g����
    static float timeToImpact = 0.2f;     //���e�܂ł̎���
    public static float GetTimeToImpact() //��e�A�j���[�V�����Ƀf�B���C���|���邽�߂ɕK�v
    {
        return timeToImpact;
    }

    [SerializeField] protected int level;
    [SerializeField] protected int maxHp;
    [SerializeField] protected int hp;
    [SerializeField] protected int atk;
    [SerializeField] protected float attackInterval;
    [SerializeField] protected int attackRange;

    protected int targetUnitID; //isEnemySide�����Ƃɂ��āA�L�����N�^�[���U�����邩���{�b�g���U�����邩�ǂ��������߂�

    protected bool isAttacking = false;
    protected bool isFristBattle = true;
    protected bool isAlive = true; //HP��0�ɂȂ����Ƃ���Die()��2��ȏ�Ă΂��̂�h�����߂ɕK�v

    [SerializeField] protected float timeToRecover;  //��������Ă��畜������܂ł̎���(Tower�p)
    protected bool isRecovering; //�R���[�`���p

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
     public virtual int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <=0 && isAlive)
            {
                isAlive = false;  //������false�ɂ��āADie()��2��ȏ�Ă΂�Ȃ��悤�ɂ���
                Die();
            }

        }
    }
    public abstract void SetDirection(Vector2 directionVector); //TowerMGR�ł̂ݏ������K�v


    protected void Start()
    {
        animator = GetComponent<Animator>();

        damageTextParent = GameObject.Find("DamageTextParent");

        HP = maxHp;

        gridPos = GameManager.instance.ToGridPosition(transform.position);

        if (isEnemySide)
        {
            targetUnitID = GameManager.instance.characterID;
        }
        else
        {
            targetUnitID = GameManager.instance.robotID;
        }

    }

    private void Update()
    {
        if (GameManager.instance.state == GameManager.State.SelectingStage)
        {
            Destroy(this.gameObject);
        }

        if (GameManager.instance.state == GameManager.State.PauseTheGame) //�|�[�Y���̎���Update�̏��������Ȃ�
        {
            return;
        }

        if (isRecovering) return;  //���J�o�[���̂Ƃ���Update�̏��������Ȃ�

        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.InBattle:
                if (GameManager.instance.state == GameManager.State.ShowingResults) //�퓬���I�������A�U���͂��Ȃ�
                {
                    return;
                }
                Battle();
                break;
        }
    }

    protected void Idle() //Update()�ŌĂ΂�邱�Ƃɒ���
    {
        //Debug.Log("Facility��Idle�����s���܂�");

        if (Function.isWithinTheAttackRange(gridPos,attackRange,targetUnitID,out targetCharacterPos))
        {
            Debug.Log("�U���͈͓��ɃL�����N�^�[������̂�InBattele�ɐ؂�ւ��܂�");
            targetCharacter = GameManager.instance.mapMGR.GetMap().GetUnitList(targetCharacterPos)[0]; //�Ƃ肠�����P�̍U��
            Debug.Log($"targetCharacter:{targetCharacter}");
            state = State.InBattle;
            return;
        }
    }
    protected void Battle()
    {
        //Debug.Log($"Facility��Battle�����s���܂�");

        if (isFristBattle)
        {
            isFristBattle = false;

        }

        if (!Function.isWithinTheAttackRange(gridPos, attackRange, targetUnitID, out targetCharacterPos))
        {
            Debug.Log($"�L�����N�^�[���U���͈͊O�ɏo���̂�Idle�ɐ؂�ւ��܂� gridPos:{gridPos}�̎{��");
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
        //Debug.Log($"FacilityAttack�����s���܂�");

        if (!isAttacking) StartCoroutine(FacilityAttackCoroutine());
    }

    IEnumerator FacilityAttackCoroutine()
    {
        float timer = 0;
        int damage;

        Debug.Log($"FacilityAttackCoroutine�����s���܂�");
         
        isAttacking = true;

        damage = GameManager.instance.CalcDamage(atk);

        //�L�����N�^�[�̕�������
        SetDirection(targetCharacter.GetGridPos() - gridPos);

        //�_���[�W�̕\����C�ۂ̕\�����s���itargetCharacter�̏����g�����߁A��ɏ�������j
        DrawCannonBall();

        GameManager.instance.musicMGR.StartCombatSE("Cannon");

        targetCharacter.HP -= damage;

        Debug.Log("BeHitCoroutine���J�n���܂�");
        StartCoroutine(BeHitCoroutine(damage));


        while (timer < attackInterval){
            timer += Time.deltaTime * GameManager.instance.gameSpeed;

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //�|�[�Y���͎~�߂�

            yield return null;
        }

        isAttacking = false;
    }
    IEnumerator BeHitCoroutine(int damage)
    {
        yield return new WaitForSeconds(timeToImpact* (1.0f/ GameManager.instance.gameSpeed));

        Debug.Log("DrawDamage��targetCharacter.HP -= damage�����s���܂�");
        DrawDamage(damage);
        Debug.Log($"Character({targetCharacterPos})��{damage}�̃_���[�W��^����");
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
        Vector3 firingPos = this.transform.position + new Vector3(0, heightToDisplayDamage, 0); //�C�e�̏����ʒu

        //�C�e�𐶐�
        //Debug.Log("�C�ۂ𐶐����܂�");
        GameObject cannonballGO = Instantiate(cannonballPrefab, firingPos, Quaternion.identity);
        cannonballGO.GetComponent<CannonballMGR>().FiringCannonball(gridPos,timeToImpact,targetCharacter);

    }

    public abstract void Die(); //���ۃ��\�b�h

}

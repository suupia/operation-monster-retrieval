using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class Unit : MonoBehaviour
{

    protected Animator animator;

    [SerializeField] protected Vector2Int gridPos;

    protected int targetUnitID;   //�L�����N�^�[�ƃ��{�b�g�ŕW�I�̃��j�b�g��ID���Ⴄ
    protected int targetTowerID;  //�L�����N�^�[�ƃ��{�b�g�ŕW�I�̃^���[��ID���Ⴄ
    protected int targetCastleID; //�L�����N�^�[�ƃ��{�b�g�ŕW�I�̏��ID���Ⴄ
    [SerializeField] protected Vector2Int targetCastlePos;  //�f�o�b�O�p��SerializeField�ɂ��Ă���
    [SerializeField] protected Vector2Int targetFacilityPos;
    [SerializeField] protected Vector2Int targetUnitPos;
    [SerializeField] protected Facility targetFacility;     //�X�N���v�g��InBattle�̂Ƃ���񂾂��擾����
    [SerializeField] protected List<Unit> targetUnits;             //�X�N���v�g����񂾂��擾����
    [SerializeField] protected Vector2Int directionVectorToTarget;

    protected GameObject damageTextParent; //Find�Ŏ擾����
    [SerializeField] protected GameObject damageTextPrefab; //�C���X�y�N�^�[��ŃZ�b�g����
    protected float heightToDisplayDamage = 0.6f; //�_���[�W�e�L�X�g���ǂ̂��炢�����\�����邩�����߂�

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
                isAlive = false; //������false�ɂ��āADie()��2��ȏ�Ă΂�Ȃ��悤�ɂ���
                Die();
                return;
            }
            if (value - beforeHp < 0)
            {
                //Debug.Log("SetDamagedAnimation���Ăт܂�");
                StartCoroutine(SetDamageAnimation());
            }

        }
    } //�v���p�e�B
    [SerializeField] protected int atk;
    [SerializeField] protected float atkInterval;
    [SerializeField] protected int atkRange;
    [SerializeField] protected bool isAttackOfArea; //�͈͍U�����ۂ�
    [SerializeField] protected float spd; //1�b�Ԃɐi�ރ}�X�̐� [�}�X/s]  �Ƃ肠�����P�ɂ��Ă���
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
                    break;
            }
            _state = value;
        }
    } //�v���p�e�B
    protected enum State
    {
        Marching,
        InBattle
    }
    protected bool isAttacking = false;
    protected bool isMoving = false;

    protected bool isTargetUnit;   //�U���ڕW�����j�b�g���^���[�̂ǂ���ł��邩���f���邽�߂ɕK�v
    bool isAlive = true;  //HP��0�ɂȂ����Ƃ���Die()��2��ȏ�Ă΂��̂�h�����߂ɕK�v


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
            Debug.LogError("GetDirectionVector()�̖߂�l��(0,0)�ɂȂ��Ă��܂�");
        }
        return resultVector2Int;
    }
    public Vector2 GetTransformPosFromGridPos()
    {
        return GameManager.instance.ToWorldPosition(gridPos);
    }
    public Vector2 GetTransformPos() //���A���^�C���ōX�V�����transformPos��Ԃ�
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
        else if (directionVector.x <= 0)
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

    //MonoBehaviour��p
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
            //CharacterMGR�̂Ƃ�
            targetUnitID = GameManager.instance.robotID;
            targetTowerID = GameManager.instance.towerID;
            targetCastlePos = GameManager.instance.mapMGR.GetEnemysCastlePos();
            targetCastleID = GameManager.instance.enemyCastleID;
        }
        else
        {
            //RobotMGR�̂Ƃ�
            targetUnitID = GameManager.instance.characterID;
            targetTowerID = GameManager.instance.allyTowerID;
            targetCastlePos = GameManager.instance.mapMGR.GetAllysCastlePos();
            targetCastleID = GameManager.instance.allyCastleID;

        }

        //�G�̃^�C�v�ɕ�����routeList�̏�������ς���(�Ƃ肠�����ASetAutoRoute�����ɂ��Ă���)
        SetAutoRoute();


        InitiMoveAlongWith();
    }

    protected void Update()
    {
        if (GameManager.instance.state == GameManager.State.SelectingStage)
        {
            Destroy(this.gameObject);
        }

        if (GameManager.instance.state == GameManager.State.PauseTheGame) //�|�[�Y���̎���Update�̏��������Ȃ�
        {
            return;
        }


        switch (state)
        {
            case State.Marching:
                March();
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


    protected void SetAutoRoute()
    {
        Debug.Log("SetAutoRoute�����s���܂�");
        routeList = autoRoute.SearchShortestRoute(gridPos, targetCastlePos); //�Q�Ɠn��
        Debug.Log("routeList:" + string.Join(",", routeList));
    }

    private void InitiMoveAlongWith()
    {
        //moveAlongWithCounter = 0; //�J�E���^�[�̏�����

        if (!routeList.Contains(gridPos)) //�L�����N�^�[�̈ʒu���܂ނ��ǂ����m�F����
        {
            Debug.LogError($"routeList��gridPos:{gridPos}���܂݂܂���");
        }

    }

    private void March() //Update()�ŌĂ΂�邱�Ƃɒ���
    {
        MoveAlongWith();
    }



    private bool CanMove(Vector2Int vector)
    {

        if (GameManager.instance.mapMGR.GetMapValue(gridPos + vector) % GameManager.instance.wallID == 0)
        {
            Debug.LogError($"�ړ����wallID�����邽�߁A�ړ��ł��܂���(gridPos:{gridPos}vector:{vector})\nGameManager.instance.mapMGR.GetMapValue(gridPos + vector)={GameManager.instance.mapMGR.GetMapValue(gridPos + vector)} GetDirectionVector={GetDirectionVector()}");
            return false;
        }

        //�΂߈ړ��̎��Ƀu���b�N�̊p���ړ����邱�Ƃ͂ł��Ȃ�
        if (vector.x != 0 && vector.y != 0)
        {
            //���������̔���
            if (GameManager.instance.mapMGR.GetMapValue(gridPos.x + vector.x, gridPos.y) % GameManager.instance.wallID == 0)
            {
                Debug.LogError($"���������ɕǂ����邽�߁A�ړ��ł��܂���B");

                return false;
            }

            //���������̔���
            if (GameManager.instance.mapMGR.GetMapValue(gridPos.x, gridPos.y + vector.y) % GameManager.instance.wallID == 0)
            {
                Debug.LogError($"���������ɕǂ����邽�߁A�ړ��ł��܂���B");
                return false;
            }
        }

        return true;
    }
    private void MoveForward()
    {
        //Debug.Log("MoveForward�����s���܂�");
        if (!isMoving) StartCoroutine(MoveForwardCoroutine());
    }

    private IEnumerator MoveForwardCoroutine()  //Character��������蓮�����֐�
    {
        Debug.Log("MoveCoroutine�����s���܂�");
        Vector2 startPos;
        Vector2 endPos;

        isMoving = true;

        MoveData(GetDirectionVector()); //���MoveDate���s��



        startPos = transform.position;
        endPos = startPos + GetDirectionVector();

        Debug.Log($"MoveForwardCoroutine�ɂ�����startPos:{startPos},endPos{endPos}");


        float remainingDistance = (endPos - startPos).sqrMagnitude;

        while (remainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, 1f / moveTime * Time.deltaTime * GameManager.instance.gameSpeed);
            //3�ڂ̈�����"1�t���[���̍ő�ړ�����"�@�P�ʂ͎���[m/s](�R���[�`����1�t���[��������Ă��邩��Time.deltaTime��������BmoveTime�o��������1�}�X�i�ށB)

            remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;


            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //�|�[�Y���͎~�߂�


            yield return null;  //1�t���[����~������B
        }
        transform.position = endPos;//���[�v�𔲂������͂�������ړ�������B



        isMoving = false;
        //Debug.Log($"MoveCoroutine()�I������endPos��{endPos}");
    }
    private void MoveData(Vector2Int directionVector)
    {

        if (this is CharacterMGR)
        {
            //CharacterMGR�̂Ƃ�
            if (!(GameManager.instance.mapMGR.GetMapValue(gridPos) % GameManager.instance.characterID == 0))
            {
                Debug.LogError("MoveDate�ňړ��O��mapValue��characterID���܂܂�Ă��܂���");
                return;
            }

            //���l�f�[�^�̈ړ�
            GameManager.instance.mapMGR.DivisionalSetMapValue(gridPos, GameManager.instance.characterID);
            GameManager.instance.mapMGR.MultiplySetMapValue(gridPos + directionVector, GameManager.instance.characterID);

            //�X�N���v�g�̈ړ�
            GameManager.instance.mapMGR.GetMap().RemoveUnit(gridPos, this);
            GameManager.instance.mapMGR.GetMap().AddUnit(gridPos + directionVector, this.gameObject.GetComponent<CharacterMGR>());
        }
        else
        {
            //RobotMGR�̂Ƃ�
            if (!(GameManager.instance.mapMGR.GetMapValue(gridPos) % GameManager.instance.robotID == 0))
            {
                Debug.LogError("MoveDate�ňړ��O��mapValue��robotID���܂܂�Ă��܂���");
                return;
            }

            //���l�f�[�^�̈ړ�
            GameManager.instance.mapMGR.DivisionalSetMapValue(gridPos, GameManager.instance.robotID);
            GameManager.instance.mapMGR.MultiplySetMapValue(gridPos + directionVector, GameManager.instance.robotID);

            //�X�N���v�g�̈ړ�
            GameManager.instance.mapMGR.GetMap().RemoveUnit(gridPos, this);
            GameManager.instance.mapMGR.GetMap().AddUnit(gridPos + directionVector, this.gameObject.GetComponent<RobotMGR>());
        }

        //gridPos���ړ�������B����͍Ō�ɍs�����Ƃɒ��ӁI
        gridPos += directionVector;

    }
    private void MoveAlongWith()
    {
        Vector2Int nextPos;
        //Vector2Int nextNextPos;  RouteList�͊��Ɏ΂߂ɐi�߂�Ƃ���͎΂߂ɂ�����ԂɂȂ��Ă���̂ŁA���̕ϐ��Ɖ��̕��ɂ����if���̏����͂���Ȃ��B�R�����g�A�E�g���Ă���

        if (isMoving) return;

        CheckIfCauseSkill();

        if (Function.isWithinTheAttackRange(gridPos, atkRange, targetUnitID, out targetUnitPos))
        {
            Debug.Log($"�U���͈͓��Ƀ��j�b�g������̂�InBattle�ɐ؂�ւ��܂� targetUnitPos:{targetUnitPos}");
            if (isAttackOfArea)
            {
                //�͈͍U��
                targetUnits.Clear();
                foreach (Unit unit in GameManager.instance.mapMGR.GetMap().GetUnitList(targetUnitPos))
                {
                    targetUnits.Add(unit);
                }
            }
            else
            {
                //�P�̍U��
                targetUnits.Clear();
                targetUnits.Add(GameManager.instance.mapMGR.GetMap().GetUnitList(targetUnitPos)[0]);  //�}�X�̒��̐擪�̃��j�b�g�̃X�N���v�g���擾
            }
            Debug.Log($"targetUnit:{targetUnits}");
            SetDirection(targetUnitPos - gridPos);
            isTargetUnit = true;
            state = State.InBattle;
            return;
        }

        if (Function.isWithinTheAttackRange(gridPos, atkRange, targetTowerID, out targetFacilityPos) || Function.isWithinTheAttackRange(gridPos, atkRange, targetCastleID, out targetFacilityPos)) //���[�g�ɉ����Ĉړ����Ă���Ƃ��ɁA�U���͈͓��Ƀ^���[�i��������j������Ƃ�
        {
            Debug.Log($"�U���͈͓��Ƀ^���[������̂�InBattle�ɐ؂�ւ��܂� targetFacilityPos:{targetFacilityPos}");
            targetFacility = GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos); //�^���[�̃X�N���v�g���擾
            SetDirection(targetFacilityPos - gridPos);
            isTargetUnit = false;
            state = State.InBattle;
            return;
        }

        if (moveAlongWithCounter == routeList.Count - 1)  //���[�g�̍ŏI�n�_�ɓ��B�������ւ̍U�����J�n����
        {
            Debug.Log($"���[�g�̍ŏI�n�_�ɂ��邽��InBattle�ɐ؂�ւ��܂�");
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
            Debug.LogError($"CanMove({nextPos - gridPos})��false�Ȃ̂ňړ��ł��܂���");
        }

    }
    protected abstract void CheckIfCauseSkill(); //CharacterMGR�ł̂ݏ������������


    private void Battle()
    {
        //Debug.Log($"Battle�����s���܂� targetTowerPos:{targetTowerPos}");


        Attack();
    }

    private void Attack()
    {
        //Debug.Log($"Attack�����s���܂�");

        //���j�b�g���^�[�Q�b�g�̂Ƃ�
        targetUnits.RemoveAll(unit => unit  == null); //�|�������j�b�g�ւ̎Q�Ƃ�null�ɂȂ��Ă��邽�߂������菜��
        Debug.Log($"targetUnit:{targetUnits}");
        if (isTargetUnit && targetUnits.Count == 0)
        {
            Debug.Log($"�^�[�Q�b�g�̃��j�b�g��|�����̂�Marching�ɐ؂�ւ��܂� targetUnitPos:{targetUnitPos}");
            state = State.Marching;
            return;
        }

        //�^�[�Q�b�g���{�݂̂Ƃ�
        if (this is CharacterMGR)
        {
            //CharacterMGR�̂Ƃ�
            if (!isTargetUnit && !targetFacility.IsEnemySide)
            {
                Debug.Log($"�G�̃^���[�𐧈������̂�Marching�ɐ؂�ւ��܂� targetFacilityPos:{targetFacilityPos}");
                state = State.Marching;
                return;
            }
        }
        else
        {
            //RobatMGR�̂Ƃ�
            if (!isTargetUnit && targetFacility.IsEnemySide)
            {
                Debug.Log($"�v���C���[�̃^���[�𐧈������̂�Marching�ɐ؂�ւ��܂� targetFacilityPos:{targetFacilityPos}");
                state = State.Marching;
                return;
            }
        }


        if (!isAttacking) StartCoroutine(AttackCoroutine());

    }

    private IEnumerator AttackCoroutine()
    {
        float timer = 0;
        int damage;

        Debug.Log($"AttackCoroutine�����s���܂� targetUnitPos:{targetUnitPos}, targetFacilityPos{targetFacilityPos}");

        isAttacking = true;

        damage = CalcDamage(atk);

        if (isTargetUnit)
        {
            //�U���Ώۂ����j�b�g�̂Ƃ�
            foreach (Unit unit in targetUnits)
            {
                if (unit == null)
                {
                    Debug.LogWarning("targetUnits�̒���unit��null�̂��߁A���̃��[�v�ɓ���܂�");
                    continue;
                }
                Debug.Log($"Unit:{unit}(targetUnitPos:{targetUnitPos})��{damage}�̃_���[�W��^���܂�");
                unit.HP -= damage;
            }
        }
        else
        {
            //�U���Ώۂ��{�݂̂Ƃ�
            if(targetFacility == null)
            {
                Debug.LogError("targetFacilityt��null�̂ł�");
                //�R���[�`���Ȃ̂�return�ł��Ȃ�����A�G���[�̃��O�����o���Ă���
            }
            Debug.Log($"targetFacility:{targetFacility}(targetFacilityPos:{targetFacilityPos})��{damage}�̃_���[�W��^���܂�");
            targetFacility.HP -= damage;
        }

        DrawDamage(damage);

        while (timer < atkInterval)
        {
            timer += Time.deltaTime * GameManager.instance.gameSpeed;

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //�|�[�Y���͎~�߂�

            yield return null;
        }

        isAttacking = false;
    }

    private int CalcDamage(int atk)
    {

        int result = 0;

        if (GameManager.instance.mapMGR.GetMap().GetValue(targetFacilityPos) % targetCastleID == 0) //���^�[�Q�b�g�Ƃ��Ă���{�݂���ł������ꍇ�A�^���[�̐����l�������v�Z���ɕς���
        {
            if (this is CharacterMGR)
            {
                result = (int)Mathf.Ceil(atk * Mathf.Pow((float)(GameManager.instance.MaxTowerNum - GameManager.instance.CurrentTowerNum) / GameManager.instance.MaxTowerNum, 2)); //�؂�グ 2�悵�Ċ��炩�ɂ���
            }
            else
            {
                result = (int)Mathf.Ceil(atk * Mathf.Pow((float)(GameManager.instance.MaxTowerNum - GameManager.instance.CurrentAllyTowerNum) / GameManager.instance.MaxTowerNum, 2)); //�؂�グ 2�悵�Ċ��炩�ɂ���
            }

            if (result == 0) result = 1; //�^���[������j�󂵂Ă��Ȃ��ꍇ�A�v�Z���ʂ�0�ɂȂ邪�A1�_���[�W�͓���悤�ɂ���

            //Debug.Log($"CalcDamage��result:{result}");
        }
        else
        {
            //�Ƃ肠�����A�^���[�ւ̍U���͉������Ȃ��ł��̂܂ܒl��Ԃ�
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
                animator.SetTrigger("BackDamage");
                break;

            case Direction.DiagLeftBack:
            case Direction.DiagRightBack:
                animator.SetTrigger("DiagonalBackDamage");
                break;

            case Direction.Left:
            case Direction.Right:
                animator.SetTrigger("SideDamage");
                break;

            case Direction.DiagLeftFront:
            case Direction.DiagRightFront:
                animator.SetTrigger("DiagonalFrontDamage");
                break;

            case Direction.Front:
                animator.SetTrigger("FrontDamage");
                break;
        }
        //Debug.Log($"{direction}��damage�A�j���[�V������ݒ肵�܂���");

    }


    public abstract void Die();

}

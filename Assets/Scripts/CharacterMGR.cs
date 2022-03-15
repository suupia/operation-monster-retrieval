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

    GameObject damageTextParent; //Find�Ŏ擾����
    [SerializeField] GameObject damageTextPrefab; //�C���X�y�N�^�[��ŃZ�b�g����
    float heightToDisplayDamage = 0.6f; //�_���[�W�e�L�X�g���ǂ̂��炢�����\�����邩�����߂�

    [SerializeField] CharacterSkillMGR characterSkillMGR;
    [SerializeField] ParticleSystem skillIconParticle;
    Material skillAuraMaterial; 

    [SerializeField] string characterName;
    [SerializeField] int characterTypeID;
    [SerializeField] int level; //0����X�^�[�g���邱�Ƃɒ���
    int maxLevel=9; //�ő僌�x����10
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
                isAlive = false; //������false�ɂ��āADie()��2��ȏ�Ă΂�Ȃ��悤�ɂ���
                Die();
                return;
            }
            if (value - beforeHp < 0)
            {
                Debug.Log("SetDamagedAnimation���Ăт܂�");
                StartCoroutine(SetDamageAnimation());
            }

        }
    } //�v���p�e�B
    [SerializeField] int atk;
    [SerializeField] float atkInterval;
    [SerializeField] int atkRange;
    [SerializeField] float spd; //1�b�Ԃɐi�ރ}�X�̐� [�}�X/s]  �Ƃ肠�����P�ɂ��Ă���
    float moveTime; // movetime = 1/spd [s]
    [SerializeField] float coolTime;
    [SerializeField] int cost;
    [SerializeField] Sprite sprite; //�����G
    [SerializeField] Sprite thumbnailSprite; //�{�^���ɕ\������X�v���C�g
    [SerializeField] string introduction; //�����X�^�[�̏Љ

    [SerializeField] int[] hpGrowthRate; //���������C���X�y�N�^�[��Ō��߂Ă��� ex) 0,1,0 �Ȃ�A�ŏ��̃��x���A�b�v�ł͕ω������A���̃��x���A�b�v��1�オ��
    [SerializeField] int[] atkGrowthRate;
    [SerializeField] int[] spdGrowthRate;
    [SerializeField] int[] atkIntervalGrowthRate;
    [SerializeField] int[] atkRangeGrowthRate;
    [SerializeField] int[] coolTimeGrowthRate;


    //bool isMarching = false;  ���͎g���Ă��Ȃ�����R�����g�A�E�g
    bool isAlive = true;  //HP��0�ɂȂ����Ƃ���Die()��2��ȏ�Ă΂��̂�h�����߂ɕK�v
    bool isAttacking = false;
    bool isMoving = false;

    bool isFristBattle = true;

    AutoRouteData autoRoute;
    ManualRouteData manualRoute;
    List<Vector2Int> routeList;
    List<int> nonDiagonalPoints;

    int moveAlongWithCounter = 0;
    List<int> causeSkillPoints;  //moceAlongWithCounter��causeSkillPoint�Ɠ���������skill�𔭓�����

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
    } //�v���p�e�B
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
    } //�v���p�e�B
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

    public CharacterMGR Clone() //�f�B�[�v�R�s�[�p
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
    public void SetCharacterData(int buttonNum, int characterTypeID)  //hp��atk�Ȃǂ̏��������ŃZ�b�g����B
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
    IEnumerator SetDamageAnimation()
    {
        yield return new WaitForSeconds(Facility.GetTimeToImpact() * (1.0f/GameManager.instance.gameSpeed));

        switch (direction)
        {
            case Direction.Back:
                Debug.Log($"{direction}��damage�A�j���[�V������ݒ肵�܂�");
                animator.SetTrigger("BackDamage");
                break;

            case Direction.DiagLeftBack:
            case Direction.DiagRightBack:
                Debug.Log($"{direction}��damage�A�j���[�V������ݒ肵�܂�");

                animator.SetTrigger("DiagonalBackDamage");
                break;

            case Direction.Left:
            case Direction.Right:
                Debug.Log($"{direction}��damage�A�j���[�V������ݒ肵�܂�");

                animator.SetTrigger("SideDamage");
                break;

            case Direction.DiagLeftFront:
            case Direction.DiagRightFront:
                Debug.Log($"{direction}��damage�A�j���[�V������ݒ肵�܂�");

                animator.SetTrigger("DiagonalFrontDamage");
                break;

            case Direction.Front:
                Debug.Log($"{direction}��damage�A�j���[�V������ݒ肵�܂�");

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
            Debug.LogError("�ő僌�x���Ȃ̂Ń��x���A�b�v�ł��܂���"); //LevelUp()�̓��x���A�b�v�ł��邱�Ƃ��m�肵�Ă���Ƃ��ɂ̂݌Ă΂��̂ŁA���̃��O�̓G���[�����ɂ���
            return;
        }

        Debug.Log($"{characterName}�̃��x�����グ�܂�");

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
        skillAuraMaterial = GetComponent<Renderer>().material;  //�u�����_���[�Ɋ��蓖�Ă��Ă���ŏ��ɃC���X�^���X�����ꂽMaterial��Ԃ��܂��vby ���t�@�����X


        HP = maxHp;
        moveTime = 1 / spd;
        gridPos = GameManager.instance.ToGridPosition(transform.position);
        state = State.Marching;

        targetCastlePos = GameManager.instance.mapMGR.GetEnemysCastlePos();


        //route�Ɋւ��鏈��
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
        causeSkillPoints = characterSkillMGR.JudgeSkillTrigger(routeList); //skill����������_�𔻒肵�A����Index���󂯎��
        InitiMoveAlongWith();
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

        //if (GameManager.instance.state != GameManager.State.RunningGame) return; //RunningGame�ӊO��������Update�̏��������Ȃ�

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

    public void March() //Update()�ŌĂ΂�邱�Ƃɒ���
    {
        MoveAlongWith();
    }



    public bool CanMove(Vector2Int vector)
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
    public void MoveForward()
    {
        Debug.Log("MoveForward�����s���܂�");
        if (!isMoving) StartCoroutine(MoveForwardCoroutine());
    }

    IEnumerator MoveForwardCoroutine()  //Character��������蓮�����֐�
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
    public void MoveData(Vector2Int directionVector)
    {

        if (!(GameManager.instance.mapMGR.GetMapValue(gridPos) % GameManager.instance.characterID == 0))
        {
            Debug.LogError("MoveDate�ňړ��O��mapValue��characterID���܂܂�Ă��܂���");
            return;
        }

        //���l�f�[�^�̈ړ�
        GameManager.instance.mapMGR.DivisionalSetMapValue(gridPos, GameManager.instance.characterID);
        GameManager.instance.mapMGR.MultiplySetMapValue(gridPos + directionVector, GameManager.instance.characterID);

        //�X�N���v�g�̈ړ�
        GameManager.instance.mapMGR.GetMap().RemoveCharacterMGR(gridPos, this);
        GameManager.instance.mapMGR.GetMap().AddCharacterMGR(gridPos + directionVector, this.gameObject.GetComponent<CharacterMGR>());



        //gridPos���ړ�������B����͍Ō�ɍs�����Ƃɒ��ӁI
        gridPos += directionVector;

    }
    public void SetAutoRoute()
    {
        Debug.Log("SetAutoRoute�����s���܂�");
        routeList = autoRoute.SearchShortestRoute(gridPos, targetCastlePos); //�Q�Ɠn��
        nonDiagonalPoints = new List<int>(); //Auto�̂Ƃ��͎g��Ȃ��Ǝv�����AManual�Ƒ����邽�߂�new���Ă���
        Debug.Log("routeList:" + string.Join(",", routeList));
    }

    public void SetManualRoute()
    {
        Debug.Log("SetManualRoute�����s���܂�");
        if (manualRoute.GetManualRoute().Count == 0)
        {
            Debug.Log("manualRoute�̒�����0�̂���AutoMode�ɐ؂�ւ��܂�");
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
        //moveAlongWithCounter = 0; //�J�E���^�[�̏�����

        if (!routeList.Contains(gridPos)) //�L�����N�^�[�̈ʒu���܂ނ��ǂ����m�F����
        {
            Debug.LogError($"routeList��gridPos:{gridPos}���܂݂܂���");
        }

    }

    public void MoveAlongWith()
    {
        Vector2Int nextPos;
        //Vector2Int nextNextPos;  RouteList�͊��Ɏ΂߂ɐi�߂�Ƃ���͎΂߂ɂ�����ԂɂȂ��Ă���̂ŁA���̕ϐ��Ɖ��̕��ɂ����if���̏����͂���Ȃ��B�R�����g�A�E�g���Ă���

        if (isMoving) return;

        if(causeSkillPoints.Contains(moveAlongWithCounter))
        {
            characterSkillMGR.CauseSkill(this, skillIconParticle,skillAuraMaterial);
        }
        //if (moveAlongWithCounter == routeList.Count -1) //���[�g�̏I�_�ɂ���Ƃ��̏���
        //{
        //    Debug.Log("���[�g�̏I�_�ɂ���̂�InBattele�ɐ؂�ւ��܂�");
        //    SetDirection(targetCastlePos - gridPos);
        //    state = State.InBattle;
        //    return;
        //}

        if (GameManager.instance.CanAttackTarget(gridPos, atkRange, GameManager.instance.facilityID, out targetFacilityPos)) //���[�g�ɉ����Ĉړ����Ă���Ƃ��ɁA�U���͈͓��Ƀ^���[������Ƃ�
        {
            Debug.Log($"�U���͈͓��Ƀ^���[������̂�InBattele�ɐ؂�ւ��܂� targetFacilityPos:{targetFacilityPos}");
            SetDirection(targetFacilityPos - gridPos);
            state = State.InBattle;
            return;
        }

        nextPos = routeList[moveAlongWithCounter + 1];

        //if (moveAlongWithCounter < routeList.Count - 2 && !nonDiagonalPoints.Contains(moveAlongWithCounter))  //���΂߈ړ��ł���Ƃ��͂�������B
        //{
        //    nextNextPos = routeList[moveAlongWithCounter + 2];
        //    if (((nextPos - gridPos).x == 0 && (nextNextPos - nextPos).y == 0) || ((nextPos - gridPos).y == 0 && (nextNextPos - nextPos).x == 0)) //nextPos���p�}�X�̂Ƃ�true
        //    {
        //        if (CanMove(nextNextPos - gridPos))
        //        {
        //            Debug.Log($"�΂߈ړ����\�Ȃ��߁AnextPos:{nextPos}��nextNextPos:{nextNextPos}�Œu�������܂� gridPos:{gridPos}");
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
            Debug.LogError($"CanMove({nextPos - gridPos})��false�Ȃ̂ňړ��ł��܂���");
        }

    }

    public void Battle()
    {
        //Debug.LogWarning($"Battle�����s���܂� targetTowerPos:{targetTowerPos}");

        if (isFristBattle)
        {
            isFristBattle = false;

            targetFacility = GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos);
        }

        Attack();
    }

    public void Attack()
    {
        //Debug.Log($"Attack�����s���܂�");

        if (GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos) == null)
        { //towerMGR���Ȃ��Ƃ������Ƃ̓^���[��j�󂵂��Ƃ������ƂȂ̂ŁAMarching�ɐ؂�ւ���
            Debug.Log($"�^���[��j�󂵂��̂�Marching�ɐ؂�ւ��܂� targetFacilityPos:{targetFacilityPos}");
            state = State.Marching;
            return;
        }

        if (!isAttacking) StartCoroutine(AttackCoroutine());

    }

    IEnumerator AttackCoroutine()
    {
        float timer = 0;
        int damage;

        Debug.Log($"AttackCoroutine�����s���܂�");

        isAttacking = true;

        damage = CalcDamage(atk);

        Debug.Log($"Facility({targetFacilityPos})��{damage}�̃_���[�W��^����");
        targetFacility.HP -= damage;
        DrawDamage(damage);

        while (timer < atkInterval)
        {
            timer += Time.deltaTime * GameManager.instance.gameSpeed;

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //�|�[�Y���͎~�߂�

            yield return null;
        }

        isAttacking = false;
    }

    public int CalcDamage(int atk)
    {

        int result =0;

        if(GameManager.instance.mapMGR.GetMap().GetFacility(targetFacilityPos) is CastleMGR) //���^�[�Q�b�g�Ƃ��Ă���{�݂���ł������ꍇ�A�^���[�̐����l�������v�Z���ɕς���
        {
            result = (int)Mathf.Ceil(atk * Mathf.Pow((float)(GameManager.instance.MaxTowerNum-GameManager.instance.CurrentTowerNum)/GameManager.instance.MaxTowerNum,2)); //�؂�グ 2�悵�Ċ��炩�ɂ���

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
        Debug.Log($"HP��0�ȉ��ɂȂ����̂ŁA�L�����N�^�[���������܂� gridPos:{gridPos},transform.pos{transform.position}�̃L�����N�^�[");

        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.characterID); //���l�f�[�^������������
        GameManager.instance.CurrentCharacterNum--;
        GameManager.instance.mapMGR.GetMap().RemoveCharacterMGR(gridPos, this);
        //GameManager.instance.mapMGR.GetMap().SetCharacterMGR(gridPos,null); //�X�N���v�g������������


        Destroy(this.gameObject);
    }
}


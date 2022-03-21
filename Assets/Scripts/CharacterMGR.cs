using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterSkillMGR))]
public class CharacterMGR : Unit
{
    [SerializeField] CharacterSkillMGR characterSkillMGR;
    [SerializeField] ParticleSystem skillIconParticle;
    Material skillAuraMaterial;

    [SerializeField] string characterName;
    [SerializeField] int characterTypeID; //�L�����N�^�[�̎�ނ����ʂ��邽�߂ɕK�v
    [SerializeField] int level; //0����X�^�[�g���邱�Ƃɒ���
    int maxLevel = 9; //�ő僌�x����10


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




    ManualRouteData manualRoute;

    List<int> causeSkillPoints;  //moceAlongWithCounter��causeSkillPoint�Ɠ���������skill�𔭓�����



    Mode _mode; //Auto���[�h��Manual���[�h
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



    public CharacterMGR Clone() //�f�B�[�v�R�s�[�p
    {
        return (CharacterMGR)MemberwiseClone();
    }

    //Getter
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
    public void SetSpd(float spdNum)
    {
        spd = spdNum;
        moveTime = 1 / spd;
    }
    public void SetCharacterData(int buttonNum, int characterTypeID)  //hp��atk�Ȃǂ̏��������ŃZ�b�g����B
    {
        autoRoute = GameManager.instance.characterAutoRouteDatas[buttonNum];
        manualRoute = GameManager.instance.characterManualRouteDatas[buttonNum];

        level = GameManager.instance.GetCharacterDatabase(characterTypeID).GetLevel();
        maxHp = GameManager.instance.GetCharacterDatabase(characterTypeID).GetMaxHp();
        atk = GameManager.instance.GetCharacterDatabase(characterTypeID).GetAtk();
        atkInterval = GameManager.instance.GetCharacterDatabase(characterTypeID).GetAtkInterval();
        atkRange = GameManager.instance.GetCharacterDatabase(characterTypeID).GetAtkRange();
        spd = GameManager.instance.GetCharacterDatabase(characterTypeID).GetSpd();
        coolTime = GameManager.instance.GetCharacterDatabase(characterTypeID).GetCoolTime();

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
    new void Start()
    {
        base.Start();

        GetComponent<Renderer>().material = GameManager.instance.characterSkillsDataMGR.skillAuraMaterials[characterSkillMGR.GetSkillNum()];
        skillAuraMaterial = GetComponent<Renderer>().material;  //�u�����_���[�Ɋ��蓖�Ă��Ă���ŏ��ɃC���X�^���X�����ꂽMaterial��Ԃ��܂��vby ���t�@�����X


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
        Debug.Log("routeList:" + string.Join(",", routeList));
    }


    protected override void CheckIfCauseSkill()
    {
        if (causeSkillPoints.Contains(moveAlongWithCounter))
        {
            characterSkillMGR.CauseSkill(this, skillIconParticle, skillAuraMaterial);
            GameManager.instance.musicMGR.StartCombatSE("Skill");
        }
    }
    public override void Die()
    {
        Debug.Log($"HP��0�ȉ��ɂȂ����̂ŁA�L�����N�^�[���������܂� gridPos:{gridPos},transform.pos{transform.position}�̃L�����N�^�[");

        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.characterID); //���l�f�[�^������������
        GameManager.instance.CurrentCharacterNum--;
        GameManager.instance.mapMGR.GetMap().RemoveUnit(gridPos, this);
        //GameManager.instance.mapMGR.GetMap().SetCharacterMGR(gridPos,null); //�X�N���v�g������������

        GameManager.instance.musicMGR.StartCombatSE("Explosion");
        Destroy(this.gameObject);
    }
}

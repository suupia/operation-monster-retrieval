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
    [SerializeField] int characterTypeID; //キャラクターの種類を識別するために必要
    [SerializeField] int level; //0からスタートすることに注意
    int maxLevel = 9; //最大レベルは10


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




    ManualRouteData manualRoute;

    List<int> causeSkillPoints;  //moceAlongWithCounterがcauseSkillPointと等しい時にskillを発動する



    Mode _mode; //AutoモードかManualモード
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



    public CharacterMGR Clone() //ディープコピー用
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
    public void SetCharacterData(int buttonNum, int characterTypeID)  //hpやatkなどの情報もここでセットする。
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
    new void Start()
    {
        base.Start();

        GetComponent<Renderer>().material = GameManager.instance.characterSkillsDataMGR.skillAuraMaterials[characterSkillMGR.GetSkillNum()];
        skillAuraMaterial = GetComponent<Renderer>().material;  //「レンダラーに割り当てられている最初にインスタンス化されたMaterialを返します」by リファレンス


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
        Debug.Log($"HPが0以下になったので、キャラクターを消去します gridPos:{gridPos},transform.pos{transform.position}のキャラクター");

        GameManager.instance.mapMGR.GetMap().DivisionalSetValue(gridPos, GameManager.instance.characterID); //数値データをを消去する
        GameManager.instance.CurrentCharacterNum--;
        GameManager.instance.mapMGR.GetMap().RemoveUnit(gridPos, this);
        //GameManager.instance.mapMGR.GetMap().SetCharacterMGR(gridPos,null); //スクリプトをを消去する

        GameManager.instance.musicMGR.StartCombatSE("Explosion");
        Destroy(this.gameObject);
    }
}

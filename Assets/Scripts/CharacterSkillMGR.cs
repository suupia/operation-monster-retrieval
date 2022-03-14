using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkillMGR : MonoBehaviour //各キャラクターのPrefabにinstanceをセットする
{
    [SerializeField] private int skillNum;
    [SerializeField] private float duration; //スキルの継続時間 SkillNumに合わせてスキルが選ばれる
    [SerializeField] private int amount; //スキルによる変化量
    [SerializeField] private int skillRemainingeTime; //現在の、スキルが重複して発動した回数 デバッグ用に[SerializeField]しておく

    //Getter
    public int GetSkillNum()
    {
        return skillNum;
    }
    public string GetSkillSentence(int skillNum)
    {
        string skillSentence;

        switch (skillNum)
        {
            case 0:
                skillSentence = duration + "秒間SPDが" + amount + "上昇";
                break;
            case 1:
                skillSentence = "残り時間が" + amount + "秒増加";
                break;
            case 2:
                skillSentence = duration + "秒間ATKが" + amount + "上昇";
                break;
            default:
                Debug.LogError("skillNumが不適切です。skillNum=" + skillNum);
                skillSentence = "skillNumが不適切です";
                break;
        }
        return skillSentence;
    }
    public List<int> JudgeSkillTrigger(List<Vector2Int> routeList) //CharacterMGRが生成されたときに呼ぶ。戻り値はCharacterMGRのcauseSkillPointに代入する
    {
        switch (skillNum)
        {
            case 0:
                return JudgeSkillTrigger_0(routeList);
            case 1:
                return JudgeSkillTrigger_1(routeList);
            case 2:
                return JudgeSkillTrigger_2(routeList);
            default:
                Debug.LogError("skillNumが不適切です。skillNum=" + skillNum);
                return null;

        }
    }

    //以下、各skillの具体的な発動条件をかく
    private List<int> JudgeSkillTrigger_0(List<Vector2Int> routeList) //1×1の正方形を描くとき、最後の点で発動
    {
        List<int> causeSkillPoints = new List<int>();
        for (int i = 0; i < routeList.Count - 4; i++)
        {
            if (routeList[i + 4] == routeList[i] &&
                (routeList[i].x == routeList[i + 1].x && routeList[i + 1].y == routeList[i + 2].y && routeList[i + 2].x == routeList[i + 3].x && routeList[i + 3].y == routeList[i].y) ||
                (routeList[i].y == routeList[i + 1].y && routeList[i + 1].x == routeList[i + 2].x && routeList[i + 2].y == routeList[i + 3].y && routeList[i + 3].x == routeList[i].x))
            {
                Debug.LogWarning("Skill_0の発動条件を満たしています:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
                causeSkillPoints.Add(i + 4);
                i += 4;
            }
        }
        return causeSkillPoints;  //発動条件を満たさないときは空のListを返す
    }
    private List<int> JudgeSkillTrigger_1(List<Vector2Int> routeList) //√2×√2の正方形を描くとき、最後の点で発動
    {
        List<int> causeSkillPoints = new List<int>();
        for (int i = 0; i < routeList.Count - 4; i++)
        {
            if (routeList[i + 4] == routeList[i] &&
                (routeList[i].x == routeList[i + 2].x && routeList[i + 1].y == routeList[i + 3].y && Mathf.Abs(routeList[i].y - routeList[i + 2].y) == 2 && Mathf.Abs(routeList[i + 1].x - routeList[i + 3].x) == 2) ||
                (routeList[i].y == routeList[i + 2].y && routeList[i + 1].x == routeList[i + 3].x && Mathf.Abs(routeList[i].x - routeList[i + 2].x) == 2 && Mathf.Abs(routeList[i + 1].y - routeList[i + 3].y) == 2))
            {
                Debug.LogWarning("Skill_1の発動条件を満たしています:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
                causeSkillPoints.Add(i + 4);
                i += 4;
            }
        }
        return causeSkillPoints;  //発動条件を満たさないときは空のListtを返す
    }
    private List<int> JudgeSkillTrigger_2(List<Vector2Int> routeList) //2×2の正方形を描くとき、最後の点で発動
    {
        List<int> causeSkillPoints = new List<int>();
        for (int i = 0; i < routeList.Count - 8; i++)
        {
            if (routeList[i + 8] == routeList[i] &&
                (routeList[i].x == routeList[i + 2].x && routeList[i + 2].y == routeList[i + 4].y && routeList[i + 4].x == routeList[i + 6].x && routeList[i + 6].y == routeList[i].y) ||
                (routeList[i].y == routeList[i + 2].y && routeList[i + 2].x == routeList[i + 4].x && routeList[i + 4].y == routeList[i + 6].y && routeList[i + 6].x == routeList[i].x))
            {
                Debug.LogWarning("Skill_2の発動条件を満たしています:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4] + routeList[i + 5] + routeList[i + 6] + routeList[i + 7] + routeList[i + 8]);
                causeSkillPoints.Add(i + 8);
                i += 8;
            }
        }
        return causeSkillPoints;  //発動条件を満たさないときは-1を返す
    }


    public void CauseSkill(CharacterMGR characterMGRInstace, ParticleSystem particleSystem , Material skillAuraMaterial) //skillNumに対応したskillを発動する CharacterMGRのMoveAlongWith内で呼ぶ
    {
        if (characterMGRInstace == null)
        {
            Debug.LogError($"characterMGRInstaceがnullです");
            return;
        }

        switch (skillNum)
        {
            case 0:
                StartCoroutine(Skill_0(characterMGRInstace, particleSystem, skillAuraMaterial));
                break;
            case 1:
                StartCoroutine(Skill_1(characterMGRInstace, particleSystem, skillAuraMaterial));
                break;
            case 2:
                StartCoroutine(Skill_2(characterMGRInstace, particleSystem, skillAuraMaterial));
                break;

            default:
                Debug.LogError("skillNumが不適切です。skillNum=" + skillNum);
                break;

        }
    }

    //以下、具体的なskillの内容をかく
    private IEnumerator Skill_0(CharacterMGR characterMGRInstace, ParticleSystem particleSystem, Material skillAuraMaterial) //SPDをあげる
    {
        PlaySkillIconParticle(particleSystem);
        skillRemainingeTime++;
        if (skillRemainingeTime != 1)
        {
            Debug.LogWarning("skill_0は発動中です。skillRemainingTime=" + skillRemainingeTime);
            yield break;  //既にこのスキルのCoroutineが動いているので、このCoroutineは終わる
        }

        Debug.LogWarning("Skill_0を発動します。Name=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetSpd(characterMGRInstace.GetSpd() + amount);
        PlaySkillAuraParticle(particleSystem, skillAuraMaterial);

        while (skillRemainingeTime != 0)
        {
            if (characterMGRInstace == null)
            {
                skillRemainingeTime = 0; yield break; //GameObjectがDestroyされた時などはコルーチンを破棄する
            }

            yield return new WaitForSeconds(duration);
            skillRemainingeTime--;
            Debug.LogWarning("Skill_0の発動からdurationが経過しました。skillRemainingTime=" + skillRemainingeTime);
        }

        if (characterMGRInstace == null) yield break; //GameObjectがDestroyされた時などはコルーチンを破棄する

        Debug.LogWarning("Skill_0を終了します。Name=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetSpd(characterMGRInstace.GetSpd() - amount);
        StopSkillAuraParticle(particleSystem, skillAuraMaterial);
    }
    private IEnumerator Skill_1(CharacterMGR characterMGRInstace, ParticleSystem particleSystem, Material skillAuraMaterial) //ATKをあげる
    {
        PlaySkillIconParticle(particleSystem);
        skillRemainingeTime++;
        if (skillRemainingeTime != 1)
        {
            Debug.LogWarning("skill_1は発動中です。skillRemainingTime=" + skillRemainingeTime);
            yield break;  //既にこのスキルのCoroutineが動いているので、このCoroutineは終わる
        }

        Debug.LogWarning("Skill_1を発動します。Name=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetAtk(characterMGRInstace.GetAtk() + amount);
        PlaySkillAuraParticle(particleSystem, skillAuraMaterial);

        while (skillRemainingeTime != 0)
        {
            if (characterMGRInstace == null)
            {
                skillRemainingeTime = 0; yield break; //GameObjectがDestroyされた時などはコルーチンを破棄する
            }

            yield return new WaitForSeconds(duration);
            skillRemainingeTime--;
            Debug.LogWarning("Skill_1の発動からdurationが経過しました。skillRemainingTime=" + skillRemainingeTime);
        }

        if (characterMGRInstace == null) yield break; //GameObjectがDestroyされた時などはコルーチンを破棄する

        Debug.LogWarning("Skill_1を終了します。Name=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetAtk(characterMGRInstace.GetAtk() - amount);
        StopSkillAuraParticle(particleSystem, skillAuraMaterial);
    }
    private IEnumerator Skill_2(CharacterMGR characterMGRInstace, ParticleSystem particleSystem, Material skillAuraMaterial) //残り時間延長
    {
        PlaySkillIconParticle(particleSystem);
        skillRemainingeTime++;
        if (skillRemainingeTime != 1)
        {
            Debug.LogWarning("skill_2は発動中です。skillRemainingTime=" + skillRemainingeTime);
            yield break;  //既にこのスキルのCoroutineが動いているので、このCoroutineは終わる
        }

        Debug.LogWarning("Skill_2を発動しました。Name=" + characterMGRInstace.gameObject.name);
        GameManager.instance.timerMGR.Timer -= amount;
        PlaySkillAuraParticle(particleSystem, skillAuraMaterial);

        while (skillRemainingeTime != 0)
        {
            if (characterMGRInstace == null)
            {
                skillRemainingeTime = 0; yield break; //GameObjectがDestroyされた時などはコルーチンを破棄する
            }

            yield return new WaitForSeconds(duration);
            skillRemainingeTime--;
            Debug.LogWarning("Skill_2の発動からdurationが経過しました。skillRemainingTime=" + skillRemainingeTime);
        }

        yield return new WaitForSeconds(duration);
        StopSkillAuraParticle(particleSystem, skillAuraMaterial);
    }

    private void PlaySkillAuraParticle(ParticleSystem particleSystem, Material skillAuraMaterial)
    {
        //particleSystem.GetComponent<ParticleSystemRenderer>().material = GameManager.instance.characterSkillsDataMGR.skillIconMaterials[skillNum]; //iconパーティクル用の画像をskillNumに応じてセットする
        Debug.LogWarning("SkillのParticleを再生します");
        //particleSystem.Play();

        //スキル発動時のオーラを付ける
        skillAuraMaterial.SetInt("_isActivatingSkill", 1); //SetBoolはないことに注意
        //characterMGRInstance.GetComponent<Renderer>().material.SetInt("Boolean_7659173a6b1843508b1713d3c0bdd4d2", 1);

    }

    private void StopSkillAuraParticle(ParticleSystem particleSystem,Material skillAuraMaterial)
    {
        if (particleSystem == null) return;

        Debug.LogWarning("SkillのParticleを停止します");
        //particleSystem.Stop();

        //スキル発動時のオーラを付ける
        skillAuraMaterial.SetInt("_isActivatingSkill", 0);
    }
    private void PlaySkillIconParticle(ParticleSystem particleSystem)
    {
        particleSystem.GetComponent<ParticleSystemRenderer>().material = GameManager.instance.characterSkillsDataMGR.skillIconMaterials[skillNum]; //iconパーティクル用の画像をskillNumに応じてセットする
        particleSystem.Play();

    }



}

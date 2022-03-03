using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkillsMGR : MonoBehaviour
{
    [SerializeField] private float durationOfSkill_0; //スキルの継続時間（WaitForSeconds()で使う）
    //private float durationOfSkill_1 = 3;
    [SerializeField] private float durationOfSkill_2; //スキルの継続時間（WaitForSeconds()で使う）
    [SerializeField] private float spdIncrease; //スキルによるSPDの増加量
    [SerializeField] private int atkIncrease; //スキルによるATKの増加量
    [SerializeField] private int timerDecrease; //スキルによる残り時間の増加量
    public int JudgeSkillTrigger(int skillNum, List<Vector2Int> routeList) //CharacterMGRが生成されたときに呼ぶ。戻り値はCharacterMGRのcauseSkillPointに代入する
    {
        switch (skillNum)
        {
            case 0:
                return JudgeSkillTrigger_0(routeList);
            case 1:
                return JudgeSkillTrigger_1(routeList);
            case 2:
                return JudgeSkillTrigger_2(routeList);
            //case 3:
            //    return JudgeSkillTrigger_3(routeList);
            default:
                Debug.LogError("skillNumが不適切です。skillNum=" + skillNum);
                return -1;

        }
    }

    //以下、各skillの具体的な発動条件をかく
    private int JudgeSkillTrigger_0(List<Vector2Int> routeList) //1×1の正方形を描くとき、最後の点で発動
    {
        for (int i = 0; i < routeList.Count - 4; i++)
        {
            if (routeList[i + 4] == routeList[i] &&
                (routeList[i].x == routeList[i + 1].x && routeList[i + 1].y == routeList[i + 2].y && routeList[i + 2].x == routeList[i + 3].x && routeList[i + 3].y == routeList[i].y) ||
                (routeList[i].y == routeList[i + 1].y && routeList[i + 1].x == routeList[i + 2].x && routeList[i + 2].y == routeList[i + 3].y && routeList[i + 3].x == routeList[i].x))
            {
                Debug.LogWarning("Skill_0の発動条件を満たしています:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
                return i + 4;
            }
        }
        return -1;  //発動条件を満たさないときは-1を返す
    }
    private int JudgeSkillTrigger_1(List<Vector2Int> routeList) //2×2の正方形を描くとき、最後の点で発動
    {
        for (int i = 0; i < routeList.Count - 8; i++)
        {
            if (routeList[i + 8] == routeList[i] &&
                (routeList[i].x == routeList[i + 2].x && routeList[i + 2].y == routeList[i + 4].y && routeList[i + 4].x == routeList[i + 6].x && routeList[i + 6].y == routeList[i].y) ||
                (routeList[i].y == routeList[i + 2].y && routeList[i + 2].x == routeList[i + 4].x && routeList[i + 4].y == routeList[i + 6].y && routeList[i + 6].x == routeList[i].x))
            {
                Debug.LogWarning("Skill_1の発動条件を満たしています:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4] + routeList[i + 5] + routeList[i + 6] + routeList[i + 7] + routeList[i + 8]);
                return i + 8;
            }
        }
        return -1;  //発動条件を満たさないときは-1を返す
    }
    private int JudgeSkillTrigger_2(List<Vector2Int> routeList) //√2×√2の正方形を描くとき、最後の点で発動
    {
        for (int i = 0; i < routeList.Count - 4; i++)
        {
            if (routeList[i + 4] == routeList[i] &&
                (routeList[i].x == routeList[i + 2].x && routeList[i + 1].y == routeList[i + 3].y && Mathf.Abs(routeList[i].y - routeList[i + 2].y) == 2 && Mathf.Abs(routeList[i + 1].x - routeList[i + 3].x) == 2) ||
                (routeList[i].y == routeList[i + 2].y && routeList[i + 1].x == routeList[i + 3].x && Mathf.Abs(routeList[i].x - routeList[i + 2].x) == 2 && Mathf.Abs(routeList[i + 1].y - routeList[i + 3].y) == 2))
            {
                Debug.LogWarning("Skill_2の発動条件を満たしています:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
                return i + 4;
            }
        }
        return -1;  //発動条件を満たさないときは-1を返す
    }
    //private int JudgeSkillTrigger_3(List<Vector2Int> routeList) //右下がりの1×√2平行四辺形を描くとき、最後の点で発動
    //{
    //    for (int i = 0; i < routeList.Count - 4; i++)
    //    {
    //        if (routeList[i + 4] == routeList[i] &&
    //            (routeList[i + 1] - routeList[i] == routeList[i + 2] - routeList[i + 3] && routeList[i + 2] - routeList[i + 1] == routeList[i + 3] - routeList[i + 4]) && 
    //            (((routeList[i + 1] - routeList[i]).x == 0 && (routeList[i + 2] - routeList[i + 1]).x * (routeList[i + 2] - routeList[i + 1]).y < 0)) || 
    //            ((routeList[i + 2] - routeList[i + 1]).x == 0 && (routeList[i + 1] - routeList[i]).x * (routeList[i + 1] - routeList[i]).y < 0))   //二組の対辺が平行、かつ、「一辺はy軸に平行で、もう一辺は傾きが負」
    //        {
    //            Debug.LogWarning("Skill_3の発動条件を満たしています:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
    //            return i + 4;
    //        }
    //    }
    //    return -1;  //発動条件を満たさないときは-1を返す
    //}

    public void CauseSkill(int skillNum, CharacterMGR characterMGRInstace) //skillNumに対応したskillを発動する CharacterMGRのMoveAlongWith内で呼ぶ
    {
        switch (skillNum)
        {
            case 0:
                StartCoroutine(Skill_0(characterMGRInstace));
                break;
            case 1:
                Skill_1(characterMGRInstace);
                break;
            case 2:
                StartCoroutine(Skill_2(characterMGRInstace));
                break;
            //case 3:
            //    Skill_3();
            //    break;
            default:
                Debug.LogError("skillNumが不適切です。skillNum=" + skillNum);
                break;

        }
    }

    //以下、具体的なskillの内容をかく
    private IEnumerator Skill_0(CharacterMGR characterMGRInstace) //SPDをあげる
    {
        Debug.LogWarning("Skill_0を発動します。Name=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetSpd(characterMGRInstace.GetSpd() + spdIncrease);

        yield return new WaitForSeconds(durationOfSkill_0);

        if (characterMGRInstace == null) yield break; //GameObjectがDestroyされた時などはコルーチンを破棄する

        Debug.LogWarning("Skill_0を終了します。Name=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetSpd(characterMGRInstace.GetSpd() - spdIncrease);

    }
    private void Skill_1(CharacterMGR characterMGRInstace) //残り時間延長
    {
        Debug.LogWarning("Skill_1を発動します。Name=" + characterMGRInstace.gameObject.name);
        GameManager.instance.timerMGR.Timer -= timerDecrease;
        return;
    }
    private IEnumerator Skill_2(CharacterMGR characterMGRInstace) //ATKをあげる
    {
        Debug.LogWarning("Skill_2を発動します。Name=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetAtk(characterMGRInstace.GetAtk() + atkIncrease);

        yield return new WaitForSeconds(durationOfSkill_2);

        if (characterMGRInstace == null) yield break; //GameObjectがDestroyされた時などはコルーチンを破棄する

        Debug.LogWarning("Skill_2を終了します。Name=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetAtk(characterMGRInstace.GetAtk() - atkIncrease);
    }
    //private void Skill_3()
    //{
    //    Debug.LogWarning("Skill_3を発動します");
    //}

}

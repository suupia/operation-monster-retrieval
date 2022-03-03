using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkillsMGR : MonoBehaviour
{
    [SerializeField] private float durationOfSkill_0; //�X�L���̌p�����ԁiWaitForSeconds()�Ŏg���j
    //private float durationOfSkill_1 = 3;
    [SerializeField] private float durationOfSkill_2; //�X�L���̌p�����ԁiWaitForSeconds()�Ŏg���j
    [SerializeField] private float spdIncrease; //�X�L���ɂ��SPD�̑�����
    [SerializeField] private int atkIncrease; //�X�L���ɂ��ATK�̑�����
    [SerializeField] private int timerDecrease; //�X�L���ɂ��c�莞�Ԃ̑�����
    public int JudgeSkillTrigger(int skillNum, List<Vector2Int> routeList) //CharacterMGR���������ꂽ�Ƃ��ɌĂԁB�߂�l��CharacterMGR��causeSkillPoint�ɑ������
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
                Debug.LogError("skillNum���s�K�؂ł��BskillNum=" + skillNum);
                return -1;

        }
    }

    //�ȉ��A�eskill�̋�̓I�Ȕ�������������
    private int JudgeSkillTrigger_0(List<Vector2Int> routeList) //1�~1�̐����`��`���Ƃ��A�Ō�̓_�Ŕ���
    {
        for (int i = 0; i < routeList.Count - 4; i++)
        {
            if (routeList[i + 4] == routeList[i] &&
                (routeList[i].x == routeList[i + 1].x && routeList[i + 1].y == routeList[i + 2].y && routeList[i + 2].x == routeList[i + 3].x && routeList[i + 3].y == routeList[i].y) ||
                (routeList[i].y == routeList[i + 1].y && routeList[i + 1].x == routeList[i + 2].x && routeList[i + 2].y == routeList[i + 3].y && routeList[i + 3].x == routeList[i].x))
            {
                Debug.LogWarning("Skill_0�̔��������𖞂����Ă��܂�:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
                return i + 4;
            }
        }
        return -1;  //���������𖞂����Ȃ��Ƃ���-1��Ԃ�
    }
    private int JudgeSkillTrigger_1(List<Vector2Int> routeList) //2�~2�̐����`��`���Ƃ��A�Ō�̓_�Ŕ���
    {
        for (int i = 0; i < routeList.Count - 8; i++)
        {
            if (routeList[i + 8] == routeList[i] &&
                (routeList[i].x == routeList[i + 2].x && routeList[i + 2].y == routeList[i + 4].y && routeList[i + 4].x == routeList[i + 6].x && routeList[i + 6].y == routeList[i].y) ||
                (routeList[i].y == routeList[i + 2].y && routeList[i + 2].x == routeList[i + 4].x && routeList[i + 4].y == routeList[i + 6].y && routeList[i + 6].x == routeList[i].x))
            {
                Debug.LogWarning("Skill_1�̔��������𖞂����Ă��܂�:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4] + routeList[i + 5] + routeList[i + 6] + routeList[i + 7] + routeList[i + 8]);
                return i + 8;
            }
        }
        return -1;  //���������𖞂����Ȃ��Ƃ���-1��Ԃ�
    }
    private int JudgeSkillTrigger_2(List<Vector2Int> routeList) //��2�~��2�̐����`��`���Ƃ��A�Ō�̓_�Ŕ���
    {
        for (int i = 0; i < routeList.Count - 4; i++)
        {
            if (routeList[i + 4] == routeList[i] &&
                (routeList[i].x == routeList[i + 2].x && routeList[i + 1].y == routeList[i + 3].y && Mathf.Abs(routeList[i].y - routeList[i + 2].y) == 2 && Mathf.Abs(routeList[i + 1].x - routeList[i + 3].x) == 2) ||
                (routeList[i].y == routeList[i + 2].y && routeList[i + 1].x == routeList[i + 3].x && Mathf.Abs(routeList[i].x - routeList[i + 2].x) == 2 && Mathf.Abs(routeList[i + 1].y - routeList[i + 3].y) == 2))
            {
                Debug.LogWarning("Skill_2�̔��������𖞂����Ă��܂�:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
                return i + 4;
            }
        }
        return -1;  //���������𖞂����Ȃ��Ƃ���-1��Ԃ�
    }
    //private int JudgeSkillTrigger_3(List<Vector2Int> routeList) //�E�������1�~��2���s�l�ӌ`��`���Ƃ��A�Ō�̓_�Ŕ���
    //{
    //    for (int i = 0; i < routeList.Count - 4; i++)
    //    {
    //        if (routeList[i + 4] == routeList[i] &&
    //            (routeList[i + 1] - routeList[i] == routeList[i + 2] - routeList[i + 3] && routeList[i + 2] - routeList[i + 1] == routeList[i + 3] - routeList[i + 4]) && 
    //            (((routeList[i + 1] - routeList[i]).x == 0 && (routeList[i + 2] - routeList[i + 1]).x * (routeList[i + 2] - routeList[i + 1]).y < 0)) || 
    //            ((routeList[i + 2] - routeList[i + 1]).x == 0 && (routeList[i + 1] - routeList[i]).x * (routeList[i + 1] - routeList[i]).y < 0))   //��g�̑Εӂ����s�A���A�u��ӂ�y���ɕ��s�ŁA������ӂ͌X�������v
    //        {
    //            Debug.LogWarning("Skill_3�̔��������𖞂����Ă��܂�:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
    //            return i + 4;
    //        }
    //    }
    //    return -1;  //���������𖞂����Ȃ��Ƃ���-1��Ԃ�
    //}

    public void CauseSkill(int skillNum, CharacterMGR characterMGRInstace) //skillNum�ɑΉ�����skill�𔭓����� CharacterMGR��MoveAlongWith���ŌĂ�
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
                Debug.LogError("skillNum���s�K�؂ł��BskillNum=" + skillNum);
                break;

        }
    }

    //�ȉ��A��̓I��skill�̓��e������
    private IEnumerator Skill_0(CharacterMGR characterMGRInstace) //SPD��������
    {
        Debug.LogWarning("Skill_0�𔭓����܂��BName=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetSpd(characterMGRInstace.GetSpd() + spdIncrease);

        yield return new WaitForSeconds(durationOfSkill_0);

        if (characterMGRInstace == null) yield break; //GameObject��Destroy���ꂽ���Ȃǂ̓R���[�`����j������

        Debug.LogWarning("Skill_0���I�����܂��BName=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetSpd(characterMGRInstace.GetSpd() - spdIncrease);

    }
    private void Skill_1(CharacterMGR characterMGRInstace) //�c�莞�ԉ���
    {
        Debug.LogWarning("Skill_1�𔭓����܂��BName=" + characterMGRInstace.gameObject.name);
        GameManager.instance.timerMGR.Timer -= timerDecrease;
        return;
    }
    private IEnumerator Skill_2(CharacterMGR characterMGRInstace) //ATK��������
    {
        Debug.LogWarning("Skill_2�𔭓����܂��BName=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetAtk(characterMGRInstace.GetAtk() + atkIncrease);

        yield return new WaitForSeconds(durationOfSkill_2);

        if (characterMGRInstace == null) yield break; //GameObject��Destroy���ꂽ���Ȃǂ̓R���[�`����j������

        Debug.LogWarning("Skill_2���I�����܂��BName=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetAtk(characterMGRInstace.GetAtk() - atkIncrease);
    }
    //private void Skill_3()
    //{
    //    Debug.LogWarning("Skill_3�𔭓����܂�");
    //}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkillMGR : MonoBehaviour //�e�L�����N�^�[��Prefab��instance���Z�b�g����
{
    [SerializeField] private int skillNum;
    [SerializeField] private float duration; //�X�L���̌p������ SkillNum�ɍ��킹�ăX�L�����I�΂��
    [SerializeField] private int amount; //�X�L���ɂ��ω���
    [SerializeField] private int skillRemainingeTime; //���݂́A�X�L�����d�����Ĕ��������� �f�o�b�O�p��[SerializeField]���Ă���

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
                skillSentence = duration + "�b��SPD��" + amount + "�㏸";
                break;
            case 1:
                skillSentence = "�c�莞�Ԃ�" + amount + "�b����";
                break;
            case 2:
                skillSentence = duration + "�b��ATK��" + amount + "�㏸";
                break;
            default:
                Debug.LogError("skillNum���s�K�؂ł��BskillNum=" + skillNum);
                skillSentence = "skillNum���s�K�؂ł�";
                break;
        }
        return skillSentence;
    }
    public List<int> JudgeSkillTrigger(List<Vector2Int> routeList) //CharacterMGR���������ꂽ�Ƃ��ɌĂԁB�߂�l��CharacterMGR��causeSkillPoint�ɑ������
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
                Debug.LogError("skillNum���s�K�؂ł��BskillNum=" + skillNum);
                return null;

        }
    }

    //�ȉ��A�eskill�̋�̓I�Ȕ�������������
    private List<int> JudgeSkillTrigger_0(List<Vector2Int> routeList) //1�~1�̐����`��`���Ƃ��A�Ō�̓_�Ŕ���
    {
        List<int> causeSkillPoints = new List<int>();
        for (int i = 0; i < routeList.Count - 4; i++)
        {
            if (routeList[i + 4] == routeList[i] &&
                (routeList[i].x == routeList[i + 1].x && routeList[i + 1].y == routeList[i + 2].y && routeList[i + 2].x == routeList[i + 3].x && routeList[i + 3].y == routeList[i].y) ||
                (routeList[i].y == routeList[i + 1].y && routeList[i + 1].x == routeList[i + 2].x && routeList[i + 2].y == routeList[i + 3].y && routeList[i + 3].x == routeList[i].x))
            {
                Debug.LogWarning("Skill_0�̔��������𖞂����Ă��܂�:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
                causeSkillPoints.Add(i + 4);
                i += 4;
            }
        }
        return causeSkillPoints;  //���������𖞂����Ȃ��Ƃ��͋��List��Ԃ�
    }
    private List<int> JudgeSkillTrigger_1(List<Vector2Int> routeList) //��2�~��2�̐����`��`���Ƃ��A�Ō�̓_�Ŕ���
    {
        List<int> causeSkillPoints = new List<int>();
        for (int i = 0; i < routeList.Count - 4; i++)
        {
            if (routeList[i + 4] == routeList[i] &&
                (routeList[i].x == routeList[i + 2].x && routeList[i + 1].y == routeList[i + 3].y && Mathf.Abs(routeList[i].y - routeList[i + 2].y) == 2 && Mathf.Abs(routeList[i + 1].x - routeList[i + 3].x) == 2) ||
                (routeList[i].y == routeList[i + 2].y && routeList[i + 1].x == routeList[i + 3].x && Mathf.Abs(routeList[i].x - routeList[i + 2].x) == 2 && Mathf.Abs(routeList[i + 1].y - routeList[i + 3].y) == 2))
            {
                Debug.LogWarning("Skill_1�̔��������𖞂����Ă��܂�:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4]);
                causeSkillPoints.Add(i + 4);
                i += 4;
            }
        }
        return causeSkillPoints;  //���������𖞂����Ȃ��Ƃ��͋��Listt��Ԃ�
    }
    private List<int> JudgeSkillTrigger_2(List<Vector2Int> routeList) //2�~2�̐����`��`���Ƃ��A�Ō�̓_�Ŕ���
    {
        List<int> causeSkillPoints = new List<int>();
        for (int i = 0; i < routeList.Count - 8; i++)
        {
            if (routeList[i + 8] == routeList[i] &&
                (routeList[i].x == routeList[i + 2].x && routeList[i + 2].y == routeList[i + 4].y && routeList[i + 4].x == routeList[i + 6].x && routeList[i + 6].y == routeList[i].y) ||
                (routeList[i].y == routeList[i + 2].y && routeList[i + 2].x == routeList[i + 4].x && routeList[i + 4].y == routeList[i + 6].y && routeList[i + 6].x == routeList[i].x))
            {
                Debug.LogWarning("Skill_2�̔��������𖞂����Ă��܂�:" + routeList[i] + routeList[i + 1] + routeList[i + 2] + routeList[i + 3] + routeList[i + 4] + routeList[i + 5] + routeList[i + 6] + routeList[i + 7] + routeList[i + 8]);
                causeSkillPoints.Add(i + 8);
                i += 8;
            }
        }
        return causeSkillPoints;  //���������𖞂����Ȃ��Ƃ���-1��Ԃ�
    }


    public void CauseSkill(CharacterMGR characterMGRInstace, ParticleSystem particleSystem , Material skillAuraMaterial) //skillNum�ɑΉ�����skill�𔭓����� CharacterMGR��MoveAlongWith���ŌĂ�
    {
        if (characterMGRInstace == null)
        {
            Debug.LogError($"characterMGRInstace��null�ł�");
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
                Debug.LogError("skillNum���s�K�؂ł��BskillNum=" + skillNum);
                break;

        }
    }

    //�ȉ��A��̓I��skill�̓��e������
    private IEnumerator Skill_0(CharacterMGR characterMGRInstace, ParticleSystem particleSystem, Material skillAuraMaterial) //SPD��������
    {
        PlaySkillIconParticle(particleSystem);
        skillRemainingeTime++;
        if (skillRemainingeTime != 1)
        {
            Debug.LogWarning("skill_0�͔������ł��BskillRemainingTime=" + skillRemainingeTime);
            yield break;  //���ɂ��̃X�L����Coroutine�������Ă���̂ŁA����Coroutine�͏I���
        }

        Debug.LogWarning("Skill_0�𔭓����܂��BName=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetSpd(characterMGRInstace.GetSpd() + amount);
        PlaySkillAuraParticle(particleSystem, skillAuraMaterial);

        while (skillRemainingeTime != 0)
        {
            if (characterMGRInstace == null)
            {
                skillRemainingeTime = 0; yield break; //GameObject��Destroy���ꂽ���Ȃǂ̓R���[�`����j������
            }

            yield return new WaitForSeconds(duration);
            skillRemainingeTime--;
            Debug.LogWarning("Skill_0�̔�������duration���o�߂��܂����BskillRemainingTime=" + skillRemainingeTime);
        }

        if (characterMGRInstace == null) yield break; //GameObject��Destroy���ꂽ���Ȃǂ̓R���[�`����j������

        Debug.LogWarning("Skill_0���I�����܂��BName=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetSpd(characterMGRInstace.GetSpd() - amount);
        StopSkillAuraParticle(particleSystem, skillAuraMaterial);
    }
    private IEnumerator Skill_1(CharacterMGR characterMGRInstace, ParticleSystem particleSystem, Material skillAuraMaterial) //ATK��������
    {
        PlaySkillIconParticle(particleSystem);
        skillRemainingeTime++;
        if (skillRemainingeTime != 1)
        {
            Debug.LogWarning("skill_1�͔������ł��BskillRemainingTime=" + skillRemainingeTime);
            yield break;  //���ɂ��̃X�L����Coroutine�������Ă���̂ŁA����Coroutine�͏I���
        }

        Debug.LogWarning("Skill_1�𔭓����܂��BName=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetAtk(characterMGRInstace.GetAtk() + amount);
        PlaySkillAuraParticle(particleSystem, skillAuraMaterial);

        while (skillRemainingeTime != 0)
        {
            if (characterMGRInstace == null)
            {
                skillRemainingeTime = 0; yield break; //GameObject��Destroy���ꂽ���Ȃǂ̓R���[�`����j������
            }

            yield return new WaitForSeconds(duration);
            skillRemainingeTime--;
            Debug.LogWarning("Skill_1�̔�������duration���o�߂��܂����BskillRemainingTime=" + skillRemainingeTime);
        }

        if (characterMGRInstace == null) yield break; //GameObject��Destroy���ꂽ���Ȃǂ̓R���[�`����j������

        Debug.LogWarning("Skill_1���I�����܂��BName=" + characterMGRInstace.gameObject.name);
        characterMGRInstace.SetAtk(characterMGRInstace.GetAtk() - amount);
        StopSkillAuraParticle(particleSystem, skillAuraMaterial);
    }
    private IEnumerator Skill_2(CharacterMGR characterMGRInstace, ParticleSystem particleSystem, Material skillAuraMaterial) //�c�莞�ԉ���
    {
        PlaySkillIconParticle(particleSystem);
        skillRemainingeTime++;
        if (skillRemainingeTime != 1)
        {
            Debug.LogWarning("skill_2�͔������ł��BskillRemainingTime=" + skillRemainingeTime);
            yield break;  //���ɂ��̃X�L����Coroutine�������Ă���̂ŁA����Coroutine�͏I���
        }

        Debug.LogWarning("Skill_2�𔭓����܂����BName=" + characterMGRInstace.gameObject.name);
        GameManager.instance.timerMGR.Timer -= amount;
        PlaySkillAuraParticle(particleSystem, skillAuraMaterial);

        while (skillRemainingeTime != 0)
        {
            if (characterMGRInstace == null)
            {
                skillRemainingeTime = 0; yield break; //GameObject��Destroy���ꂽ���Ȃǂ̓R���[�`����j������
            }

            yield return new WaitForSeconds(duration);
            skillRemainingeTime--;
            Debug.LogWarning("Skill_2�̔�������duration���o�߂��܂����BskillRemainingTime=" + skillRemainingeTime);
        }

        yield return new WaitForSeconds(duration);
        StopSkillAuraParticle(particleSystem, skillAuraMaterial);
    }

    private void PlaySkillAuraParticle(ParticleSystem particleSystem, Material skillAuraMaterial)
    {
        //particleSystem.GetComponent<ParticleSystemRenderer>().material = GameManager.instance.characterSkillsDataMGR.skillIconMaterials[skillNum]; //icon�p�[�e�B�N���p�̉摜��skillNum�ɉ����ăZ�b�g����
        Debug.LogWarning("Skill��Particle���Đ����܂�");
        //particleSystem.Play();

        //�X�L���������̃I�[����t����
        skillAuraMaterial.SetInt("_isActivatingSkill", 1); //SetBool�͂Ȃ����Ƃɒ���
        //characterMGRInstance.GetComponent<Renderer>().material.SetInt("Boolean_7659173a6b1843508b1713d3c0bdd4d2", 1);

    }

    private void StopSkillAuraParticle(ParticleSystem particleSystem,Material skillAuraMaterial)
    {
        if (particleSystem == null) return;

        Debug.LogWarning("Skill��Particle���~���܂�");
        //particleSystem.Stop();

        //�X�L���������̃I�[����t����
        skillAuraMaterial.SetInt("_isActivatingSkill", 0);
    }
    private void PlaySkillIconParticle(ParticleSystem particleSystem)
    {
        particleSystem.GetComponent<ParticleSystemRenderer>().material = GameManager.instance.characterSkillsDataMGR.skillIconMaterials[skillNum]; //icon�p�[�e�B�N���p�̉摜��skillNum�ɉ����ăZ�b�g����
        particleSystem.Play();

    }



}

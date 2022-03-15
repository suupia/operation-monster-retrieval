using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballMGR : MonoBehaviour
{

    public void FiringCannonball(Vector2Int towerGridPos,float timeToImpact,CharacterMGR targetCharacter)
    {
        StartCoroutine(FiringCannonballCoroutine(towerGridPos,timeToImpact,targetCharacter));
    }

    IEnumerator FiringCannonballCoroutine(Vector2Int towerGridPos,float timeToImpact, CharacterMGR targetCharacter)
    {
        Vector2 startPos = GameManager.instance.ToWorldPosition(towerGridPos);
        Vector2 endPos = targetCharacter.GetTransformPos(); //FiringCannonball���Ăяo������Ƀ_���[�W��^���邽�߁A���Ȃ��Ƃ��ŏ��̈��͊m����endPos���X�V�ł���

        float remainingDistance = (endPos - startPos).sqrMagnitude;


        ////�C�e���ړ�
        //while (remainingDistance > float.Epsilon)
        //{
        //    if (targetCharacter != null) //�L�����N�^�[����Ɏc���Ă���Ƃ��̂�endPos�̍X�V���s��
        //    {
        //        endPos = targetCharacter.GetTransformPos();
        //    }
        //    else
        //    {
        //        //endPos�̍X�V�͍s��Ȃ��i�O�̃t���[����endPos�����̂܂ܗ��p����j
        //    }

        //    transform.position = Vector3.MoveTowards(transform.position, endPos, 1f / timeToImpact * Time.deltaTime * GameManager.instance.gameSpeed);
        //    //3�ڂ̈�����"1�t���[���̍ő�ړ�����"�@�P�ʂ͎���[m/s](�R���[�`����1�t���[��������Ă��邩��Time.deltaTime��������BmoveTime�o��������1�}�X�i�ށB)

        //    remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;


        //    while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //�|�[�Y���͎~�߂�


        //    yield return null;  //1�t���[����~������B
        //}
        //transform.position = endPos;//���[�v�𔲂������͂�������ړ�������B


        //�C�e���ړ��i�V�j
        for (float timer=0;timer < timeToImpact; timer+= Time.deltaTime * GameManager.instance.gameSpeed)
        {
            if (targetCharacter != null) //�L�����N�^�[����Ɏc���Ă���Ƃ��̂�endPos�̍X�V���s��
            {
                endPos = targetCharacter.GetTransformPos();
            }
            else
            {
                //endPos�̍X�V�͍s��Ȃ��i�O�̃t���[����endPos�����̂܂ܗ��p����j
            }
            transform.position = startPos + (endPos - startPos) * (timer / timeToImpact);

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //�|�[�Y���͎~�߂�

            yield return null;  //1�t���[����~������B
        }
        transform.position = endPos;//���[�v�𔲂������͂�������ړ�������B


        //�C�e���폜
        //Debug.Log("�C�ۂ��폜���܂�");
        Destroy(this.gameObject);
    }


}

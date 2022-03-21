using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillIconMotionMGR : MonoBehaviour
{
    [SerializeField] int swingTime;
    [SerializeField] int moveToCastleTime;
    public void StartSwingCoroutine(Transform characterTransform)
    {
        if (characterTransform == null) return;

        StartCoroutine(SwingCoroutine(characterTransform));
    }
    IEnumerator SwingCoroutine(Transform characterTransform)
    {
        if (characterTransform == null) yield break;

        float delta = transform.position.y - characterTransform.position.y;
        for(int i = 0; i < swingTime; i++)
        {
            if (characterTransform == null) yield break;
            delta += 0.01f;
            transform.position = characterTransform.position + Vector3.up * delta;
            yield return new WaitForSeconds(0.01f);
        }

        delta -= 0.01f * swingTime;
        for (int i = 0; i < swingTime; i++)
        {
            if (characterTransform == null) yield break;

            delta += 0.01f;
            transform.position = characterTransform.position + Vector3.up * delta;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }

    public void StartMoveToCastleCoroutine(Transform characterTransform)
    {
        StartCoroutine(MoveToCastleCoroutine(characterTransform));
    }
    IEnumerator MoveToCastleCoroutine(Transform characterTransform)
    {
        Vector3 moveVec = (Vector3)GameManager.instance.ToWorldPosition(GameManager.instance.mapMGR.GetAllysCastlePos()) - transform.position; //現在の位置から城までのベクトル

        int l = 5;
        for(int i = 1; i <= moveToCastleTime; i++)
        {
            transform.position += (i / l) * (i / l) * (moveVec / (moveToCastleTime * (moveToCastleTime + 1) * (2 * moveToCastleTime + 1) / (6 * l * l)));
            yield return new WaitForSeconds(0.01f);
        }

        //以下、城についた時の処理
        Destroy(gameObject);
    }
}

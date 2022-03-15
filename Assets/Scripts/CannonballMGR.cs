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
        Vector2 endPos = targetCharacter.GetTransformPos(); //FiringCannonballを呼び出した後にダメージを与えるため、少なくとも最初の一回は確実にendPosを更新できる

        float remainingDistance = (endPos - startPos).sqrMagnitude;


        ////砲弾を移動
        //while (remainingDistance > float.Epsilon)
        //{
        //    if (targetCharacter != null) //キャラクターが場に残っているときのみendPosの更新を行う
        //    {
        //        endPos = targetCharacter.GetTransformPos();
        //    }
        //    else
        //    {
        //        //endPosの更新は行わない（前のフレームのendPosをそのまま利用する）
        //    }

        //    transform.position = Vector3.MoveTowards(transform.position, endPos, 1f / timeToImpact * Time.deltaTime * GameManager.instance.gameSpeed);
        //    //3つ目の引数は"1フレームの最大移動距離"　単位は実質[m/s](コルーチンが1フレームずつ回っているからTime.deltaTimeが消える。moveTime経った時に1マス進む。)

        //    remainingDistance = (endPos - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;


        //    while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める


        //    yield return null;  //1フレーム停止させる。
        //}
        //transform.position = endPos;//ループを抜けた時はきっちり移動させる。


        //砲弾を移動（新）
        for (float timer=0;timer < timeToImpact; timer+= Time.deltaTime * GameManager.instance.gameSpeed)
        {
            if (targetCharacter != null) //キャラクターが場に残っているときのみendPosの更新を行う
            {
                endPos = targetCharacter.GetTransformPos();
            }
            else
            {
                //endPosの更新は行わない（前のフレームのendPosをそのまま利用する）
            }
            transform.position = startPos + (endPos - startPos) * (timer / timeToImpact);

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める

            yield return null;  //1フレーム停止させる。
        }
        transform.position = endPos;//ループを抜けた時はきっちり移動させる。


        //砲弾を削除
        //Debug.Log("砲丸を削除します");
        Destroy(this.gameObject);
    }


}

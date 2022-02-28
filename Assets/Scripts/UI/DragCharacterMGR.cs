using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragCharacterMGR : MonoBehaviour
{
    [SerializeField] int dragNum; //CharacterInReverse(GameObject)の添え字をインスペクター上でセットしておく

    private void Start()
    {
        StartCoroutine(LateStart(0.5f)); //characterDatabaseがGameManagerのStartで初期化されるため、若干遅れてからサムネイルの初期化をする
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (dragNum < GameManager.instance.characterPrefabs.Length) //モンスターの種類だけサムネイル画像を取得する
        {
            this.gameObject.GetComponent<Image>().sprite = GameManager.instance.GetCharacterDatabase(dragNum).GetThumbnailSprite();
        }

    }
    public void DragCharacter() //EventTriggerで呼ぶ
    {
        //Debug.Log($"{dragNum}がドラッグされました");

        GameManager.instance.DragCharacterData(dragNum);

    }

    public void PointerDownCharacterInReserve() //EventTriggerで呼ぶ（２つセットしていることに注意）
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInReserve(dragNum);
    }

    public void PointerDown() //EventTriggerで呼ぶ　DraggedCharacterThumbnail用
    {
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().ShowDraggedCharacterThumbnail(dragNum); //dragNumの場合はそのままIDになる

    }
    public void PointerUp() //EventTriggerで呼ぶ　DraggedCharacterThumbnail用
    {
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNumの場合はそのままIDになる
    }

}

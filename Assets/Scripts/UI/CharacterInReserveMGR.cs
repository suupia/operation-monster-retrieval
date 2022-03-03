using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterInReserveMGR : MonoBehaviour
{
    [SerializeField] int dragNum; //CharacterInReverse(GameObject)の添え字をインスペクター上でセットしておく
    [SerializeField] Image characterInReserveImage; //インスペクター上でセットする

    private void Start()
    {
        if (GameManager.instance.InitializationFlag == false) GameManager.instance.Initialization();

        if (dragNum < GameManager.instance.characterPrefabs.Length) //モンスターの種類だけサムネイル画像を取得する
        {
            characterInReserveImage.sprite = GameManager.instance.GetCharacterDatabase(dragNum).GetThumbnailSprite();
        }
    }


    public void UpdateCharacterInReserve() //サムネイルの色を決める
    {
        //Debug.Log($"UpdateCharacterInReserveを実行します\ndragNum:{dragNum}");
        if (dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum] && dragNum <GameManager.instance.characterInReserveMGRs.Length /*今の実装ではdragNumは9まで(0～8)*/)
        {

            characterInReserveImage.color = Color.white;
        }
        else
        {
            characterInReserveImage.color = new Color(0.04f, 0.04f, 0.04f, 1);//黒に近い色にする
        }
    }

    public void DragCharacter() //EventTriggerで呼ぶ
    {
        //Debug.Log($"{dragNum}がドラッグされました");

        if (!(dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }

        GameManager.instance.DragCharacterData(dragNum);

    }

    public void PointerDownCharacterInReserve() //EventTriggerで呼ぶ（２つセットしていることに注意）
    {
        if (!(dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInReserve(dragNum);
    }

    public void PointerDown() //EventTriggerで呼ぶ　DraggedCharacterThumbnail用
    {
        if (!(dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().ShowDraggedCharacterThumbnail(dragNum); //dragNumの場合はそのままIDになる

    }
    public void PointerUp() //EventTriggerで呼ぶ　DraggedCharacterThumbnail用
    {
        if (!(dragNum < GameManager.instance.CharacterIDsThatCanBeUsed[GameManager.instance.StagesClearedNum]))
        {
            return;
        }
        GameManager.instance.inputMGR.GetDraggedCharacterThumbnail().HideDraggedCharacterThumbnail(); //dragNumの場合はそのままIDになる
    }

}

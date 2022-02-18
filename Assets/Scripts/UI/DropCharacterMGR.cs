using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropCharacterMGR : MonoBehaviour
{
    [SerializeField] int dropNum;
    Image thumbnailImage;

    private void Start()
    {
        thumbnailImage = GetComponent<Image>();
    }
    public void DropCharacter() //Event Triggerで呼ぶ
    {
        Debug.LogWarning($"{dropNum}にドロップされました");

        GameManager.instance.DropCharacterData(dropNum);

        thumbnailImage.sprite = GameManager.instance.GetCharacterDatabase(GameManager.instance.IDsOfCharactersInCombat[dropNum]).GetThumbnailSprite();
    }

    public void PointerDownCharacterInCombat()
    {
        GameManager.instance.statusCanvasMGR.UpdateStatusCanvasInCombat(dropNum);
    }
}

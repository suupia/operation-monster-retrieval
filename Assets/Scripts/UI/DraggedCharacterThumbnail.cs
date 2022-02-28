using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggedCharacterThumbnail : MonoBehaviour
{
    //UpdateDraggedCharacterThumbnail—p
    [SerializeField] Canvas menuCanvas;
    [SerializeField] RectTransform menuCanvasRT; //menuCanvas‚ÌRectTransform
    [SerializeField] GameObject draggedCharacterThumnail; //SetActive—p
    [SerializeField] RectTransform draggedCharacterThumnailRT; //characterInReserve‚ÌRectTransform
    [SerializeField] Image draggedCharacterThumnailImage;

    bool isShowing;
    int showingCharacterID;

    private void Update()
    {
        if (isShowing) UpdateDraggedCharacterThumbnail();
    }
    public void UpdateDraggedCharacterThumbnail()
    {
        Vector2 mousePos = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            menuCanvasRT,
            Input.mousePosition,
            menuCanvas.worldCamera,
            out mousePos
            );
        draggedCharacterThumnailRT.localPosition = mousePos;

        draggedCharacterThumnailImage.sprite = GameManager.instance.GetCharacterDatabase(showingCharacterID).GetThumbnailSprite();

        draggedCharacterThumnailImage.enabled = true;

    }

    public void ShowDraggedCharacterThumbnail(int characterID)
    {
        showingCharacterID = characterID;
        isShowing = true;
        draggedCharacterThumnailImage.enabled = true;
    }
    public void HideDraggedCharacterThumbnail()
    {
        isShowing = false;
        draggedCharacterThumnailImage.enabled = false;
    }
}

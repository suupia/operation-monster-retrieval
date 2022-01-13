using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCharacterMGR : MonoBehaviour
{
    [SerializeField] int dropNum;

    public void DropCharacter() //Event Triggerで呼ぶ
    {
        Debug.LogWarning($"{dropNum}にドロップされました");

        GameManager.instance.DropCharacterData(dropNum);

    }
}

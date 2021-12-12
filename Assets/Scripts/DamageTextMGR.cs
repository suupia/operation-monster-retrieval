using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextMGR : MonoBehaviour
{
    Rigidbody2D rd2D;

    [SerializeField] float impalseValue;

    private void Start()
    {
        rd2D = GetComponent<Rigidbody2D>();

        rd2D.AddForce(Vector3.up * impalseValue);
        Debug.LogWarning($"AddForce({Vector3.up}*{impalseValue})Çé¿çsÇµÇ‹ÇµÇΩ");
    }
}

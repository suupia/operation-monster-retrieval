using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextMGR : MonoBehaviour
{
    Rigidbody2D rd2D;

    float timer;

    [SerializeField] float impalseValue;
    [SerializeField] float timeToDisplay;

    private void Start()
    {
        rd2D = GetComponent<Rigidbody2D>();

        rd2D.AddForce(Vector3.up * impalseValue);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeToDisplay) Destroy(this.gameObject);
    }
}

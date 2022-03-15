using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextMGR : MonoBehaviour
{
    Rigidbody2D rd2D;

    float timer =0;

    [SerializeField] float impalseValue;
    [SerializeField] float timeToDisplay;

    private void Start()
    {
        rd2D = GetComponent<Rigidbody2D>();

        rd2D.AddForce(Vector3.up * impalseValue);
    }

    private void Update()
    {
        timer += Time.deltaTime * GameManager.instance.gameSpeed;

        if (timer > timeToDisplay * GameManager.instance.gameSpeed) Destroy(this.gameObject);
    }

    //public void ShowDamage(float timeToImpact)
    //{
    //    Debug.LogWarning("ShowDamageCoroutineを実行します");
    //    ShowDamageCoroutine(timeToImpact);
    //}
    //IEnumerator ShowDamageCoroutine(float timeToImpact)
    //{
    //   yield return new WaitForSeconds(timeToImpact);

    //    //力を与えて上向きに動かす
    //    rd2D.bodyType = RigidbodyType2D.Dynamic;
    //    rd2D = GetComponent<Rigidbody2D>();
    //    rd2D.AddForce(Vector3.up * impalseValue);

    //    while (timer <= timeToDisplay)
    //    {
    //        timer += Time.deltaTime * GameManager.instance.gameSpeed;
    //        Debug.LogWarning($"timer:{timer}");

    //        yield return null;  //1フレーム停止させる。
    //    }

    //    Destroy(this.gameObject);
    //}
}

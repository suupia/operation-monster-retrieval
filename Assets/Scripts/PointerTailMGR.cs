using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTailMGR : MonoBehaviour
{
    private int pointerTailIndex;
    // Start is called before the first frame update
    void Start()
    {
        pointerTailIndex = GameManager.instance.pointerMGR.GetPoinerTails().IndexOf(gameObject);
    }

    private void Update()
    {
        if(pointerTailIndex < GameManager.instance.pointerMGR.GetPoinerTails().Count - 3)
        {
            return;
        }
        RotatePointerTail();
    }
    public void RotatePointerTail()
    {
        if (GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].x == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].x)
        {
            if(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].y < GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].y)
            {
                transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
            }
            else
            {
                transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);

            }
        }
        else if(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].y == GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].y)
        {
            if(GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex].x < GameManager.instance.pointerMGR.GetManualRoute()[pointerTailIndex + 1].x)
            {
                transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
            }
            else
            {
                transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
            }
        }
    }

}

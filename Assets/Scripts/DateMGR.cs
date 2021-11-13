using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateMGR : MonoBehaviour
{
    public static DateMGR instance;

    public readonly int wallID;
    public readonly int groundID;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public DateMGR()
    {
        wallID = 2;
        groundID = 1;
    }
}

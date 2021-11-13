using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public DateMGR dateMGR;
    public MapMGR mapMGR;

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

    void Start()
    {
        dateMGR = GameObject.Find("DateMGR").GetComponent<DateMGR>();
        mapMGR = GameObject.Find("Tilemap").GetComponent<MapMGR>();

        mapMGR.SetupMap();
       
    }


}

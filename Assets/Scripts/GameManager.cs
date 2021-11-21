using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.NonSerialized] public MapMGR mapMGR;
    [System.NonSerialized] public DebugMGR debugMGR;

    public readonly int wallID = 3;
    public readonly int groundID = 2;
    public readonly int towerID = 5;

    public GameObject[] characterPrefabs;

    bool isSpawnCharacter=false;

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
        mapMGR = GameObject.Find("Tilemap").GetComponent<MapMGR>();
        debugMGR = GameObject.Find("DebugMGR").GetComponent<DebugMGR>();

        mapMGR.SetupMap();
       
    }

    private void Update()
    {
        SpawnCharacter();   
    }

    private void SpawnCharacter()
    {
        if (isSpawnCharacter)
        {
            return;
        }
        for (int i = 0; i< mapMGR.characterSpawnPoss.Length;i++)
        {
        if (mapMGR.GetMapValue(mapMGR.characterSpawnPoss[i])== GameManager.instance.groundID)
        {
                Debug.Log("SendCharacterCoroutineを実行します");
                StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss[i],0)); //とりあえずcharacterIDの引数は0にしておく
        }

        }
    }

    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector,int chracterID)
    {
        isSpawnCharacter = true;
        Instantiate(characterPrefabs[chracterID],new Vector3(vector.x+0.5f,vector.y+0.5f,0),Quaternion.identity);
        yield return new WaitForSeconds(3f);
        isSpawnCharacter = false;
    }

}

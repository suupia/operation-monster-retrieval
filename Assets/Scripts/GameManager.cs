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
    public readonly int characterID = 11;

    public GameObject[] characterPrefabs;

    bool isSpawnCharacter=false;

    //座標変換
    public Vector2Int ToGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    }
    public Vector2 ToWorldPosition(int x, int y)
    {
        return new Vector2(x + 0.5f, y + 0.5f);

    }
    public Vector2 ToWorldPosition(Vector2Int gridPosition)
    {
        return ToWorldPosition(gridPosition.x, gridPosition.y);
    }

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
        if (mapMGR.GetMapValue(mapMGR.characterSpawnPoss[i])% GameManager.instance.groundID ==0)
        {
                Debug.Log("SendCharacterCoroutineを実行します");
                StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss[i],0)); //とりあえずcharacterIDの引数は0にしておく
        }

        }
    }

    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector,int characterTypeNum)
    {
        isSpawnCharacter = true;
        Instantiate(characterPrefabs[characterTypeNum],new Vector3(vector.x+0.5f,vector.y+0.5f,0),Quaternion.identity);
        mapMGR.MultiplySetMapValue(vector,characterID);
        yield return new WaitForSeconds(200f); //とりあえずデバッグしやすいように長くしておく
        isSpawnCharacter = false;
    }

    public Vector2 RotateVector(Vector2 vector, float radian)
    {
        return Quaternion.Euler(0, 0, -radian) * vector;
    }

}

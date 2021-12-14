using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.NonSerialized] public MapMGR mapMGR;
    [System.NonSerialized] public InputMGR inputMGR;
    [System.NonSerialized] public DebugMGR debugMGR;
    [System.NonSerialized] public PointerMGR pointerMGR;

    [SerializeField] int numOfCharacterTypes = 4; //戦闘に参加するモンスターの種類は4種類
    [SerializeField] AutoRouteData autoRouteData; //インスペクター上でセットする
    [SerializeField] ManualRouteData manualRouteData; //インスペクター上でセットする
    public AutoRouteData[] autoRouteDatas;
    public ManualRouteData[] manualRouteDatas;


    public readonly int wallID = 3;
    public readonly int groundID = 2;
    public readonly int towerID = 5;
    public readonly int characterID = 11;

    public GameObject[] characterPrefabs; //配列にしているのは仮。実際にはデータベースから情報を読み取ってインスタンス化するからプレハブは一つでよい

    bool isSpawnCharacter = false;

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
        inputMGR = GameObject.Find("InputMGR").GetComponent<InputMGR>();
        debugMGR = GameObject.Find("DebugMGR").GetComponent<DebugMGR>();
        pointerMGR = GameObject.Find("Pointer").GetComponent<PointerMGR>();
        Debug.LogWarning($"pointerMGRを初期化しました。pointerMGR.transform.position:{pointerMGR.transform.position}");

        autoRouteDatas = new AutoRouteData[numOfCharacterTypes];  //とりあえず5体分のルートを用意しておく
        manualRouteDatas = new ManualRouteData[numOfCharacterTypes];

        for (int i=0;i<numOfCharacterTypes;i++)
        {
            autoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(),mapMGR.GetMapHeight());
            manualRouteDatas[i] = new ManualRouteData(); //今はmanualRouteDataがない
        }

        mapMGR.SetupMap();

    }

    public void SpawnCharacter(int CharacterTypeNum)
    {
        if (isSpawnCharacter)
        {
            return;
        }
        for (int i = 0; i < mapMGR.characterSpawnPoss.Length; i++)
        {
            if (mapMGR.GetMapValue(mapMGR.characterSpawnPoss[i]) % GameManager.instance.groundID == 0)
            {
                Debug.Log("SpawnCharacterCoroutineを実行します");
                StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss[i], CharacterTypeNum)); //とりあえずcharacterIDの引数は0にしておく
            }

        }
    }

    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector, int characterTypeNum)
    {
        isSpawnCharacter = true;

        GameObject characterGO; //スコープに注意　このメソッドの中でしか使えない
        CharacterMGR characterMGR;

        characterGO = Instantiate(characterPrefabs[characterTypeNum], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0), Quaternion.identity);
        characterMGR = characterGO.GetComponent<CharacterMGR>();

        //キャラクターのデータをここで渡す
        characterMGR.SetCharacterData(0); //今はCharacterTypeIDとして0を渡しておく。AutoRouteDataも[0]を参照するようになる。
        

        mapMGR.MultiplySetMapValue(vector, characterID);
        mapMGR.GetMap().SetCharacterMGR(vector,characterMGR);

        yield return new WaitForSeconds(1f); //クールタイムは適当

        isSpawnCharacter = false;
    }

    public int[,] CalcSearchRangeArray(int advancingDistance, int lookingForValue, int notLookingForValue, int centerValue) //マス目に置ける円形の索敵範囲を計算して、2次元配列で返す
    {
        int t = lookingForValue; //索敵範囲
        int f = notLookingForValue; //索敵範囲外
        int o = centerValue; //原点

        int size = 2 * (advancingDistance + 1) + 1;
        int[,] resultArray = new int[size, size];

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (i + j == advancingDistance || i + j == advancingDistance + 1 //左下
                   || i - j == advancingDistance + 1 || i - j == advancingDistance + 2 //右下
                   || -i + j == advancingDistance + 1 || -i + j == advancingDistance + 2 //左下
                   || i + j == 3 * (advancingDistance + 1) || i + j == 3 * (advancingDistance + 1) + 1 //右上
                    )
                {
                    resultArray[i, j] = t;
                }
                else if (i == advancingDistance + 1 && j == advancingDistance + 1)
                {
                    resultArray[i, j] = o;
                }
                else
                {
                    resultArray[i, j] = f;
                }
            }
        }
        return resultArray;
    }

    public bool CanAttackTarget(Vector2Int gridPos, int attackRange,int targetID,out Vector2Int targetPos) //最も近い攻撃対象がの座標を返す（存在しないときはVector2Int.zeroを返す）
    {
        int lookingForValue = 1; //索敵範囲の値
        int notLookingForValue = 0; //索敵範囲外の値
        int centerValue = 0; //原点の値

        Vector2Int vector; //ループ内で使い、(i,j)をワールド座標に直したもの
        List<Vector2Int> nearestTargetList = new List<Vector2Int>();

        int[,] searchRangeArray;
        int maxRange = attackRange;


        //索敵範囲内の攻撃対象の位置をListに追加する
        for (int k = 0; k < maxRange; k++) //kは中心のマスから何マスまで歩けるかを表す
        {
            //Debug.Log($"{k}回目のループを開始します");

            searchRangeArray = CalcSearchRangeArray(k, lookingForValue, notLookingForValue, centerValue);
            for (int j = 0; j < searchRangeArray.GetLength(0); j++)
            {
                for (int i = 0; i < searchRangeArray.GetLength(1); i++)
                {
                    vector = new Vector2Int(gridPos.x - (k + 1) + i, gridPos.y - (k + 1) + j); //ワールド座標に変換する

                    if (vector.x < 0 || vector.y < 0 || vector.x > GameManager.instance.mapMGR.GetMapWidth() || vector.y > GameManager.instance.mapMGR.GetMapHeight())
                    {
                        continue;
                    }

                    if (searchRangeArray[i, j] == lookingForValue && GameManager.instance.mapMGR.GetMapValue(vector) % targetID == 0)
                    {
                        nearestTargetList.Add(vector);
                    }
                }
            }

            if (nearestTargetList.Count > 0)
            {
                Debug.Log($"nearestTargetList[0]={nearestTargetList[0]}");
                break;
            }
        }


        // 2  1
        // 4  3 と優先順位をつける（攻撃範囲内に存在するかを判定するだけならいらないが、ログを出した時に混乱しないように優先順位をつけておく）
        //targetが同じ距離にあるときに優先順位をつけて検索するために Listの中身をソートする
        nearestTargetList.Sort((a, b) => b.y - a.y); //まずy座標に関して降順でソートする
        nearestTargetList.Sort((a, b) => b.x - a.x); //次にx座標に関して降順でソートする

        if (nearestTargetList.Count > 0)
        {
            targetPos = nearestTargetList[0];
            Debug.Log("nearestTargetList:" + string.Join(",", nearestTargetList) + $"\ntargetPos:{targetPos}");
            return true;
        }
        else
        {
            targetPos = Vector2Int.zero; //nullの代わり
            Debug.Log("nearestTargetList:" + string.Join(",", nearestTargetList) + $"\ntargetPos:{targetPos}");
            return false;
        }
    }

    public int CalcDamage(int atk)
    {
        return atk; //とりあえず今は攻撃力をそのまま返すだけ
    }



}

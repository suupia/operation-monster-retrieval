using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] public MapMGR mapMGR; //インスペクター上でセットする
    [SerializeField] public InputMGR inputMGR;
    [SerializeField] public DebugMGR debugMGR;
    [SerializeField] public PointerMGR pointerMGR;
    [SerializeField] public TimerMGR timerMGR;
    [SerializeField] public EnergyMGR energyMGR;

    [SerializeField] GameObject selectStageCanvas; //SetActiveで表示を制御するのでゲームオブジェクトごと取得する必要がある インスペクター上でセットする

    [SerializeField] GameObject resultTextGO; //SetActiveで表示を制御するのでゲームオブジェクトごと取得する必要がある インスペクター上でセットする
    Text resultText;


    int numOfCharacterInCombat = 4; //戦闘に参加するモンスターの種類は4種類
    int[] IDsOfCharactersInCombat; //戦闘に酸化しているモンスターのID (numOfCharacterTypesの分だけ要素を用意する)
    [SerializeField] AutoRouteData autoRouteData; //インスペクター上でセットする
    [SerializeField] ManualRouteData manualRouteData; //インスペクター上でセットする
    public AutoRouteData[] autoRouteDatas;
    public ManualRouteData[] manualRouteDatas;


    public readonly int wallID = 3;
    public readonly int groundID = 2;
    public readonly int facilityID = 5;
    public readonly int characterID = 11;

    public GameObject[] characterPrefabs; //配列にしているのは仮。実際にはデータベースから情報を読み取ってインスタンス化するからプレハブは一つでよい

    int characterCounter =0; //キャラクターが何体スポーンしたかを数える
    float characterDisplacement= 0.03f; //キャラクターがスポーンしたときにどれくらいズレるかを決める(10回で一周するようにする)

    public CharacterMGR.Mode[] characterMode; //キャラクターの種類ごとの操作モードを格納する

    bool isSpawnCharacter = false;

    [SerializeField] State _state; //デバッグしやすいようにSerializeFieldにしておく
    public State state
    {
        get { return _state; }
        set
        {

            _state = value;
        }
    } //プロパティ
    public enum State
    {
        SelectingStage,
        PlayingGame,
        ShowingResults
    }


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

    //Setter
    public void SetCharacterMode(int characterTypeNum,CharacterMGR.Mode mode)
    {
        if(characterTypeNum<0 || this.numOfCharacterInCombat < characterTypeNum)
        {
            Debug.LogError($"SetCharacterModeの引数characterTypeNumが{characterTypeNum}になっています");
            return;
        }
        characterMode[characterTypeNum] = mode;
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
        resultText = resultTextGO.GetComponent<Text>();

        IDsOfCharactersInCombat = new int[numOfCharacterInCombat];

        autoRouteDatas = new AutoRouteData[numOfCharacterInCombat];
        manualRouteDatas = new ManualRouteData[numOfCharacterInCombat];

        characterMode = new CharacterMGR.Mode[numOfCharacterInCombat];
        for(int i = 0; i<characterMode.Length; i++)
        {
            characterMode[i] = CharacterMGR.Mode.Auto; //デフォルトはAutoMode
        }

        for (int i=0;i<numOfCharacterInCombat;i++)
        {
            autoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(),mapMGR.GetMapHeight());
            manualRouteDatas[i] = new ManualRouteData(); //今はmanualRouteDataがない
        }

        StartSelectingStage();


    }
    

    public void StartSelectingStage()
    {
        state = State.SelectingStage;
        selectStageCanvas.SetActive(true);
        //次のPlayingGameに備えて前の戦闘のデータをここでリセットする（今は特に書くことはない）
    }
    public  void StartPlayingGame()
    {
        state = State.PlayingGame;
        selectStageCanvas.SetActive(false);

        timerMGR.InitiTimer();

        SetCharacterTypeIDInCombat();

        mapMGR.SetupMap();

    }
    public void StartShowingResults( bool isWin)
    {
        state = State.ShowingResults;

        resultTextGO.SetActive(true);

        if (isWin)
        {
            resultText.text = "勝利";
        }
        else
        {
            resultText.text = "敗北";
        }
    }
    public void SpawnCharacter(int buttonNum)
    {
        if (isSpawnCharacter)
        {
            return;
        }
        for (int i = 0; i < mapMGR.characterSpawnPoss.Length; i++) //characterSpawnPossが1つしかないので、forループに意味はない
        {

            if (mapMGR.GetMapValue(mapMGR.characterSpawnPoss[i]) % GameManager.instance.groundID != 0)
            {
                Debug.Log($"{mapMGR.characterSpawnPoss[i]}のmapValueにgroundIDが含まれないため、モンスターをスポーンできません。");
                return;
            }
            if (energyMGR.CurrentEnergy < CSVLoader.monsterDataList[IDsOfCharactersInCombat[buttonNum]].Cost)
            {
                Debug.Log($"Energyが足りないためモンスターをスポーンできません。（energyMGR.CurrentEnergy :{energyMGR.CurrentEnergy},Cost:{CSVLoader.monsterDataList[IDsOfCharactersInCombat[buttonNum]].Cost}）");
                return;
            }

            Debug.Log("SpawnCharacterCoroutineを実行します");
            StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss[i], buttonNum));

        }
    }

    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector, int buttonNum)
    {
        isSpawnCharacter = true;

        int characterTypeID = IDsOfCharactersInCombat[buttonNum];

        Vector3 displacement = new Vector3(characterDisplacement * (characterCounter%7)-3*characterDisplacement, 0.5f*(characterDisplacement * (characterCounter % 7) - 3 * characterDisplacement), 0); //キャラクターを少しずらす y方向のズレはx方向のズレの0.5倍
        Debug.LogWarning($"displacement:{displacement}");

        GameObject characterGO = Instantiate(characterPrefabs[characterTypeID], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0) + displacement, Quaternion.identity);
        CharacterMGR characterMGR = characterGO.GetComponent<CharacterMGR>();

        //スポーンでCostを消費する
        energyMGR.CurrentEnergy -= CSVLoader.monsterDataList[characterTypeID].Cost;

        //キャラクターのデータをここで渡す
        characterMGR.SetCharacterData(buttonNum,characterTypeID);

        mapMGR.MultiplySetMapValue(vector, characterID);
        mapMGR.GetMap().AddCharacterMGR(vector,characterMGR);

        characterCounter++;

        //キャラクターのモードを決める
        characterMGR.SetMode(characterMode[buttonNum]);

        yield return new WaitForSeconds(1f); //クールタイムは適当

        isSpawnCharacter = false;
    }
    int ToCharacterTypeID(int buttonNum)
    {
        return buttonNum; //今のところはそのまま返す
    }

    void SetCharacterTypeIDInCombat()
    {
        //とりあえず適当に出撃するモンスターを決めておく
        for(int i = 0;i<numOfCharacterInCombat; i++)
        {
            IDsOfCharactersInCombat[i] = i;

        }
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
            //Debug.Log("nearestTargetList:" + string.Join(",", nearestTargetList) + $"\ntargetPos:{targetPos}");
            return true;
        }
        else
        {
            targetPos = Vector2Int.zero; //nullの代わり
            //Debug.Log("nearestTargetList:" + string.Join(",", nearestTargetList) + $"\ntargetPos:{targetPos}");
            return false;
        }
    }

    public int CalcDamage(int atk)
    {
        return atk; //とりあえず今は攻撃力をそのまま返すだけ
    }



}

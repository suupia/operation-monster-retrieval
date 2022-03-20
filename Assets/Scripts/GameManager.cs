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
    [SerializeField] public SelectStageButtonMGR[] selectStageButtonMGRs; //7つセットする
    [SerializeField] public SelectCharacterButtonMGR[] selectCharacterButtonMGRs; //4つセットする
    [SerializeField] public CharacterInReserveMGR[] characterInReserveMGRs; //とりあえず9つセットする
    [SerializeField] public CharacterInCombatMGR[] characterInCombatMGRs; //4つセットする
    [SerializeField] public TimerMGR timerMGR;
    [SerializeField] public EnergyMGR energyMGR;
    [SerializeField] public TowerPosDataMGR towerPosDataMGR;
    [SerializeField] public SaveMGR saveMGR;
    [SerializeField] public CharacterSkillsDataMGR characterSkillsDataMGR;
    [SerializeField] public CurveToMouseMGR curveToMouseMGR;
    [SerializeField] public MusicMGR musicMGR;
    //[SerializeField] public CharacterSkillsMGR characterSkillsMGR; 

    [SerializeField] public GameObject selectStageCanvas; //SetActiveで表示を制御するのでゲームオブジェクトごと取得する必要がある インスペクター上でセットする
    [SerializeField] public GameObject menuCanvas; //SetActiveで表示を制御するのでゲームオブジェクトごと取得する必要がある インスペクター上でセットする
    [SerializeField] public StatusCanvasMGR statusCanvasMGR;
    [SerializeField] GameObject frameCanvas; //SetActiveで表示を制御するのでゲームオブジェクトごと取得する必要がある インスペクター上でセットする

    [SerializeField] GameObject resultCanvas; //SetActiveで表示を制御するのでゲームオブジェクトごと取得する必要がある インスペクター上でセットする
    [SerializeField] Text resultText;

    [SerializeField] public float gameSpeed = 1; //デバッグ用　コルーチンの中とかで使い、ゲームスピードを速くする

    bool initializationFlag = false;
    public bool InitializationFlag
    {
        get { return initializationFlag; }
    }

    int numOfCharacterInCombat = 4; //戦闘に参加するモンスターの種類は4種類
    int[] idsOfCharactersInCombat; //戦闘に参加しているモンスターのID (numOfCharacterTypesの分だけ要素を用意する)
    public int[] IDsOfCharactersInCombat //プロパティ
    {
        get { return idsOfCharactersInCombat; }
    } //getterのみ

    [SerializeField] int[] characterIDsThatCanBeUsed;
    public int[] CharacterIDsThatCanBeUsed  //8つ用意する(最大でStage7の値7が入るから) クリアしたステージを引数に取って、使えるキャラクターのIDの最大値を返す配列(この配列は単調増加であることに注意)
    {
        get { return characterIDsThatCanBeUsed; }
    }


    int maxCharacterNum = 50; //フィールドに出せるキャラクターの最大数 characterIDが11の時は23体まで出せ、characterIDが2の時は50体出しても問題ない。　とりあえず、にゃんこ大戦争と同じように50にしておく
    int maxRobotNum = 30;   //30は適当な数字
    public int MaxCharacterNum //Getterのみ
    {
        get { return maxCharacterNum; }
    }
    [SerializeField] int currentCharacterNum = 0; //フィールド上にいるキャラクターの数 キャラクターをずらすのと、場に出せる上限を決めるために必要（デバッグ用にSerializeFieldにしている）
    [SerializeField] int currentRobotNum = 0; //フィールド上にいるロボットの数 ロボットをずらすのと、場に出せる上限を決めるために必要（デバッグ用にSerializeFieldにしている）

    public int CurrentCharacterNum
    {
        get { return currentCharacterNum; }
        set
        {
            currentCharacterNum = value;
        }
    }
    public int CurrentRobotNum
    {
        get { return currentRobotNum; }
        set { currentRobotNum = value; }
    }
    int maxTowerNum; //各ステージでのタワーの最大値(ステージごとに変わる)
    public int MaxTowerNum //Getterのみ
    {
        get { return maxTowerNum; }
    }
    [SerializeField] int currentTowerNum; //生き残っているタワーの数　デバッグ用にSerializeFieldにしている
    public int CurrentTowerNum
    {
        get { return currentTowerNum; }
        set
        {
            currentTowerNum = value;
            //Debug.Log($"currentTowerNumを{currentTowerNum}にしました");
        }
    }


    int stagesClearedNum = 0;
    public int StagesClearedNum
    { //プロパティ　どこのステージまでクリアしたかを記録しておく
        get { return stagesClearedNum; }
        set
        {
            //Debug.Log($"StagesClearedNumのSetterを開始します");

            //StageButtonを更新する
            if (value - stagesClearedNum < 0)
            {
                Debug.LogError("StagesCleardNumの値が小さくなるように値が代入されました");
                return;
            }
            stagesClearedNum = value;
            for (int i = 0; i < selectStageButtonMGRs.Length; i++)
            {
                selectStageButtonMGRs[i].UpdateSelectStageButtonMGR();
            }

            //CharacterInReverseとCharacterInCombatを更新する
            for (int i = 0; i < characterInReserveMGRs.Length; i++)  //CharacterInReserveの数は最大12
            {
                characterInReserveMGRs[i].UpdateCharacterInReserve();
            }
        }
    }


    [SerializeField] AutoRouteData autoRouteData; //インスペクター上でセットする
    [SerializeField] ManualRouteData manualRouteData; //インスペクター上でセットする
    public AutoRouteData[] characterAutoRouteDatas;
    public ManualRouteData[] characterManualRouteDatas;
    public AutoRouteData[] robotAutoRouteDatas;


    public readonly int wallID = 5;
    public readonly int groundID = 11;
    public readonly int towerID = 7;
    public readonly int allyCastleID = 13;
    public readonly int enemyCastleID = 17;
    public readonly int characterID = 2;
    public readonly int robotID = 3;

    public GameObject[] characterPrefabs;
    public GameObject[] robotPrefabs;
    CharacterMGR[] characterDatabase; //上のchraracterPrefabsをCharacter型に直したもの。データベースとして使う。

    int dragNum; //ドラッグした番号を保持しておくために必要
    bool dragFlag; //CharacterInReserveからCharacterInCombatへのドラッグのみを有効にするために必要
    public bool DragFlag
    {
        get { return dragFlag; }
        set { dragFlag = value; }
    }

    public int copyingSelectCharacterButtonNum = -1; //ManualRouteをコピーするとき、ドラッグした番号を保持しておくために必要
    public bool copyingManualRoute; //ManualRouteをコピーするとき、SelectCharacterButtonを選択している間trueにする
    public int mouseEnteredSelectCharacterButtonNum = -1;

    float characterDisplacement = 0.03f; //キャラクターがスポーンしたときにどれくらいズレるかを決める(10回で一周するようにする)

    public CharacterMGR.Mode[] characterMode; //キャラクターの種類ごとの操作モードを格納する

    bool[] isSpawnCharacterArray; //コルーチン用

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
        SetupGame,
        MakeTheFirstRoad,
        RunningGame,
        PauseTheGame,
        ShowingResults,
        ResetData
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
    //Getter
    public CharacterMGR GetCharacterDatabase(int characterID)
    {
        return characterDatabase[characterID];
    }
    public CharacterMGR GetCharacterMGRFromButtonNum(int buttonNum)
    {
        return characterDatabase[IDsOfCharactersInCombat[buttonNum]];
    }
    //Setter
    public void SetCharacterMode(int characterTypeNum, CharacterMGR.Mode mode)
    {
        if (characterTypeNum < 0 || this.numOfCharacterInCombat < characterTypeNum)
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
        if (InitializationFlag == false) Initialization();
    }

    public void Initialization() //Start関数の変わり。他の関数の初期化で参照したいため、InitializationFlag(bool)を用意し、他から呼ぶときに初期化されていなければ初期化するようにする
    {
        initializationFlag = true;

        idsOfCharactersInCombat = new int[numOfCharacterInCombat];

        isSpawnCharacterArray = new bool[numOfCharacterInCombat];
        for (int i = 0; i < isSpawnCharacterArray.Length; i++)
        {
            isSpawnCharacterArray[i] = false;
        }

        characterAutoRouteDatas = new AutoRouteData[numOfCharacterInCombat];
        characterManualRouteDatas = new ManualRouteData[numOfCharacterInCombat];
        robotAutoRouteDatas = new AutoRouteData[robotPrefabs.Length];

        for (int i = 0; i < numOfCharacterInCombat; i++)
        {
            characterAutoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(), mapMGR.GetMapHeight());
            characterManualRouteDatas[i] = new ManualRouteData(); //今はmanualRouteDataがない
        }
        for (int i = 0; i < robotPrefabs.Length; i++)
        {
            robotAutoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(), mapMGR.GetMapHeight());
        }

        characterDatabase = new CharacterMGR[characterPrefabs.Length];
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            //characterDatabase[i] = characterPrefabs[i].GetComponent<CharacterMGR>();
            characterDatabase[i] = characterPrefabs[i].GetComponent<CharacterMGR>().Clone(); //ディープコピーをする

            //キャラクターのレベルをファイルから読み込む
            characterDatabase[i].SetInitiLevel(saveMGR.GetCharacterLevel(i));

        }

        characterMode = new CharacterMGR.Mode[numOfCharacterInCombat];
        for (int i = 0; i < characterMode.Length; i++)
        {
            characterMode[i] = CharacterMGR.Mode.Auto; //デフォルトはAutoMode
        }



        StagesClearedNum = saveMGR.GetStagesCleardNum();

        statusCanvasMGR.InitiStatusCanvasMGR();

        for (int i = 0; i < IDsOfCharactersInCombat.Length; i++) //CharacterInCombatの順番をファイルから読み込む
        {
            IDsOfCharactersInCombat[i] = saveMGR.GetCharacterInCombatID(i);
        }


        StartSelectingStage();

    }
    public void StartSelectingStage()
    {
        state = State.SelectingStage;

        musicMGR.StopAllBGM();

        resultCanvas.SetActive(false);
        selectStageCanvas.SetActive(true);
        //次のPlayingGameに備えて前の戦闘のデータをここでリセットする（今は特に書くことはない）

        frameCanvas.SetActive(true);      //SelectStageCanvasとMenuCanvasではFrameCanvasを表示する
    }

    public void SetupGame()
    {
        state = State.SetupGame;
        selectStageCanvas.SetActive(false);

        foreach (SelectCharacterButtonMGR MGR in selectCharacterButtonMGRs)
        {
            MGR.InitiSelectCharacterButton();
        }
        timerMGR.InitiTimer();
        energyMGR.InitiEnergy();

        //SetCharacterTypeIDInCombat();

        frameCanvas.SetActive(false);    //戦闘画面ではFrameCanvasを非表示にする

        mapMGR.SetupMap();
        musicMGR.StartStageBGM(mapMGR.GetStageNum());

        CurrentCharacterNum = 0;
        CurrentRobotNum = 0;
        maxTowerNum = mapMGR.GetMaxTowerNum();
        CurrentTowerNum = maxTowerNum;

        MakeTheFirstRoad();

        InitiManualRouteData();

    }
    public void MakeTheFirstRoad()
    {
        state = State.MakeTheFirstRoad;
        mapMGR.makeTheFirstRoadGO.SetActive(true);
    }
    public void RunningGame()
    {
        state = State.RunningGame;
        mapMGR.makeTheFirstRoadGO.SetActive(false);
    }
    public void PauseTheGame()
    {
        state = State.PauseTheGame;
    }
    public void StartShowingResults(bool isWin)
    {
        state = State.ShowingResults;

        //resultTextGO.SetActive(true);
        resultCanvas.SetActive(true);

        if (isWin)
        {
            int EXPGained = mapMGR.GetEXPOfTheStage();

            //経験値を与える
            statusCanvasMGR.EXPRetained += EXPGained;

            resultText.text = $"勝利！\nEXP{EXPGained}を獲得！";


        }
        else
        {
            //resultText.text = "敗北";
            resultText.text = "敗北";

        }

        Debug.LogWarning($"StagesClearedNum:{StagesClearedNum}");
        //StagesClearedNumを更新する
        if (StagesClearedNum < mapMGR.GetStageNum() + 1)
        {
            StagesClearedNum = mapMGR.GetStageNum() + 1;
            Debug.LogWarning($"StagesClearedNumを更新して:{StagesClearedNum}　にしました");

        }


        //Saveをする
        saveMGR.SaveEXPAmount(statusCanvasMGR.EXPRetained);
        saveMGR.SaveStagesCleardNum(StagesClearedNum);

    }
    public void ResetData()
    {
        state = State.ResetData;
    }
    public void SpawnCharacter(int buttonNum)
    {
        if (isSpawnCharacterArray[buttonNum])
        {
            return;
        }

        if (mapMGR.GetMapValue(mapMGR.characterSpawnPoss) % GameManager.instance.groundID != 0)
        {
            Debug.Log($"{mapMGR.characterSpawnPoss}のmapValueにgroundIDが含まれないため、モンスターをスポーンできません。");
            return;
        }
        if (energyMGR.CurrentEnergy < characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost())
        {
            Debug.Log($"Energyが足りないためモンスターをスポーンできません。（energyMGR.CurrentEnergy :{energyMGR.CurrentEnergy},Cost:{characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost()}）");
            return;
        }
        if (currentCharacterNum >= maxCharacterNum)
        {
            Debug.Log($"currentCharacterNum:{currentCharacterNum}がmaxCharacterNum:{maxCharacterNum}以上であるため、スポーンできません");
            return;
        }

        Debug.Log("SpawnCharacterCoroutineを実行します");
        StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss, buttonNum));


    }


    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector, int buttonNum)
    {
        isSpawnCharacterArray[buttonNum] = true;

        int characterTypeID = IDsOfCharactersInCombat[buttonNum];

        Vector3 displacement = new Vector3(characterDisplacement * (currentCharacterNum % 7) - 3 * characterDisplacement, 0.5f * (characterDisplacement * (currentCharacterNum % 7) - 3 * characterDisplacement), 0); //キャラクターを少しずらす y方向のズレはx方向のズレの0.5倍
        Debug.Log($"displacement:{displacement}");

        GameObject characterGO = Instantiate(characterPrefabs[characterTypeID], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0) + displacement, Quaternion.identity);

        CharacterMGR characterMGR = characterGO.GetComponent<CharacterMGR>();

        //スポーンでCostを消費する
        energyMGR.CurrentEnergy -= characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost();

        //キャラクターのデータをここで渡す
        characterMGR.SetCharacterData(buttonNum, characterTypeID);

        mapMGR.MultiplySetMapValue(vector, characterID);
        mapMGR.GetMap().AddUnit(vector, characterMGR);

        currentCharacterNum++;

        //キャラクターのモードを決める
        characterMGR.SetMode(characterMode[buttonNum]);


        float time = 0;
        while (time < characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCoolTime()) //クールタイムの時間だけ止める
        {
            time += Time.deltaTime * GameManager.instance.gameSpeed;
            selectCharacterButtonMGRs[buttonNum].RefreshGauge(time / characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCoolTime());

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める

            yield return null;
        }

        isSpawnCharacterArray[buttonNum] = false;
    }
    public void SpawnRobot()
    {
        if (mapMGR.GetMapValue(mapMGR.robotSpawnPoss) % GameManager.instance.groundID != 0)
        {
            Debug.Log($"{mapMGR.robotSpawnPoss}のmapValueにgroundIDが含まれないため、ロボットをスポーンできません。");
            return;
        }

        if (currentRobotNum >= maxRobotNum)
        {
            Debug.Log($"currentRobotNum:{currentRobotNum}がmaxRobotNum:{maxRobotNum}以上であるため、スポーンできません");
            return;
        }

        Debug.Log("SpawnRobotCoroutineを実行します");
        StartCoroutine(SpawnRobotCoroutine(mapMGR.robotSpawnPoss, 0));

    }
    private IEnumerator SpawnRobotCoroutine(Vector2Int vector, int robotTypeID)
    {

        Vector3 displacement = new Vector3(characterDisplacement * (currentRobotNum % 7) - 3 * characterDisplacement, 0.5f * (characterDisplacement * (currentRobotNum % 7) - 3 * characterDisplacement), 0); //ロボットを少しずらす y方向のズレはx方向のズレの0.5倍
        Debug.Log($"displacement:{displacement}");

        GameObject robotGO = Instantiate(robotPrefabs[robotTypeID], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0) + displacement, Quaternion.identity);

        RobotMGR robotMGR = robotGO.GetComponent<RobotMGR>();



        //ロボットのautoRoteを初期化する
        robotMGR.SetCharacterData(robotTypeID);

        mapMGR.MultiplySetMapValue(vector, robotID);
        mapMGR.GetMap().AddUnit(vector, robotMGR);

        currentRobotNum++;

        //クールタイムの処理はとりあえずコメントアウト
        //float time = 0;
        //while (time < characterDatabase[IDsOfCharactersInCombat[robotTypeNum]].GetCoolTime()) //クールタイムの時間だけ止める
        //{
        //    time += Time.deltaTime * GameManager.instance.gameSpeed;
        //    selectCharacterButtonMGRs[robotTypeNum].RefreshGauge(time / characterDatabase[IDsOfCharactersInCombat[robotTypeNum]].GetCoolTime());

        //    while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //ポーズ中は止める

        //    yield return null;
        //}

        yield return new WaitForSeconds(0); ; //何も返さないとまずいので適当な戻り値を返す

    }
    public void DragCharacterData(int dragNum)
    {
        this.dragNum = dragNum;
        Debug.Log($"dragNum:{dragNum}");
    }

    public void DropCharacterData(int dropNum)
    {
        IDsOfCharactersInCombat[dropNum] = dragNum;
        Debug.Log($"IDsOfCharactersInCombat[{dropNum}]:{IDsOfCharactersInCombat[dropNum] }");
    }
    public int CalcDamage(int atk)
    {
        return atk; //とりあえず今は攻撃力をそのまま返すだけ
    }

    public void InitiManualRouteData()
    {
        foreach (ManualRouteData m in characterManualRouteDatas)
        {
            m.ResetManualRouteData();
        }
    }
}

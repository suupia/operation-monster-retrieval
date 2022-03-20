using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapMGR : MonoBehaviour
{
[System.Serializable]
    class StageTileArray //インスペクター上でセットできるようにするためにクラスを作る
    {
        [SerializeField] public TileBase[] stageTileArray;
    }
    [SerializeField] Tilemap tilemap;
    [SerializeField] StageTileArray[]  tileArray;
    [SerializeField] int stageNum; //ステージの番号によって使うタイルマップを決める（0からスタート）あと経験値の決定にも使う

    MapData map;

    [SerializeField] int mapHeight;
    [SerializeField] int mapWidth;
    [SerializeField] int outOfStageQ; //マップの外にタイルを何枚はみ出して張るかを決める

    [SerializeField] GameObject allysCastlePrefab;
    [SerializeField] Vector2Int allysCastlePos;
    [SerializeField] bool isFristTimePlaceAllyCastle = true; //自分の城にはスクリプトがついていないため、Destroyできない　そのため、boolで一回だけ配置されるようにする
    [SerializeField] Vector2Int[] characterSpawnPossFromCastle;
    [System.NonSerialized] public Vector2Int[] characterSpawnPoss;
    [SerializeField] GameObject enemysCastlePrefab;
    [SerializeField] Vector2Int enemysCastlePos;

    [SerializeField] GameObject[] towerPrefabs;
    int maxTowerNum; //PlaceTower()で決定する

    [SerializeField] int numOfFristRoad; //インスペクター上以外で値を代入してはいけない
    [SerializeField] int numOfFristRoadCounter; //SetUpMap()で numOfFristRoadCounter = numOfFristRoad; と初期化している  デバッグ用にSerializeFieldにしておく
    [SerializeField] public GameObject makeTheFirstRoadGO; //GameManagerでSetActiveを変える
    Text makeTheFirstRoadText;
    bool isDisplayMakefristRoadAgainCoroutine = false;

    [SerializeField] int[] EXPOfTheStage; //クリア時にもらえる経験値をインスペクター上で決める

    //Getter
    public MapData GetMap()
    {
        return map;
    }
    public int GetMapSize()
    {
        return mapHeight * mapWidth;
    }
    public int GetMapHeight()
    {
        return mapHeight;
    }
    public int GetMapWidth()
    {
        return mapWidth;
    }
    public long GetMapValue(int x, int y)
    {
        return map.GetValue(x, y);
    }
    public long GetMapValue(Vector2Int vector)
    {
        return map.GetValue(vector);
    }
    public long GetMapValue(int index)
    {
        return map.GetValue(index);
    }
    public Vector2Int GetAllysCastlePos()
    {
        return allysCastlePos;
    }
    public Vector2Int GetEnemysCastlePos()
    {
        return enemysCastlePos;
    }
    public int GetMaxTowerNum()
    {
        return maxTowerNum;
    }
    public int GetNumOfFristRoad()
    {
        return numOfFristRoad;
    }
    public int GetStageNum()
    {
        return stageNum;
    }
    public int GetEXPOfTheStage() 
    {
        return EXPOfTheStage[stageNum];
    }

    //Setter
    public void MultiplySetMapValue(Vector2Int vector, int value)
    {
        map.MultiplySetValue(vector, value);
    }

    public void DivisionalSetMapValue(Vector2Int vector, int value)
    {
        if (map.GetValue(vector) % value != 0)
        {
            Debug.LogError($"DivisionalSetMapValue({vector},{value})でmap({vector})の値を{value}で割り切ることができませんでした。");
        }
        else
        {
            map.DivisionalSetValue(vector, value);
        }
    }
    public void SetStageNum(int stageNum)
    {
        this.stageNum = stageNum;
    }

    public void SetupMap() //これをGameManagerから呼ぶ
    {
        //初期化
        map = null;
        map = new MapData(mapWidth, mapHeight); //mapは1つしかないのでとりあえず、numberは0としておく

        numOfFristRoadCounter = GetNumOfFristRoad();

        makeTheFirstRoadText = makeTheFirstRoadGO.GetComponent<Text>();
        makeTheFirstRoadText.text = $"敵の城につながるように\nあと" + $"{numOfFristRoadCounter}".PadLeft(2) + "つ道を配置してください"; //表示をリセットする


        //Debug.Log($"numOfFristRoadCounterを{GetNumOfFristRoad()}に初期化しました");

        RenderMap();

        PlaceCastle();

        PlaceTower();
    }
    private void ReSetupMap() //MakeTheFirstRoadで失敗したときに呼ばれる
    {
        for (int x =0;x<map.Width;x++)
        {
            for(int y = 0; y < map.Height; y++)
            {
                if(map.GetValue(x,y) % GameManager.instance.groundID == 0) //MakeTheFirstRoadで作った道を壁に戻す
                {
                    map.SetValue(x, y,GameManager.instance.wallID); //直接値を代入していることに注意
                }
            }
        }

        numOfFristRoadCounter = GetNumOfFristRoad();

        makeTheFirstRoadText = makeTheFirstRoadGO.GetComponent<Text>();
        makeTheFirstRoadText.text = $"敵の城につながるように\nあと" + $"{numOfFristRoadCounter}".PadLeft(2) + "つ道を配置してください"; //表示をリセットする


        //Debug.Log($"numOfFristRoadCounterを{GetNumOfFristRoad()}に初期化しました");

        RenderMap();

        //SetupMap()に比べて、PlaceCastle()とPlaceTower()がない
    }
    private void RenderMap()
    {
        //マップをクリアする（重複しないようにする）
        tilemap.ClearAllTiles();

        for (int y = -outOfStageQ; y < map.Height + outOfStageQ; y++)
        {
            for (int x = -outOfStageQ; x < map.Width + outOfStageQ; x++)
            {
                SetTileAccordingToValues(x, y);
            }
        }

    }

    private void PlaceCastle()
    {
        if (isFristTimePlaceAllyCastle)
        {
            isFristTimePlaceAllyCastle = false;
            Instantiate(allysCastlePrefab, new Vector3(allysCastlePos.x, allysCastlePos.y + 1, 0), Quaternion.identity); //画像の中心が格子点にくるように、+1していることに注意
        }
        GameObject enemyCastleGO = Instantiate(enemysCastlePrefab, new Vector3(enemysCastlePos.x + 1, enemysCastlePos.y, 0), Quaternion.identity);
        CastleMGR enemyCastleMGR = enemyCastleGO.GetComponent<CastleMGR>();

        map.MultiplySetValue(enemysCastlePos, GameManager.instance.facilityID); //数値データをセット
        map.SetFacility(enemysCastlePos, enemyCastleMGR); //スクリプトをセット

        characterSpawnPoss = new Vector2Int[characterSpawnPossFromCastle.Length];
        Debug.Log($"characterSpawnPossFromCastle.Length={characterSpawnPossFromCastle.Length}");
        for (int i = 0; i < characterSpawnPossFromCastle.Length; i++)
        {
            characterSpawnPoss[i] = allysCastlePos + characterSpawnPossFromCastle[i];
            Debug.Log($"characterSpawnPoss[{i}]={allysCastlePos + characterSpawnPossFromCastle[i]}");
        }
    }

    private void PlaceTower()
    {
        maxTowerNum = 0; //初期化

        for (int y = 0;y< mapHeight;y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int towerType = GameManager.instance.towerPosDataMGR.GetTowerPosDataArray()[stageNum].posData[y * mapWidth + x]; //(x,y)に対応するjsonファイルに書いてある値
                if (towerType != -1) //-1でないならば入っている数字に応じて   タワーの種類と位置を決める
                {
                    if (towerType >= towerPrefabs.Length) //入っている数字が適切かどうかチェックする
                    {
                        Debug.LogError($"towerPosDataArrayにタワーの種類以上の値が入っています");
                        return;
                    }

                    maxTowerNum++;

                    Vector2Int towerSpawnPos = new Vector2Int(x, y);

                    GameObject towerGO = Instantiate(towerPrefabs[towerType], new Vector3(towerSpawnPos.x + 0.5f, towerSpawnPos.y + 0.75f, 0), Quaternion.identity);
                    TowerMGR towerMGR = towerGO.GetComponent<TowerMGR>();

                    map.MultiplySetValue(towerSpawnPos, GameManager.instance.facilityID); //数値データをセット
                    map.SetFacility(towerSpawnPos, towerMGR); //スクリプトをセット

                }
            }
        }

    }


    private void SetTileAccordingToValues(int x, int y)
    {

        if (0 <= y && y < map.Height && 0 <= x && x < map.Width)
        {
            // 1 = タイルあり、0 = タイルなし
            if (map.GetValue(x, y) % GameManager.instance.wallID == 0)
            {
                if (CalculateTileType(x, y) < 47) //周りがすべて壁のタイルは3種類ある
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[CalculateTileType(x, y)]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]);
                }
                //Debug.Log($"タイルを{x},{y}に敷き詰めました");
            }
            else if (map.GetValue(x, y) % GameManager.instance.groundID == 0)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(50, 52 + 1)]);
            }

        }
        else //mapの領域外
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]); //全方向が壁のタイルを張る(3枚)
        }
    }
    private int CalculateTileType(int x, int y)
    {
        bool upIsWall = false;
        bool leftIsWall = false;
        bool downIsWall = false;
        bool rightIsWall = false;
        int upleftWallValue = 0; //1のときwallがあることを表す
        int downleftWallValue = 0;
        int downrightWallValue = 0;
        int uprightWallValue = 0;
        int binarySub;

        if (IsOutRangeOfMap(x, y))
        {
            Debug.LogError($"CalculateTileType({x},{y})の引数でmapの範囲外が指定されました");
            return -100;
        }

        //そもそもgroundIDの時は0を返すようにする（これはRenderMapでは使わない）
        if (map.GetValue(x, y) % GameManager.instance.groundID == 0)
        {
            return 0;
        }

        //if (y == 0 || x == 0 || y == mapHeight - 1 || x == mapWidth - 1)
        //{
        //    if (x == 0)
        //    {
        //        leftIsWall = true;
        //        upleftWallValue = 1;
        //        downleftWallValue = 1;
        //    }

        //    if (y == 0) //else ifにしないのは条件が重複する恐れがあるため
        //    {
        //        downIsWall = true;
        //        downleftWallValue = 1;
        //        downrightWallValue = 1;
        //    }

        //    if (x == mapWidth - 1)
        //    {
        //        rightIsWall = true;
        //        uprightWallValue = 1;
        //        downrightWallValue = 1;
        //    }

        //    if (y == mapHeight - 1)
        //    {
        //        upIsWall = true;
        //        upleftWallValue = 1;
        //        uprightWallValue = 1;
        //    }

        //}


        if (y == map.Height - 1 || map.GetValue(x, y + 1) % GameManager.instance.wallID == 0) //左側の条件式の方が先に判定されるので、mapの範囲外にGetValueすることはない（と思う）
        {
            upIsWall = true;
        }

        if (x == 0 || map.GetValue(x - 1, y) % GameManager.instance.wallID == 0)
        {
            leftIsWall = true;
        }

        if (y == 0 || map.GetValue(x, y - 1) % GameManager.instance.wallID == 0)
        {
            downIsWall = true;
        }

        if (x == map.Width - 1 || map.GetValue(x + 1, y) % GameManager.instance.wallID == 0)
        {
            rightIsWall = true;
        }


        if (x == 0 || y == map.Height - 1 || map.GetValue(x - 1, y + 1) % GameManager.instance.wallID == 0) //この4つの場合分けは4隅を調べればよいので、xだけの判定で十分
        {
            upleftWallValue = 1;
        }

        if (x == 0 || y == 0 || map.GetValue(x - 1, y - 1) % GameManager.instance.wallID == 0)
        {
            downleftWallValue = 1;
        }

        if (x == map.Width - 1 || y == 0 || map.GetValue(x + 1, y - 1) % GameManager.instance.wallID == 0)
        {
            downrightWallValue = 1;
        }

        if (x == map.Width - 1 || y == map.Height - 1 || map.GetValue(x + 1, y + 1) % GameManager.instance.wallID == 0)
        {
            uprightWallValue = 1;
        }








        //壁と接しないとき
        if (!upIsWall && !leftIsWall && !downIsWall && !rightIsWall)
        {
            return 1;
        }

        //1つの壁と接するとき
        if (upIsWall && !leftIsWall && !downIsWall && !rightIsWall)
        {
            return 2;
        }
        else if (!upIsWall && leftIsWall && !downIsWall && !rightIsWall)
        {
            return 3;
        }
        else if (!upIsWall && !leftIsWall && downIsWall && !rightIsWall)
        {
            return 4;
        }
        else if (!upIsWall && !leftIsWall && !downIsWall && rightIsWall)
        {
            return 5;
        }

        //2つの壁と接するとき
        if (upIsWall && !leftIsWall && downIsWall && !rightIsWall)
        {
            return 6;
        }
        else if (!upIsWall && leftIsWall && !downIsWall && rightIsWall)
        {
            return 7;
        }
        else if (upIsWall && leftIsWall && !downIsWall && !rightIsWall)
        {
            return 8 + upleftWallValue;
        }
        else if (!upIsWall && leftIsWall && downIsWall && !rightIsWall)
        {
            return 10 + downleftWallValue;
        }
        else if (!upIsWall && !leftIsWall && downIsWall && rightIsWall)
        {
            return 12 + downrightWallValue;
        }
        else if (upIsWall && !leftIsWall && !downIsWall && rightIsWall)
        {
            return 14 + uprightWallValue;
        }

        //3つの壁と接するとき
        if (upIsWall && leftIsWall && downIsWall && !rightIsWall)
        {
            binarySub = 0;
            binarySub = upleftWallValue + 2 * downleftWallValue;
            return 16 + binarySub;

        }
        else if (!upIsWall && leftIsWall && downIsWall && rightIsWall)
        {
            binarySub = 0;
            binarySub = downleftWallValue + 2 * downrightWallValue;
            return 20 + binarySub;

        }
        else if (upIsWall && !leftIsWall && downIsWall && rightIsWall)
        {
            binarySub = 0;
            binarySub = downrightWallValue + 2 * uprightWallValue;
            return 24 + binarySub;
        }
        else if (upIsWall && leftIsWall && !downIsWall && rightIsWall)
        {
            binarySub = 0;
            binarySub = uprightWallValue + 2 * upleftWallValue;
            return 28 + binarySub;
        }

        //4つの壁と接するとき
        if (upIsWall && leftIsWall && downIsWall && rightIsWall)
        {
            //周囲に地面が一つでもあるとき
            if (upleftWallValue == 0 || downleftWallValue == 0 || downrightWallValue == 0 || uprightWallValue == 0)
            {
                binarySub = 0;
                binarySub = upleftWallValue + 2 * downleftWallValue + 4 * downrightWallValue + 8 * uprightWallValue;
                return 32 + binarySub;
            }
            else   //周囲がすべて壁の時はタイルの種類をあとで乱数で決める
            {
                return 47;
            }

        }

        Debug.LogError($"CalculateTileNum({y} ,{x})に失敗しました");
        return -100;
    }
    private bool IsOutRangeOfMap(int x, int y)
    {
        if (x < 0 || y < 0 || x > map.Width - 1 || y > map.Height - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void MakeRoadByPointer(int x, int y)
    {
        if (x < 1 || y < 1 || x > map.Width - 2 || y > map.Height - 2)
        {
            //Debug.Log($"MakeRoad({x},{y})の引数で掘れる範囲の外が指定されました。");
            return;
        }

        if (GameManager.instance.energyMGR.CurrentEnergy < GameManager.instance.energyMGR.EnergyToMakeRoad && GameManager.instance.state == GameManager.State.RunningGame)
        {
            //Debug.Log("エネルギーが足りないので道を作れません");
            return;
        }
        if(GetMapValue(x,y) / GameManager.instance.wallID != 1) //普通の割り算をしていることに注意
        {
            //Debug.Log("壁のみのマスでないのでMakeROadByPointerを中断します");
            return;
        }
        if (GameManager.instance.state == GameManager.State.RunningGame) //MakeTheFirstRoadの時はエネルギーを消費しない
        {
            GameManager.instance.energyMGR.CurrentEnergy -= GameManager.instance.energyMGR.EnergyToMakeRoad;
        }
        if (GameManager.instance.state == GameManager.State.MakeTheFirstRoad && map.GetValue(x,y) == GameManager.instance.wallID) //MakeTheFirstRoadの処理
        {
            numOfFristRoadCounter--;
            //Debug.Log($"numOfFristRoadCounterが{numOfFristRoadCounter}になりました");

            if (isDisplayMakefristRoadAgainCoroutine) return; //コルーチンが作動している間は以下の処理を行わない

            makeTheFirstRoadText.text = $"敵の城につながるように\nあと" + $"{numOfFristRoadCounter}".PadLeft(2) + "つ道を配置してください";
            if (numOfFristRoadCounter <= 0)
            {
                //Debug.Log("numOfFristRoadCounterが0以下になりました");
                if (IsReachable2())
                {
                    //Debug.Log($"GameManagerのStateをRunningGameにします");
                    GameManager.instance.RunningGame();
                }
                else
                {
                    //Debug.Log("道が敵の城につながっていないため\nやり直してください");
                    makeTheFirstRoadText.text = $"道が敵の城につながっていないため\nやり直してください";
                    StartCoroutine(DisplayMakefristRoadAgainCoroutine());
                }

            }
        }

        MakeRoad(x,y);

    }
    public void MakeRoadByTowerDead(int x, int y) //タワーが破壊されたときに呼ぶ
    {
        if (x < 1 || y < 1 || x > map.Width - 2 || y > map.Height - 2)
        {
            //Debug.Log($"MakeRoad({x},{y})の引数で掘れる範囲の外が指定されました。");
            return;
        }

        MakeRoad(x,y);
    }
    public void MakeRoad(int x , int y)
    {
        Vector2Int vector = new Vector2Int(x, y);


        if (map.GetValue(vector) == GameManager.instance.wallID)
        {
            int[] beforeTileTypes = new int[9];
            int[] afterTileTypes = new int[9];

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (IsOutRangeOfMap(x + dx, y + dy))
                    {
                        beforeTileTypes[(dx + 1) * 3 + (dy + 1)] = -10;
                    }
                    else
                    {
                        beforeTileTypes[(dx + 1) * 3 + (dy + 1)] = CalculateTileType(x + dx, y + dy);
                    }
                }
            }


            map.DivisionalSetValue(vector, GameManager.instance.wallID);
            map.MultiplySetValue(vector, GameManager.instance.groundID);


            //周囲9マスのタイルを更新する必要がある
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int i = (dx + 1) * 3 + (dy + 1); //3進法を利用

                    if (IsOutRangeOfMap(x + dx, y + dy))
                    {
                        afterTileTypes[i] = -10;
                    }
                    else
                    {
                        afterTileTypes[i] = CalculateTileType(x + dx, y + dy);
                    }

                    if (!(beforeTileTypes[i] == afterTileTypes[i]) || (dx == 0 && dy == 0))
                    {
                        SetTileAccordingToValues(x + dx, y + dy);

                    }
                    else
                    {
                        //TileTypeが変化していなかったら、タイルは張り替えない
                    }
                    //Debug.LogWarning($"EqualsCalculateTileType(beforeTileTypes[i], afterTileTypes[i])が{(beforeTileTypes[i] == afterTileTypes[i])}です\n(dx,dy)=({dx},{dy})");
                    //Debug.LogWarning($"(beforeTileTypes[i], afterTileTypes[i])=({beforeTileTypes[i]}, {afterTileTypes[i]})");


                }
            }
        }
    }
    IEnumerator DisplayMakefristRoadAgainCoroutine()
    {
        isDisplayMakefristRoadAgainCoroutine = true;
        //ここに到達したとき、GameManager.instance.stateはState.MakeTheFirstRoad
        yield return new WaitForSeconds(1.2f);
        //コルーチンの後も、State.MakeTheFirstRoadのまま（なぜならば、次の遷移状態はRuuningGameだが、MakeRoadByPointer()のMakeTheFirstRoadの処理のIsReachableのif文でtrueの時に初めて移るから）
        //GameManager.instance.SetupGame(); //SetupGameに戻すのはよくない。遷移状態はMakeTheFirstRoadのままマップの再配置を行う

        ReSetupMap();

        isDisplayMakefristRoadAgainCoroutine = false;
    }

    bool IsReachable()
    {
        int initiValue = -10; //PlaceNumAroundで重複して数字を置かないようにするために必要
        //int roadValue = 1; //roadのマス
        int wallValue = -1; //wallのマス
        int _errorValue = -88;
        int[,] cell = new int[mapHeight,mapWidth]; //到達できるかどうかを判定する用の2次元配列
        Vector2Int startPos = allysCastlePos;
        Vector2Int targetPos = enemysCastlePos;

        Queue<Vector2Int> searchQue = new Queue<Vector2Int>();
        int i = 1; //1から始まることに注意
        bool isComplete = false;
        int maxDistance = 0;

        //初期化
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                    cell[y, x] = initiValue;  
            }
        }

        //mapの壁のマスをwallValueにする。
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (map.GetValue(x, y) % GameManager.instance.wallID == 0)
                {
                    cell[y,x] = wallValue;  //実質これがinitiValueみたいな役割を持つ
                }
            }
        }

        //動けるマスに数字を順番に振っていく
        Debug.Log($"WaveletSearchを実行します startPos:{startPos}");
        WaveletSearch();

        //デバッグ用
        string debugCell = "";
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //debugCell += $"({x},{mapHeight - y-1}){GetValue(mapHeight - y-1, x)}".PadRight(3) + ",";
                debugCell += $"{GetValue(x,mapHeight - y-1)}".PadRight(3) + ",";
            }
            debugCell += "\n";
        }
        //Debug.Log($"cellの中身は\n{debugCell}");

        

        if (CanGetCloseToTheCastle())
        {
            Debug.Log("IsReachableはtrueです");
            return true;
        }
        else
        {
            Debug.Log("IsReachableはfalseです");
            return false; 
        }


        ////////////////////////////////////////////////////////////////////

        //以下ローカル関数
        int GetValue(int x, int y)
        {
            if (IsOutRangeOfMap(x, y))
            {
                Debug.LogError($"領域外に値を取得しようとしました (x,y):({x},{y})");
                return _errorValue;
            }
            if (x == 0 || y == 0 || x == map.Width - 1 || y == map.Height - 1)
            {
                return wallValue; //端は壁扱いにする
            }
            return cell[y,x];
        }

        void SetValue(int x, int y, int value)
        {
            if (IsOutRangeOfMap(x, y))
            {
                Debug.LogError($"領域外に値を設定しようとしました (x,y):({x},{y})");
                return;
            }
            cell[y, x] = value; 
        }

        void WaveletSearch()
        {
            SetValue(startPos.x,startPos.y,0); //startPosの部分だけ周囲の判定を行わないため、ここで個別に設定する 0としているのは0番目のマスだから
            searchQue.Enqueue(startPos);

            while (!isComplete)
            {
                int loopNum = searchQue.Count; //前のループでキューに追加された個数を数える
                Debug.Log($"i:{i}のときloopNum:{loopNum}");
                for (int k = 0; k < loopNum; k++)
                {
                    Debug.Log($"PlaceNumAround({searchQue.Peek()})を実行します");
                    if (isComplete) break;
                    PlaceNumAround(searchQue.Dequeue());
                }
                i++; //前のループでキューに追加された文を処理しきれたら、インデックスを増やして次のループに移る

                if (i > 100) //無限ループを防ぐ用   100まで探索して道が見つからないのならば、処理を中断する
                {
                    isComplete = true;
                    Debug.Log("SearchShortestRouteのwhile文でループが100回行われたため処理を終了します");
                }

            }
        }

        void PlaceNumAround(Vector2Int centerPos)
        {
            Vector2Int inspectPos;

            //8マス判定する（真ん中のマスの判定は必要ない）
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0) continue; //真ん中のマスは飛ばす

                    inspectPos = centerPos + new Vector2Int(x, y);

                    if (IsOutRangeOfMap(inspectPos.x, inspectPos.y)) continue; //map外のマスの判定は飛ばす

                    //Debug.Log($"centerPos:{centerPos},inspectPos:{inspectPos}のとき、CanMove(centerPos, inspectPos):{CanMove(centerPos, inspectPos)}");
                    if (GetValue(inspectPos.x, inspectPos.y) == initiValue && CanMoveDiagonally(centerPos, inspectPos))
                    {
                        SetValue(inspectPos.x,inspectPos.y,i);
                        if(GetValue(inspectPos.x, inspectPos.y)==0) Debug.LogError($"GetValue({inspectPos.x}, {inspectPos.y})が0です");

                        searchQue.Enqueue(inspectPos);
                        Debug.Log($"({inspectPos})を{i}にし、探索用キューに追加しました。");
                    }
                    if (inspectPos == targetPos)
                    {
                        isComplete = true;
                        maxDistance = i - 1;
                        Debug.Log($"isCompleteをtrueにしました。maxDistance:{maxDistance}");
                        break; //探索終了
                    }
                }
            }
        }

        bool CanMoveDiagonally(Vector2Int prePos, Vector2Int afterPos)
        {
            Vector2Int directionVector = afterPos - prePos;

            //斜め移動の時にブロックの角を移動することはできない
            if (directionVector.x != 0 && directionVector.y != 0)
            {
                //水平方向の判定
                if (GetValue(prePos.x + directionVector.x, prePos.y) == wallValue)
                {
                    return false;
                }

                //垂直方向の判定
                if (GetValue(prePos.x,prePos.y + directionVector.y) == wallValue)
                {
                    return false;
                }
            }

            return true;
        }

        bool CanGetCloseToTheCastle()
        {
            Vector2Int inspectPos;
            bool result = false;

            //8マス判定する（真ん中のマスの判定は必要ない）
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0) continue; //真ん中のマスは飛ばす

                    inspectPos = targetPos + new Vector2Int(x, y);

                    if (IsOutRangeOfMap(inspectPos.x, inspectPos.y)) continue; //map外のマスの判定は飛ばす

                    if (GetValue(inspectPos.x, inspectPos.y) != initiValue && GetValue(inspectPos.x, inspectPos.y) != wallValue && CanMoveDiagonally(targetPos, inspectPos))
                     //初期の値でもない　かつ　壁の値でもない　かつ　targetPosからinspectPosへ移動することができる
                    {
                        result = true;
                    }
                }
            }

            if (map.GetValue(allysCastlePos.x,allysCastlePos.y) % GameManager.instance.wallID == 0) //ここは本当のmapを参照して調べる
            {
                Debug.LogWarning("キャラクターのスポーン地点に道がないためfalseです");
                result = false;
            }

            return result;
        }
    }

    bool IsReachable2()
    {
        bool result = false;
        Vector2Int startPos = allysCastlePos;
        Vector2Int targetPos = enemysCastlePos;
        var adjacentPosList = new List<Vector2Int>();//targetPosに隣接する、壁でないマス


        //targetPosの周囲8マスを判定して、adjacentPosListを決める
        Vector2Int inspectPos;
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x == 0 && y == 0) continue; //真ん中のマスは飛ばす

                inspectPos = targetPos + new Vector2Int(x, y);

                if (IsOutRangeOfMap(inspectPos.x, inspectPos.y)) continue; //map外のマスの判定は飛ばす

                if (map.GetValue(inspectPos.x, inspectPos.y) % GameManager.instance.wallID !=0)
                {
                    adjacentPosList.Add(inspectPos);
                }
            }
        }

        if(adjacentPosList.Count == 0)
        {
            Debug.Log("敵の城の周辺に壁のマスしかないためIsReachableはfalseです");
            return false;
        }

        if (map.GetValue(allysCastlePos) % GameManager.instance.wallID == 0)
        {
            Debug.Log("キャラクターのスポーン地点が壁のためfalseです");
            result = false;
        }

        foreach (Vector2Int adjacentPos in adjacentPosList)
        {
            if (Function.SearchShortestNonDiagonalRoute(startPos, adjacentPos) == null)
            {
                result = false;
                continue;
            }

            if (Function.SearchShortestNonDiagonalRoute(startPos ,adjacentPos).Count >=(mapWidth-3)+(mapHeight-3)-1) //少なくとも最短場合よりルートが長くなる (-3は城があるところは壁が確定していることに注意)
            {
                result = true;
            }
        }

        return result;

    }

}


public class MapData
{
    int _width;
    int _height;
    long[] _values = null;
    List<CharacterMGR>[] _characterMGRs = null;
    Facility[] _facilities = null;
    int _edgeValue;
    int _outOfRangeValue = -1;

    //コンストラクタ
    public MapData(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Mapの幅または高さが0以下になっています");
            return;
        }
        _width = width;
        _height = height;
        _values = new long [width * height];
        _characterMGRs = new List<CharacterMGR>[width * height];
        for(int i= 0; i < width * height; i++)
        {
            _characterMGRs[i] = new List<CharacterMGR>();
        }
        _facilities = new Facility[width * height];

        FillAll(GameManager.instance.wallID); //mapの初期化はwallIDで行う
    }

    //プロパティ
    public int Width { get { return _width; } } //親クラスのメンバを使用
    public int Height { get { return _height; } }

    //Getter
    public int GetLength()
    {
        return _values.Length;
    }
    public long GetValue(int x, int y)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"IsOutOfRange({x},{y})がtrueです");
            return _outOfRangeValue;
        }
        if (IsOnTheEdge(x,y))
        {
            //Debug.Log($"IsOnTheEdge({x},{y})がtrueです");
            return _edgeValue;
        }
        return _values[ToSubscript(x, y)];
    }
    public long GetValue(Vector2Int vector)
    {
        return GetValue(vector.x, vector.y);
    }
    public long GetValue(int index)
    {
        if (index < 0 || index > _values.Length)
        {
            Debug.LogError("領域外の値を習得しようとしました");
            return _outOfRangeValue;
        }
        return _values[index];
    }

    public List<CharacterMGR> GetCharacterMGRList(int x,int y)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})がtrueです");
            return null; //例外用の数字を設定できないため、nullを返す
        }
        return _characterMGRs[ToSubscript(x,y)];
    }
    public List<CharacterMGR> GetCharacterMGRList(Vector2Int vector)
    {
        return GetCharacterMGRList(vector.x,vector.y);
    }
    public List<CharacterMGR> GetCharacterMGRList(int index)
    {
        if(index < 0 || index > _values.Length)
        {
            Debug.LogError("領域外の値を習得しようとしました");
            return null; //例外用の数字を設定できないため、nullを返す
        }
        return _characterMGRs[index];
    }
    public Facility GetFacility(int x,int y)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})がtrueです");
            return null; //例外用の数字を設定できないため、nullを返す。
        }
        return _facilities[ToSubscript(x, y)];
    }
    public Facility GetFacility(Vector2Int vector)
    {
        return GetFacility(vector.x,vector.y);
    }
    public Facility GetFacility(int index)
    {
        if (index < 0 || index > _values.Length)
        {
            Debug.LogError("領域外の値を習得しようとしました");
            return null; //例外用の数字を設定できないため、nullを返す
        }
        return _facilities[index];
    }
    //Setter
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"IsOutOfRange({x},{y})がtrueです");
            return;
        }
        _values[ToSubscript(x, y)] = value;
    }

    public void SetValue(Vector2Int vector, int value)
    {
        SetValue(vector.x, vector.y, value);
    }

    public void AddCharacterMGR(int x,int y, CharacterMGR characterMGR)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})がtrueです");
            return;
        }
        _characterMGRs[ToSubscript(x, y)].Add(characterMGR);
    }
    public void AddCharacterMGR(Vector2Int vector,CharacterMGR characterMGR)
    {
        AddCharacterMGR(vector.x,vector.y,characterMGR);
    }
    public void RemoveCharacterMGR(int x,int y ,CharacterMGR characterMGR)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})がtrueです");
            return;
        }
        _characterMGRs[ToSubscript(x, y)].Remove(characterMGR);
    }
    public void RemoveCharacterMGR(Vector2Int vector,CharacterMGR characterMGR)
    {
        RemoveCharacterMGR(vector.x, vector.y, characterMGR);
    }
    public void SetFacility(int x, int y ,Facility facility)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})がtrueです");
            return;
        }
        _facilities[ToSubscript(x, y)] = facility;
    }
    public void SetFacility(Vector2Int vector, Facility facility)
    {
        SetFacility(vector.x,vector.y,facility);
    }
    public void MultiplySetValue(Vector2Int vector, int value)
    {
        int x = vector.x;
        int y = vector.y;

        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"MultiplySetValue({x},{y})で領域外に値{value}を設定しようとしました");
            return;
        }

        _values[ToSubscript(x, y)] *= value;
    }

    public void DivisionalSetValue(Vector2Int vector, int value)
    {
        int x = vector.x;
        int y = vector.y;

        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"DivisionalSetValue({x},{y})で領域外に値{value}を設定しようとしました");
            return;
        }
        if (GetValue(x,y)% value !=0)
        {
            Debug.LogError($"DivisionalSetValue(vector:{vector},value:{value})で余りが出たため実行できません");
            return;
        }

        _values[ToSubscript(x, y)] /= value;
    }



    //添え字を変換する
    int ToSubscript(int x, int y)
    {
        return x + (y * _width);
    }

    public Vector2Int DivideSubscript(int subscript)
    {
        int xSub = subscript % _width;
        int ySub = (subscript - xSub) / _width; //ここは割り算
        return new Vector2Int(xSub, ySub);
    }

    bool IsOutOfRange(int x, int y) //edgeの外側。つまり、データがedgeValueすらない
    {
        if (x < -1 || x > _width) { return true; }
        if (y < -1 || y > _height) { return true; }

        //mapの中
        return false;
    }

    bool IsOnTheEdge(int x, int y)
    {
        if (x == -1 || x == _width) { return true; }
        if (y == -1 || y == _height) { return true; }
        return false;
    }

    bool IsOutOfDataRange(int x,int y) //座標(0,0)～(mapWidht-1,mapHeight-1)のデータが存在する領域の外側
    {
        if (x < 0 || x > _width - 1) { return true; }
        if (y < 0 || y > _height - 1) { return true; }
        return false;
    }

    public void FillAll(int value)
    {
        for (int j = 0; j < _height; j++)
        {
            for (int i = 0; i < _width; i++)
            {
                _values[ToSubscript(i, j)] = value;
            }
        }

        _edgeValue = value;
    }
}


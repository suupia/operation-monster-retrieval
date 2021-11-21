using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapMGR : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase[] tileArray;

    MapDate map;

    [SerializeField] int mapHeight;
    [SerializeField] int mapWidth;
    [SerializeField] int outOfStageQ;

    [SerializeField] GameObject allysCastlePrefab;
    [SerializeField] Vector2Int allysCastlePos;
    [SerializeField] Vector2Int[] characterSpawnPossFromCastle;
    [SerializeField] public Vector2Int[] characterSpawnPoss;
    [SerializeField] GameObject enemysCastlePrefab;
    [SerializeField] Vector2Int enemysCastlePos;

    [SerializeField] GameObject[] towerPrefabs;
    [SerializeField] Vector2Int[] towerPoss;

    //Getter
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
    public int GetMapValue(Vector2Int vector)
    {
        return map.GetValue(vector);
    }
    public int GetMapValue(int index)
    {
        return map.GetValue(index);
    }


    public void SetupMap() //これをGameManagerから呼ぶ
    {
        //初期化
        map = null;
        map = new MapDate(mapWidth, mapHeight); //mapは1つしかないのでとりあえず、numberは0としておく

        RenderMap();

        PlaceCastle();

        PlaceTower();
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
        Instantiate(allysCastlePrefab, new Vector3(allysCastlePos.x + 1, allysCastlePos.y + 1, 0), Quaternion.identity); //画像の中心が格子点にくるように、+1していることに注意
        Instantiate(enemysCastlePrefab, new Vector3(enemysCastlePos.x + 1, enemysCastlePos.y + 1, 0), Quaternion.identity);

        characterSpawnPoss = new Vector2Int[characterSpawnPossFromCastle.Length];
        Debug.Log($"characterSpawnPossFromCastle.Length={characterSpawnPossFromCastle.Length}");
        for(int i = 0; i<characterSpawnPossFromCastle.Length; i++)
        {
            characterSpawnPoss[i] = allysCastlePos + characterSpawnPossFromCastle[i];
            Debug.Log($"characterSpawnPoss[{i}]={allysCastlePos + characterSpawnPossFromCastle[i]}");
        }
    }

    private void PlaceTower()
    {
        for(int i = 0; i < towerPrefabs.Length; i++)
        {
            Instantiate(towerPrefabs[i], new Vector3(towerPoss[i].x+0.5f,towerPoss[i].y+0.75f, 0),Quaternion.identity);

            //map.SetValue(towerPoss[i],GameManager.instance.towerID);
        }
    }


    private void SetTileAccordingToValues(int x, int y)
    {

        if (0 <= y && y < map.Height && 0 <= x && x < map.Width)
        {
            // 1 = タイルあり、0 = タイルなし
            if (map.GetValue(x, y) == GameManager.instance.wallID)
            {
                if (CalculateTileType(x, y) < 47) //周りがすべて壁のタイルは3種類ある
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[CalculateTileType(x, y)]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[UnityEngine.Random.Range(47, 49 + 1)]);
                }
                //Debug.Log($"タイルを{x},{y}に敷き詰めました");
            }
            else if (map.GetValue(x, y) == GameManager.instance.groundID)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[UnityEngine.Random.Range(50, 52 + 1)]);
            }

        }
        else //mapの領域外
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[UnityEngine.Random.Range(47, 49 + 1)]); //全方向が壁のタイルを張る(3枚)
        }
    }
    private int CalculateTileType(int x, int y)
    {
        bool upIsWall = false;
        bool leftIsWall=false;
        bool downIsWall=false;
        bool rightIsWall=false;
        int upleftWallValue =0; //1のときwallがあることを表す
        int downleftWallValue=0;
        int downrightWallValue=0;
        int uprightWallValue=0;
        int binarySub;

        if (IsOutRangeOfMap(x, y))
        {
            Debug.LogError($"CalculateTileType({x},{y})の引数でmapの範囲外が指定されました");
            return -100;
        }

        //そもそもgroundIDの時は0を返すようにする（これはRenderMapでは使わない）
        if (map.GetValue(x, y) == GameManager.instance.groundID)
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

        
            if (y== map.Height-1 ||map.GetValue(x, y + 1) == GameManager.instance.wallID) //左側の条件式の方が先に判定されるので、mapの範囲外にGetValueすることはない（と思う）
            {
                upIsWall = true;
            }

            if (x== 0|| map.GetValue(x - 1, y) == GameManager.instance.wallID)
            {
                leftIsWall = true;
            }

            if (y==0||map.GetValue(x, y - 1) == GameManager.instance.wallID)
            {
                downIsWall = true;
            }

            if (x == map.Width -1||map.GetValue(x + 1, y) == GameManager.instance.wallID)
            {
                rightIsWall = true;
            }


            if (x==0|| y == map.Height-1||map.GetValue(x - 1, y + 1) == GameManager.instance.wallID) //この4つの場合分けは4隅を調べればよいので、xだけの判定で十分
            {
                upleftWallValue = 1;
            }

            if (x==0||y==0||map.GetValue(x - 1, y - 1) == GameManager.instance.wallID)
            {
                downleftWallValue = 1;
            }

            if (x== map.Width-1 || y== 0||map.GetValue(x + 1, y - 1) == GameManager.instance.wallID)
            {
                downrightWallValue = 1;
            }

            if (x== map.Width-1||y==map.Height -1|| map.GetValue(x + 1, y + 1) == GameManager.instance.wallID)
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
    public void MakeRoad(int x, int y)
    {
        Vector2Int vector = new Vector2Int(x, y);

        if (IsOutRangeOfMap(x, y))
        {
            Debug.Log($"MakeRoad({x},{y})の引数でmapの範囲外が指定されました");
            return;
        }

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

            map.SetValue(vector, GameManager.instance.groundID);

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
    private bool IsOutRangeOfMap(int x, int y)
    {
        if (x < 0 || y < 0 || x > map.Width-1 || y > map.Height-1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }





}

public class MapDate
{
    protected int _width;
    protected int _height;
    protected int[] _values = null;
    protected int _outOfRange = -1;

    //コンストラクタ
    public MapDate(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Mapの幅または高さが0以下になっています");
            return;
        }
        _width = width;
        _height = height;
        _values = new int[width * height];

        //mapの初期化は１で行う。あとから部屋や道を空けていく。
        FillAll(GameManager.instance.wallID);
    }

    //プロパティ
    public int Width { get { return _width; } } //親クラスのメンバを使用
    public int Height { get { return _height; } }

    //Getter
    public int GetLength()
    {
        return _values.Length;
    }
    public int GetValue(int x, int y)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"領域外の値を取得しようとしました(x,y)=({x},{y})");
            return _outOfRange;
        }
        return _values[ToSubscript(x, y)];
    }
    public int GetValue(Vector2Int vector)
    {
        return GetValue(vector.x, vector.y);
    }
    public int GetValue(int index)
    {
        if (index < 0 || index > _values.Length)
        {
            Debug.LogError("領域外の値を習得しようとしました");
            return _outOfRange;
        }
        return _values[index];
    }

    //Setter
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError("領域外に値を設定しようとしました");
            return;
        }
        _values[ToSubscript(x, y)] = value;
    }

    public void SetValue(Vector2Int vector, int value)
    {
        SetValue(vector.x, vector.y, value);
    }

    public void SwapMapValue(int startX, int startY, int endX, int endY)
    {
        int startValue = GetValue(startX, startY);
        int endValue = GetValue(endX, endY);
        SetValue(startX, startY, endValue);
        SetValue(endX, endY, startValue);
    }

    //添え字を変換する
    protected int ToSubscript(int x, int y)
    {
        return x + (y * _width);
    }

    public Vector2Int DivideSubscript(int subscript)
    {
        int xSub = subscript % _width;
        int ySub = (subscript - xSub) / _width; //ここは割り算
        return new Vector2Int(xSub, ySub);
    }

    protected bool IsOutOfRange(int x, int y)
    {
        if (x < 0 || x >= _width) { return true; }
        if (y < 0 || y >= _height) { return true; }

        //領域の中
        return false;
    }


    public void FillAll(int value)
    {
        for (int j = 0; j < _height; j++)
        {
            for (int i = 0; i < _width; i++)
            {
                SetValue(i, j, value);
            }
        }
    }
}



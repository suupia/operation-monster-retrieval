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
    private void SetTileAccordingToValues(int x, int y)
    {

        if (0 <= y && y < map.Height && 0 <= x && x < map.Width)
        {
            // 1 = タイルあり、0 = タイルなし
            if (map.GetValue(x, y) == DateMGR.instance.wallID)
            {
                if (CalculateTileType(x, y)<47)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[CalculateTileType(x, y)]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[UnityEngine.Random.Range(47, 49 + 1)]);
                }
                //Debug.Log($"タイルを{x},{y}に敷き詰めました");
            }
            else if (map.GetValue(x, y) == DateMGR.instance.groundID)
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
        bool upIsWall;
        bool leftIsWall;
        bool downIsWall;
        bool rightIsWall;
        int upleftWallValue;
        int downleftWallValue;
        int downrightWallValue;
        int uprightWallValue;
        int binarySub;

        if (IsOutRangeOfMap(x,y))
        {
            Debug.LogError($"CalculateTileType({x},{y})の引数でmapの範囲外が指定されました");
            return -100;
        }

        //そもそもgroundIDの時は0を返すようにする（これはRenderMapでは使わない）
        if (map.GetValue(x,y) == DateMGR.instance.groundID)
        {
            return 0;
        }

        //端のタイルは先に処理する
        if (y == 0 || x == 0 || y == mapHeight - 1 || x == mapWidth - 1)
        {
            return 47;
        }

        if (map.GetValue(x, y + 1) == DateMGR.instance.wallID)
        {
            upIsWall = true;
        }
        else
        {
            upIsWall = false;
        }
        if (map.GetValue(x - 1, y) == DateMGR.instance.wallID)
        {
            leftIsWall = true;
        }
        else
        {
            leftIsWall = false;
        }
        if (map.GetValue(x, y - 1) == DateMGR.instance.wallID)
        {
            downIsWall = true;
        }
        else
        {
            downIsWall = false;
        }
        if (map.GetValue(x + 1, y) == DateMGR.instance.wallID)
        {
            rightIsWall = true;
        }
        else
        {
            rightIsWall = false;
        }

        if (map.GetValue(x - 1, y + 1) == DateMGR.instance.wallID)
        {
            upleftWallValue = 1;
        }
        else
        {
            upleftWallValue = 0;
        }
        if (map.GetValue(x - 1, y - 1) == DateMGR.instance.wallID)
        {
            downleftWallValue = 1;
        }
        else
        {
            downleftWallValue = 0;
        }
        if (map.GetValue(x + 1, y - 1) == DateMGR.instance.wallID)
        {
            downrightWallValue = 1;
        }
        else
        {
            downrightWallValue = 0;
        }
        if (map.GetValue(x + 1, y + 1) == DateMGR.instance.wallID)
        {
            uprightWallValue = 1;
        }
        else
        {
            uprightWallValue = 0;
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

        if (IsOutRangeOfMap(x,y))
        {
            Debug.Log($"MakeRoad({x},{y})の引数でmapの範囲外が指定されました");
            return;
        }

        if (map.GetValue(vector) == DateMGR.instance.wallID)
        {
            int[] beforeTileTypes = new int[9];
            int[] afterTileTypes = new int[9];

            for (int dy =-1; dy <=1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (IsOutRangeOfMap(x+dx,y+dy))
                    {
                        beforeTileTypes[(dx + 1) * 3 + (dy + 1)] = -10;
                    }
                    else
                    {
                        beforeTileTypes[(dx + 1) * 3 + (dy + 1)] = CalculateTileType(x + dx, y + dy);
                    }
                }
            }

            map.SetValue(vector, DateMGR.instance.groundID);

            //周囲9マスのタイルを更新する必要がある
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int i = (dx+1)*3+(dy+1); //3進法を利用

                    if (IsOutRangeOfMap(x+dx,y+dy))
                    {
                        afterTileTypes[i] = -10;
                    }
                    else
                    {
                        afterTileTypes[i] = CalculateTileType(x + dx, y + dy);
                        if (dx == 0 && dy == -1)
                        {
                            Debug.LogError($"CalculateTileType({x + dx}, {y + dy})={CalculateTileType(x + dx, y + dy)}");

                        }
                    }

                    if (!(beforeTileTypes[i]==afterTileTypes[i]) || (dx==0&&dy==0))
                    {
                        SetTileAccordingToValues(x + dx, y + dy);

                    }
                    else
                    {
                    }
                    Debug.LogWarning($"EqualsCalculateTileType(beforeTileTypes[i], afterTileTypes[i])が{(beforeTileTypes[i] == afterTileTypes[i])}です\n(dx,dy)=({dx},{dy})");
                    Debug.LogWarning($"(beforeTileTypes[i], afterTileTypes[i])=({beforeTileTypes[i]}, {afterTileTypes[i]})");


                }
            }
            Debug.LogWarning("########");
        }
    }

    //private bool equalscalculatetiletype(int beforetiletype ,int aftertiletype)
    //{
    //    if (beforetiletype == aftertiletype)
    //    {
    //        return true;
    //    }
    //    else if ((47 <= beforetiletype && beforetiletype <= 49) && (47 <= aftertiletype && aftertiletype <= 49)) //すべての辺が壁に接している壁タイルは三種類ある
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    private bool IsOutRangeOfMap(int x, int y)
    {
        if (x < 0 || y < 0 || x > map.Width || y > map.Height)
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
        FillAll(DateMGR.instance.wallID);
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
            Debug.LogError("領域外の値を取得しようとしました");
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



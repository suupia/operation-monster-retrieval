using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRouteData
{
    int _width;
    int _height;
    int[] _values = null;
    int _initiValue = -10;
    int _wallValue = -1;
    int _errorValue = -88;

    List<Vector2Int> autoRouteList;

    //コンストラクタ
    public AutoRouteData(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("RouteSearchDataの幅または高さが0以下になっています");
            return;
        }
        _width = width;
        _height = height;
        _values = new int[width * height];

        FillAll(_initiValue); //mapの初期化は_initiValueで行う
    }

    //プロパティ
    public int Width { get { return _width; } }
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
            return _errorValue;
        }
        if (IsOnTheEdge(x, y))
        {
            Debug.Log($"IsOnTheEdge({x},{y})がtrueです");
            return _wallValue;
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
            return _errorValue;
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
    public void SetWall(int x, int y)
    {
        SetValue(x, y, _wallValue);
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

    bool IsOutOfRange(int x, int y)
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

    public void FillAll(int value) //edgeValueまでは書き換えられないことに注意
    {
        for (int j = 0; j < _height; j++)
        {
            for (int i = 0; i < _width; i++)
            {
                _values[ToSubscript(i, j)] = value;
            }
        }
    }

    public List<Vector2Int> SearchShortestRoute(Vector2Int startPos,Vector2Int targetPos)
    {
        Queue<Vector2Int> searchQue = new Queue<Vector2Int>();
        int i = 1; //1から始まることに注意
        bool isComplete = false;
        int maxDistance = 0;

        //何よりもまず初期化
        FillAll(_initiValue);

        //次にmapをコピーして、動けない場所を-1にする。
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (GameManager.instance.mapMGR.GetMap().GetValue(x, y) % GameManager.instance.wallID == 0)
                {
                    SetWall(x, y);
                }
            }
        }

        //動けるマスに数字を順番に振っていく
        Debug.Log($"WaveletSearchを実行します startPos:{startPos}");
        WaveletSearch();

        //先に配列をnewしておく
        autoRouteList = new List<Vector2Int>();

        //Debug.LogWarning($"autoRouteArray:{autoRouteArray}");
        //autoRouteArray.Add(new Vector2Int(10,10));
        //Debug.LogWarning($"autoRouteArray[0]:{autoRouteArray[0]}");

        //数字をもとにして最短ルートを配列に格納する
        Debug.Log($"StoreRouteAround({targetPos},{maxDistance})を実行します");
        StoreShortestRoute(targetPos, maxDistance);

        Debug.Log($"resultRouteList:{string.Join(",", autoRouteList)}");

        autoRouteList.Reverse(); //リストを反転させる
        return autoRouteList;

        ////////////////////////////////////////////////////////////////////
        
        //以下ローカル関数
        void WaveletSearch()
        {
            SetValue(startPos, 0); //startPosの部分だけ周囲の判定を行わないため、ここで個別に設定する
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

                if (i > 100) //無限ループを防ぐ用
                {
                    isComplete = true;
                    Debug.LogError("SearchShortestRouteのwhile文でループが100回行われてしまいました");
                }
            }
        }

        void StoreShortestRoute(Vector2Int centerPos, int distance)
        {

            if (distance < 0)
            {
                return; //0までQueに入れれば十分
            }

            Debug.Log($"dinstance:{distance}、maxDistance:{maxDistance}のため、distance == maxDistanceは{distance == maxDistance}");
            if (distance == maxDistance) //最終地点の周りだけ周囲8マスに判定を行う(9マス)
            {
                Vector2Int inspectPos;

                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        inspectPos = centerPos + new Vector2Int(x, y);
                        if (GetValue(inspectPos) == distance)
                        {
                            Debug.Log($"distance:{distance} == maxDistance:{maxDistance}のときの、resultRouteArray[{distance}] = {inspectPos}を実行します");
                            autoRouteList.Add(inspectPos);
                            StoreShortestRoute(inspectPos, distance - 1);
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"GetValue({centerPos})は{GetValue(centerPos)}、distance:{distance}");

                // 5 7 8
                // 3 * 6
                // 1 2 4 の優先順位で判定していく

                Vector2Int[] orderInDirectionArray = new Vector2Int[] { Vector2Int.left + Vector2Int.down, Vector2Int.down, Vector2Int.left, Vector2Int.right + Vector2Int.down, Vector2Int.left + Vector2Int.up, Vector2Int.right, Vector2Int.up, Vector2Int.right + Vector2Int.up };

                foreach (Vector2Int direction in orderInDirectionArray)
                {
                    if (GetValue(centerPos + direction) == distance && CanMoveDiagonally(centerPos, centerPos + direction))
                    {
                        Debug.Log($"distance:{distance} != maxDistance:{maxDistance}のときの、resultRouteArray[{distance}] = {centerPos + direction}を実行します");
                        autoRouteList.Add(centerPos + direction);
                        StoreShortestRoute(centerPos + direction, distance - 1);
                        break;
                    }
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
                    //Debug.Log($"centerPos:{centerPos},inspectPos:{inspectPos}のとき、CanMove(centerPos, inspectPos):{CanMove(centerPos, inspectPos)}");
                    if (GetValue(inspectPos) == _initiValue && CanMoveDiagonally(centerPos, inspectPos))
                    {
                        SetValue(inspectPos, i);
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
                if (GetValue(prePos.x + directionVector.x, prePos.y) == _wallValue)
                {
                    return false;
                }

                //垂直方向の判定
                if (GetValue(prePos.x, prePos.y + directionVector.y) == _wallValue)
                {
                    return false;
                }
            }

            return true;
        }

    }
}


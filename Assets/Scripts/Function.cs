using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Function
{

    public static List<Vector2Int> SearchShortestNonDiagonalRoute(Vector2Int startPos, Vector2Int endPos)  //startPosからendPosへの斜め移動なしの最短経路を返す（startPos、endPosどちらも含む）
    {
        int width = GameManager.instance.mapMGR.GetMapWidth();
        int height = GameManager.instance.mapMGR.GetMapHeight();
        int[] _values = null;  //2次元配列のように扱う
        int _initiValue = -10; //PlaceNumAroundで重複して数字を置かないようにするために必要
        int _wallValue = -1;   //wallのマス
        int _errorValue = -88;
        List<Vector2Int> shortestRouteList;

        Queue<Vector2Int> searchQue = new Queue<Vector2Int>();
        int n = 1; //1から始まることに注意!!
        bool isComplete = false;
        int maxDistance = 0;


        //引数が適切かどうかチェックする
        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("SearchShortestRouteの幅または高さが0以下になっています");
            return null;
        }
        if (GameManager.instance.mapMGR.GetMapValue(startPos) % GameManager.instance.wallID == 0)
        {
            Debug.LogWarning("SearchShortestRouteのstatPosにwallIDが含まれています");
            return null;
        }
        if (GameManager.instance.mapMGR.GetMapValue(endPos) % GameManager.instance.wallID == 0)
        {
            Debug.LogWarning("SearchShortestRouteのendPosにwallIDが含まれています");
            return null;
        }


        //初期化
        _values = new int[width * height];
        shortestRouteList = new List<Vector2Int>();
        FillAll(_initiValue); //mapの初期化は_initiValueで行う


        //次にmapをコピーして、壁のマスを-1にする。
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (GameManager.instance.mapMGR.GetMap().GetValue(x, y) % GameManager.instance.wallID == 0)
                {
                    SetValue(x, y, _wallValue);
                }
            }
        }

        //壁でないマスに数字を順番に振っていく
        Debug.Log($"WaveletSearchを実行します startPos:{startPos}");
        WaveletSearch();


        //デバッグ用
        string debugCell = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                debugCell += $"{GetValue(x, height - y - 1)}".PadRight(3) + ",";
            }
            debugCell += "\n";
        }
        Debug.Log($"WaveletSearchの結果は\n{debugCell}");



        //数字をもとに、大きい数字から巻き戻すようにして最短ルートを配列に格納する
        Debug.Log($"StoreRouteAround({endPos},{maxDistance})を実行します");
        StoreShortestRoute(endPos, maxDistance);

        shortestRouteList.Reverse(); //リストを反転させる


        //デバッグ
        //Debug.Log($"shortestRouteList:{string.Join(",", shortestRouteList)}");
        GameManager.instance.debugMGR.DebugRoute(shortestRouteList);

        return shortestRouteList;

        ////////////////////////////////////////////////////////////////////

        //以下ローカル関数


        void WaveletSearch()
        {
            SetValueByVector(startPos, 0); //startPosの部分だけ周囲の判定を行わないため、ここで個別に設定する
            searchQue.Enqueue(startPos);

            while (!isComplete)
            {
                int loopNum = searchQue.Count; //前のループでキューに追加された個数を数える
                //Debug.Log($"i:{n}のときloopNum:{loopNum}");
                for (int k = 0; k < loopNum; k++)
                {
                    if (isComplete) break;
                    //Debug.Log($"PlaceNumAround({searchQue.Peek()})を実行します");
                    PlaceNumAround(searchQue.Dequeue());
                }
                n++; //前のループでキューに追加された文を処理しきれたら、インデックスを増やして次のループに移る

                if (n > 100) //無限ループを防ぐ用
                {
                    isComplete = true;
                    Debug.Log("SearchShortestRouteのwhile文でループが100回行われてしまいました");
                }
            }
        }

        void StoreShortestRoute(Vector2Int centerPos, int distance) //再帰的に呼ぶ
        {

            if (distance < 0) return; //0までQueに入れれば十分

            //Debug.Log($"GetValue({centerPos})は{GetValueFromVector(centerPos)}、distance:{distance}");

            // 1 3 5
            // 2 * 7
            // 4 6 8 の優先順位で判定していく

            Vector2Int[] orderInDirectionArray = new Vector2Int[] { Vector2Int.left + Vector2Int.up, Vector2Int.left, Vector2Int.up, Vector2Int.left + Vector2Int.down, Vector2Int.right + Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.right + Vector2Int.down };


            foreach (Vector2Int direction in orderInDirectionArray)
            {
                if (GetValueFromVector(centerPos + direction) == distance - 1 && CanMoveDiagonally(centerPos, centerPos + direction))
                {
                    shortestRouteList.Add(centerPos);
                    StoreShortestRoute(centerPos + direction, distance - 1);
                    break;
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

                    if (x != 0 && y != 0) continue; //斜めのマスの判定は飛ばす

                    inspectPos = centerPos + new Vector2Int(x, y);
                    //Debug.Log($"centerPos:{centerPos},inspectPos:{inspectPos}のとき、CanMove(centerPos, inspectPos):{CanMove(centerPos, inspectPos)}");
                    if (GetValueFromVector(inspectPos) == _initiValue && CanMoveDiagonally(centerPos, inspectPos))
                    {
                        SetValueByVector(inspectPos, n);
                        searchQue.Enqueue(inspectPos);
                        //Debug.Log($"({inspectPos})を{n}にし、探索用キューに追加しました。");
                    }
                    else  //このelseはデバッグ用
                    {
                        //Debug.Log($"{inspectPos}は初期値が入っていない　または　斜め移動でいけません\nGetValueFromVector({inspectPos}):{GetValueFromVector(inspectPos)}, CanMoveDiagonally({centerPos}, {inspectPos}):{CanMoveDiagonally(centerPos, inspectPos)}");
                    }

                    if (inspectPos == endPos && CanMoveDiagonally(centerPos, inspectPos))
                    {
                        isComplete = true;
                        SetValueByVector(inspectPos, n);
                        maxDistance = n;
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


        //Getter
        int GetValue(int x, int y)
        {
            if (IsOutOfRange(x, y))
            {
                Debug.LogError($"領域外の値を取得しようとしました (x,y):({x},{y})");
                return _errorValue;
            }
            if (IsOnTheEdge(x, y))
            {
                Debug.Log($"IsOnTheEdge({x},{y})がtrueです");
                return _wallValue;
            }
            return _values[ToSubscript(x, y)];
        }
        int GetValueFromVector(Vector2Int vector) //ローカル関数はオーバーライドができないことに注意
        {
            return GetValue(vector.x, vector.y);
        }


        //Setter
        void SetValue(int x, int y, int value)
        {
            if (IsOutOfRange(x, y))
            {
                Debug.LogError($"領域外に値を設定しようとしました (x,y):({x},{y})");
                return;
            }
            _values[ToSubscript(x, y)] = value;
        }
        void SetValueByVector(Vector2Int vector, int value)
        {
            SetValue(vector.x, vector.y, value);
        }

        //添え字を変換する
        int ToSubscript(int x, int y)
        {
            return x + (y * width);
        }

        bool IsOutOfRange(int x, int y)
        {
            if (x < -1 || x > width) { return true; }
            if (y < -1 || y > height) { return true; }

            //mapの中
            return false;
        }
        bool IsOnTheEdge(int x, int y)
        {
            if (x == -1 || x == width) { return true; }
            if (y == -1 || y == height) { return true; }
            return false;
        }

        void FillAll(int value) //edgeValueまでは書き換えられないことに注意
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    _values[ToSubscript(i, j)] = value;
                }
            }
        }

    }

    public static List<Vector2Int> SearchShortestDiagonalRoute(Vector2Int startPos, Vector2Int endPos)  //startPosからendPosへの斜め移動蟻の最短経路を返す（startPos、endPosどちらも含む）
    {
        return ConvertToDiagonalRoute(SearchShortestNonDiagonalRoute(startPos, endPos));
    }

    public static List<Vector2Int> SearchShortestRouteToCastle(Vector2Int startPos)
    {
        Vector2Int castlePos = GameManager.instance.mapMGR.GetEnemysCastlePos();
        Vector2Int endPos = Vector2Int.zero;

        if (GameManager.instance.mapMGR.GetMapValue(castlePos) % GameManager.instance.enemyCastleID != 0)
        {
            Debug.LogWarning("SearchShortestRouteToCastleのendPosにfacilityが含まれていません");
            return null;
        }

        // 1 3 5
        // 2 * 7
        // 4 6 8 の優先順位でendPosの周り判定していく

        Vector2Int[] orderInDirectionArray = new Vector2Int[] { Vector2Int.left + Vector2Int.up, Vector2Int.left, Vector2Int.up, Vector2Int.left + Vector2Int.down, Vector2Int.right + Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.right + Vector2Int.down };


        foreach (Vector2Int direction in orderInDirectionArray)
        {
            if (GameManager.instance.mapMGR.GetMapValue(castlePos + direction) % GameManager.instance.groundID == 0)
            {
                endPos = castlePos + direction;
            }
        }

        if (endPos == Vector2Int.zero)
        {
            Debug.LogError($"castlePos:{castlePos}の周りにgroundIDを含むマスがありません");
            return null;
        }

        return SearchShortestDiagonalRoute(startPos, endPos);
    }

    private static List<Vector2Int> ConvertToDiagonalRoute(List<Vector2Int> nonDiagonalRoute)
    {

        for (int i = 0; i < nonDiagonalRoute.Count; i++)
        {
            if (i >= nonDiagonalRoute.Count - 2) break; //nonDiagonalRoute.Count - 3まで判定をする

            Vector2Int gridPos = nonDiagonalRoute[i];
            Vector2Int nextPos = nonDiagonalRoute[i + 1];
            Vector2Int nextNextPos = nonDiagonalRoute[i + 2];

            if ((((nextPos - gridPos).x == 0 && (nextNextPos - nextPos).y == 0) || ((nextPos - gridPos).y == 0 && (nextNextPos - nextPos).x == 0)) && CanMoveDiagonally(gridPos, nextNextPos))
            {
                Debug.Log($"斜め移動が可能なため、nextPos:{nextPos}をnextNextPos:{nextNextPos}で置き換えます gridPos:{gridPos}");

                // nextPos = nextNextPosに対応する
                nonDiagonalRoute[i + 1] = nonDiagonalRoute[i + 2];
                nonDiagonalRoute.RemoveAt(i + 1);
                i++;
            }
        }

        return nonDiagonalRoute;


        //以下ローカル関数
        bool CanMoveDiagonally(Vector2Int prePos, Vector2Int afterPos)
        {
            Vector2Int directionVector = afterPos - prePos;

            //斜め移動の時にブロックの角を移動することはできない
            if (directionVector.x != 0 && directionVector.y != 0)
            {
                //水平方向の判定
                if (GameManager.instance.mapMGR.GetMapValue(prePos.x + directionVector.x, prePos.y) % GameManager.instance.wallID == 0)
                {
                    return false;
                }

                //垂直方向の判定
                if (GameManager.instance.mapMGR.GetMapValue(prePos.x, prePos.y + directionVector.y) % GameManager.instance.wallID == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }



    public static bool isWithinTheAttackRange(Vector2Int gridPos, int attackRange, int targetID, out Vector2Int targetPos) //最も近い攻撃対象の座標を返す （存在しないときはVector2Int.zeroを返す）
    {
        //攻撃できるかどうかでもう一度ループを回す必要があるため、攻撃できるかどうかと、最も近いターゲットの座標を取得するのを同時に行っている

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
                //Debug.Log($"nearestTargetList[0]={nearestTargetList[0]}");
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
    private static int[,] CalcSearchRangeArray(int advancingDistance, int lookingForValue, int notLookingForValue, int centerValue) //中心からadvancingDstanceだけ進んで（斜め移動はなし）隣接できる外側のマスをlookinForValueにした2次元配列を返す　lookinForValueはドーナッツ状に入る
    {
        int t = lookingForValue; //索敵範囲の値
        int f = notLookingForValue; //索敵範囲外の値
        int o = centerValue; //原点の値

        int size = 2 * (advancingDistance + 1) + 1;
        int[,] resultArray = new int[size, size];

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (i + j == advancingDistance || i + j == advancingDistance + 1 //左下
                   || i - j == advancingDistance + 1 || i - j == advancingDistance + 2 //右下
                   || -i + j == advancingDistance + 1 || -i + j == advancingDistance + 2 //左上
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


}


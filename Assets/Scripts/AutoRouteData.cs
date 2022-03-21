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
    public int GetValue(Vector2Int vector)
    {
        return GetValue(vector.x, vector.y);
    }
    public int GetValue(int index)
    {
        if (index < 0 || index > _values.Length)
        {
            Debug.LogError($"領域外の値を習得しようとしました index:{index}");
            return _errorValue;
        }
        return _values[index];
    }

    //Setter
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"領域外に値を設定しようとしました (x,y):({x},{y})");
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

    public List<Vector2Int> SearchShortestRoute(Vector2Int startPos, Vector2Int targetPos)
    {
       return Function.DiagonalSearchShortestRoute(startPos,targetPos);
    }
}


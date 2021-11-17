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


    public void SetupMap() //�����GameManager����Ă�
    {
        //������
        map = null;
        map = new MapDate(mapWidth,mapHeight); //map��1�����Ȃ��̂łƂ肠�����Anumber��0�Ƃ��Ă���

        RenderMap();
    }
    private void RenderMap()
    {
        //�}�b�v���N���A����i�d�����Ȃ��悤�ɂ���j
        tilemap.ClearAllTiles();

        for (int y = -outOfStageQ; y < map.Height + outOfStageQ; y++)
        {
            for (int x = -outOfStageQ; x < map.Width + outOfStageQ; x++)
            {
                SetTileAccordingToValues(new Vector2Int(x, y));
            }
        }

    }
    private void SetTileAccordingToValues(Vector2Int vector)
    {
        int x = vector.x;
        int y = vector.y;

        if (0 <= y && y < map.Height && 0 <= x && x < map.Width)
        {
            // 1 = �^�C������A0 = �^�C���Ȃ�
            if (map.GetValue(x,y) == DateMGR.instance.wallID)
            {
                //tilemap�̓^�C���}�b�v�S�̂̂���(Tilemap)�Btile�͌X�̃^�C���̂���(Tile)
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[CalculateTileType(x, y)]);
                //Debug.Log($"�^�C����{x},{y}�ɕ~���l�߂܂���");
            }
            else if (map.GetValue(x, y) == DateMGR.instance.groundID)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[UnityEngine.Random.Range(50, 52 + 1)]);
            }

        }
        else //map�̗̈�O
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[UnityEngine.Random.Range(47, 49 + 1)]); //�S�������ǂ̃^�C���𒣂�(3��)
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

        //�[�̃^�C���͐�ɏ�������
        if (y == 0 || x == 0 || y == mapHeight - 1 || x == mapWidth - 1)
        {
            return UnityEngine.Random.Range(47, 49 + 1);//�ő�l�͊܂܂Ȃ����Ƃɒ���
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
        //�ǂƐڂ��Ȃ��Ƃ�
        if (!upIsWall && !leftIsWall && !downIsWall && !rightIsWall)
        {
            return 1;
        }

        //1�̕ǂƐڂ���Ƃ�
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

        //2�̕ǂƐڂ���Ƃ�
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

        //3�̕ǂƐڂ���Ƃ�
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

        //4�̕ǂƐڂ���Ƃ�
        if (upIsWall && leftIsWall && downIsWall && rightIsWall)
        {
            //���͂ɒn�ʂ���ł�����Ƃ�
            if (upleftWallValue == 0 || downleftWallValue == 0 || downrightWallValue == 0 || uprightWallValue == 0)
            {
                binarySub = 0;
                binarySub = upleftWallValue + 2 * downleftWallValue + 4 * downrightWallValue + 8 * uprightWallValue;
                return 32 + binarySub;
            }
            else   //���͂����ׂĕǂ̎��̓^�C���̎�ނ𗐐��Ō��߂�
            {
                return UnityEngine.Random.Range(47, 49 + 1);//�ő�l�͊܂܂Ȃ����Ƃɒ���
            }

        }

        Debug.LogError($"CalculateTileNum({y} ,{x})�Ɏ��s���܂���");
        return -100;
    }

    public void MakeRoad(Vector2Int vector)
    {
        if (map.GetValue(vector) == DateMGR.instance.wallID)
        {
            map.SetValue(vector, DateMGR.instance.groundID);

            //����9�}�X�̃^�C�����X�V����K�v������
            for(int y = -1; y <= 1; y++)
            {
                for (int x = -1; x<= 1;x++)
                {
                    SetTileAccordingToValues(new Vector2Int(vector.x +x,vector.y+y));
                }
            }
        }
    }



}

public class MapDate 
{
    protected int _width;
    protected int _height;
    protected int[] _values = null;
    protected int _outOfRange = -1;

    //�R���X�g���N�^
    public MapDate(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Map�̕��܂��͍�����0�ȉ��ɂȂ��Ă��܂�");
            return;
        }
        _width = width;
        _height = height;
        _values = new int[width * height];

        //map�̏������͂P�ōs���B���Ƃ��畔���⓹���󂯂Ă����B
        FillAll(DateMGR.instance.wallID);
    }

    //�v���p�e�B
    public int Width { get { return _width; } } //�e�N���X�̃����o���g�p
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
            Debug.LogError("�̈�O�̒l���擾���悤�Ƃ��܂���");
            return _outOfRange;
        }
        return _values[ToSubscript(x, y)];
    }
    public int GetValue(Vector2Int vector)
    {
        return GetValue(vector.x,vector.y);
    }
    public int GetValue(int index)
    {
        if (index < 0 || index > _values.Length)
        {
            Debug.LogError("�̈�O�̒l���K�����悤�Ƃ��܂���");
            return _outOfRange;
        }
        return _values[index];
    }

    //Setter
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError("�̈�O�ɒl��ݒ肵�悤�Ƃ��܂���");
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

    //�Y������ϊ�����
    protected int ToSubscript(int x, int y)
    {
        return x + (y * _width);
    }

    public Vector2Int DivideSubscript(int subscript)
    {
        int xSub = subscript % _width;
        int ySub = (subscript - xSub) / _width; //�����͊���Z
        return new Vector2Int(xSub, ySub);
    }

    protected bool IsOutOfRange(int x, int y)
    {
        if (x < 0 || x >= _width) { return true; }
        if (y < 0 || y >= _height) { return true; }

        //�̈�̒�
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

 

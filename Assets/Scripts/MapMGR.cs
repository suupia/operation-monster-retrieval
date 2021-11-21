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


    public void SetupMap() //�����GameManager����Ă�
    {
        //������
        map = null;
        map = new MapDate(mapWidth, mapHeight); //map��1�����Ȃ��̂łƂ肠�����Anumber��0�Ƃ��Ă���

        RenderMap();

        PlaceCastle();

        PlaceTower();
    }
    private void RenderMap()
    {
        //�}�b�v���N���A����i�d�����Ȃ��悤�ɂ���j
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
        Instantiate(allysCastlePrefab, new Vector3(allysCastlePos.x + 1, allysCastlePos.y + 1, 0), Quaternion.identity); //�摜�̒��S���i�q�_�ɂ���悤�ɁA+1���Ă��邱�Ƃɒ���
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
            // 1 = �^�C������A0 = �^�C���Ȃ�
            if (map.GetValue(x, y) == GameManager.instance.wallID)
            {
                if (CalculateTileType(x, y) < 47) //���肪���ׂĕǂ̃^�C����3��ނ���
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[CalculateTileType(x, y)]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[UnityEngine.Random.Range(47, 49 + 1)]);
                }
                //Debug.Log($"�^�C����{x},{y}�ɕ~���l�߂܂���");
            }
            else if (map.GetValue(x, y) == GameManager.instance.groundID)
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
        bool upIsWall = false;
        bool leftIsWall=false;
        bool downIsWall=false;
        bool rightIsWall=false;
        int upleftWallValue =0; //1�̂Ƃ�wall�����邱�Ƃ�\��
        int downleftWallValue=0;
        int downrightWallValue=0;
        int uprightWallValue=0;
        int binarySub;

        if (IsOutRangeOfMap(x, y))
        {
            Debug.LogError($"CalculateTileType({x},{y})�̈�����map�͈̔͊O���w�肳��܂���");
            return -100;
        }

        //��������groundID�̎���0��Ԃ��悤�ɂ���i�����RenderMap�ł͎g��Ȃ��j
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

        //    if (y == 0) //else if�ɂ��Ȃ��̂͏������d�����鋰�ꂪ���邽��
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

        
            if (y== map.Height-1 ||map.GetValue(x, y + 1) == GameManager.instance.wallID) //�����̏������̕�����ɔ��肳���̂ŁAmap�͈̔͊O��GetValue���邱�Ƃ͂Ȃ��i�Ǝv���j
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


            if (x==0|| y == map.Height-1||map.GetValue(x - 1, y + 1) == GameManager.instance.wallID) //����4�̏ꍇ������4���𒲂ׂ�΂悢�̂ŁAx�����̔���ŏ\��
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
            else   //���͂����ׂĕǂ̎��̓^�C���̎�ނ����Ƃŗ����Ō��߂�
            {
                return 47;
            }

        }

        Debug.LogError($"CalculateTileNum({y} ,{x})�Ɏ��s���܂���");
        return -100;
    }
    public void MakeRoad(int x, int y)
    {
        Vector2Int vector = new Vector2Int(x, y);

        if (IsOutRangeOfMap(x, y))
        {
            Debug.Log($"MakeRoad({x},{y})�̈�����map�͈̔͊O���w�肳��܂���");
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

            //����9�}�X�̃^�C�����X�V����K�v������
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int i = (dx + 1) * 3 + (dy + 1); //3�i�@�𗘗p

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
                        //TileType���ω����Ă��Ȃ�������A�^�C���͒���ւ��Ȃ�
                    }
                    //Debug.LogWarning($"EqualsCalculateTileType(beforeTileTypes[i], afterTileTypes[i])��{(beforeTileTypes[i] == afterTileTypes[i])}�ł�\n(dx,dy)=({dx},{dy})");
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
        FillAll(GameManager.instance.wallID);
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
            Debug.LogError($"�̈�O�̒l���擾���悤�Ƃ��܂���(x,y)=({x},{y})");
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



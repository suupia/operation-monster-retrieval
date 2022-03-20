using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapMGR : MonoBehaviour
{
[System.Serializable]
    class StageTileArray //�C���X�y�N�^�[��ŃZ�b�g�ł���悤�ɂ��邽�߂ɃN���X�����
    {
        [SerializeField] public TileBase[] stageTileArray;
    }
    [SerializeField] Tilemap tilemap;
    [SerializeField] StageTileArray[]  tileArray;
    [SerializeField] int stageNum; //�X�e�[�W�̔ԍ��ɂ���Ďg���^�C���}�b�v�����߂�i0����X�^�[�g�j���ƌo���l�̌���ɂ��g��

    MapData map;

    [SerializeField] int mapHeight;
    [SerializeField] int mapWidth;
    [SerializeField] int outOfStageQ; //�}�b�v�̊O�Ƀ^�C���������͂ݏo���Ē��邩�����߂�

    [SerializeField] GameObject allysCastlePrefab;
    [SerializeField] Vector2Int allysCastlePos;
    [SerializeField] bool isFristTimePlaceAllyCastle = true; //�����̏�ɂ̓X�N���v�g�����Ă��Ȃ����߁ADestroy�ł��Ȃ��@���̂��߁Abool�ň�񂾂��z�u�����悤�ɂ���
    [SerializeField] Vector2Int[] characterSpawnPossFromCastle;
    [System.NonSerialized] public Vector2Int[] characterSpawnPoss;
    [SerializeField] GameObject enemysCastlePrefab;
    [SerializeField] Vector2Int enemysCastlePos;

    [SerializeField] GameObject[] towerPrefabs;
    int maxTowerNum; //PlaceTower()�Ō��肷��

    [SerializeField] int numOfFristRoad; //�C���X�y�N�^�[��ȊO�Œl�������Ă͂����Ȃ�
    [SerializeField] int numOfFristRoadCounter; //SetUpMap()�� numOfFristRoadCounter = numOfFristRoad; �Ə��������Ă���  �f�o�b�O�p��SerializeField�ɂ��Ă���
    [SerializeField] public GameObject makeTheFirstRoadGO; //GameManager��SetActive��ς���
    Text makeTheFirstRoadText;
    bool isDisplayMakefristRoadAgainCoroutine = false;

    [SerializeField] int[] EXPOfTheStage; //�N���A���ɂ��炦��o���l���C���X�y�N�^�[��Ō��߂�

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
            Debug.LogError($"DivisionalSetMapValue({vector},{value})��map({vector})�̒l��{value}�Ŋ���؂邱�Ƃ��ł��܂���ł����B");
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

    public void SetupMap() //�����GameManager����Ă�
    {
        //������
        map = null;
        map = new MapData(mapWidth, mapHeight); //map��1�����Ȃ��̂łƂ肠�����Anumber��0�Ƃ��Ă���

        numOfFristRoadCounter = GetNumOfFristRoad();

        makeTheFirstRoadText = makeTheFirstRoadGO.GetComponent<Text>();
        makeTheFirstRoadText.text = $"�G�̏�ɂȂ���悤��\n����" + $"{numOfFristRoadCounter}".PadLeft(2) + "����z�u���Ă�������"; //�\�������Z�b�g����


        //Debug.Log($"numOfFristRoadCounter��{GetNumOfFristRoad()}�ɏ��������܂���");

        RenderMap();

        PlaceCastle();

        PlaceTower();
    }
    private void ReSetupMap() //MakeTheFirstRoad�Ŏ��s�����Ƃ��ɌĂ΂��
    {
        for (int x =0;x<map.Width;x++)
        {
            for(int y = 0; y < map.Height; y++)
            {
                if(map.GetValue(x,y) % GameManager.instance.groundID == 0) //MakeTheFirstRoad�ō��������ǂɖ߂�
                {
                    map.SetValue(x, y,GameManager.instance.wallID); //���ڒl�������Ă��邱�Ƃɒ���
                }
            }
        }

        numOfFristRoadCounter = GetNumOfFristRoad();

        makeTheFirstRoadText = makeTheFirstRoadGO.GetComponent<Text>();
        makeTheFirstRoadText.text = $"�G�̏�ɂȂ���悤��\n����" + $"{numOfFristRoadCounter}".PadLeft(2) + "����z�u���Ă�������"; //�\�������Z�b�g����


        //Debug.Log($"numOfFristRoadCounter��{GetNumOfFristRoad()}�ɏ��������܂���");

        RenderMap();

        //SetupMap()�ɔ�ׂāAPlaceCastle()��PlaceTower()���Ȃ�
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
        if (isFristTimePlaceAllyCastle)
        {
            isFristTimePlaceAllyCastle = false;
            Instantiate(allysCastlePrefab, new Vector3(allysCastlePos.x, allysCastlePos.y + 1, 0), Quaternion.identity); //�摜�̒��S���i�q�_�ɂ���悤�ɁA+1���Ă��邱�Ƃɒ���
        }
        GameObject enemyCastleGO = Instantiate(enemysCastlePrefab, new Vector3(enemysCastlePos.x + 1, enemysCastlePos.y, 0), Quaternion.identity);
        CastleMGR enemyCastleMGR = enemyCastleGO.GetComponent<CastleMGR>();

        map.MultiplySetValue(enemysCastlePos, GameManager.instance.facilityID); //���l�f�[�^���Z�b�g
        map.SetFacility(enemysCastlePos, enemyCastleMGR); //�X�N���v�g���Z�b�g

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
        maxTowerNum = 0; //������

        for (int y = 0;y< mapHeight;y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int towerType = GameManager.instance.towerPosDataMGR.GetTowerPosDataArray()[stageNum].posData[y * mapWidth + x]; //(x,y)�ɑΉ�����json�t�@�C���ɏ����Ă���l
                if (towerType != -1) //-1�łȂ��Ȃ�Γ����Ă��鐔���ɉ�����   �^���[�̎�ނƈʒu�����߂�
                {
                    if (towerType >= towerPrefabs.Length) //�����Ă��鐔�����K�؂��ǂ����`�F�b�N����
                    {
                        Debug.LogError($"towerPosDataArray�Ƀ^���[�̎�ވȏ�̒l�������Ă��܂�");
                        return;
                    }

                    maxTowerNum++;

                    Vector2Int towerSpawnPos = new Vector2Int(x, y);

                    GameObject towerGO = Instantiate(towerPrefabs[towerType], new Vector3(towerSpawnPos.x + 0.5f, towerSpawnPos.y + 0.75f, 0), Quaternion.identity);
                    TowerMGR towerMGR = towerGO.GetComponent<TowerMGR>();

                    map.MultiplySetValue(towerSpawnPos, GameManager.instance.facilityID); //���l�f�[�^���Z�b�g
                    map.SetFacility(towerSpawnPos, towerMGR); //�X�N���v�g���Z�b�g

                }
            }
        }

    }


    private void SetTileAccordingToValues(int x, int y)
    {

        if (0 <= y && y < map.Height && 0 <= x && x < map.Width)
        {
            // 1 = �^�C������A0 = �^�C���Ȃ�
            if (map.GetValue(x, y) % GameManager.instance.wallID == 0)
            {
                if (CalculateTileType(x, y) < 47) //���肪���ׂĕǂ̃^�C����3��ނ���
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[CalculateTileType(x, y)]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]);
                }
                //Debug.Log($"�^�C����{x},{y}�ɕ~���l�߂܂���");
            }
            else if (map.GetValue(x, y) % GameManager.instance.groundID == 0)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(50, 52 + 1)]);
            }

        }
        else //map�̗̈�O
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tileArray[stageNum].stageTileArray[UnityEngine.Random.Range(47, 49 + 1)]); //�S�������ǂ̃^�C���𒣂�(3��)
        }
    }
    private int CalculateTileType(int x, int y)
    {
        bool upIsWall = false;
        bool leftIsWall = false;
        bool downIsWall = false;
        bool rightIsWall = false;
        int upleftWallValue = 0; //1�̂Ƃ�wall�����邱�Ƃ�\��
        int downleftWallValue = 0;
        int downrightWallValue = 0;
        int uprightWallValue = 0;
        int binarySub;

        if (IsOutRangeOfMap(x, y))
        {
            Debug.LogError($"CalculateTileType({x},{y})�̈�����map�͈̔͊O���w�肳��܂���");
            return -100;
        }

        //��������groundID�̎���0��Ԃ��悤�ɂ���i�����RenderMap�ł͎g��Ȃ��j
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


        if (y == map.Height - 1 || map.GetValue(x, y + 1) % GameManager.instance.wallID == 0) //�����̏������̕�����ɔ��肳���̂ŁAmap�͈̔͊O��GetValue���邱�Ƃ͂Ȃ��i�Ǝv���j
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


        if (x == 0 || y == map.Height - 1 || map.GetValue(x - 1, y + 1) % GameManager.instance.wallID == 0) //����4�̏ꍇ������4���𒲂ׂ�΂悢�̂ŁAx�����̔���ŏ\��
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
            //Debug.Log($"MakeRoad({x},{y})�̈����Ō@���͈͂̊O���w�肳��܂����B");
            return;
        }

        if (GameManager.instance.energyMGR.CurrentEnergy < GameManager.instance.energyMGR.EnergyToMakeRoad && GameManager.instance.state == GameManager.State.RunningGame)
        {
            //Debug.Log("�G�l���M�[������Ȃ��̂œ������܂���");
            return;
        }
        if(GetMapValue(x,y) / GameManager.instance.wallID != 1) //���ʂ̊���Z�����Ă��邱�Ƃɒ���
        {
            //Debug.Log("�ǂ݂̂̃}�X�łȂ��̂�MakeROadByPointer�𒆒f���܂�");
            return;
        }
        if (GameManager.instance.state == GameManager.State.RunningGame) //MakeTheFirstRoad�̎��̓G�l���M�[������Ȃ�
        {
            GameManager.instance.energyMGR.CurrentEnergy -= GameManager.instance.energyMGR.EnergyToMakeRoad;
        }
        if (GameManager.instance.state == GameManager.State.MakeTheFirstRoad && map.GetValue(x,y) == GameManager.instance.wallID) //MakeTheFirstRoad�̏���
        {
            numOfFristRoadCounter--;
            //Debug.Log($"numOfFristRoadCounter��{numOfFristRoadCounter}�ɂȂ�܂���");

            if (isDisplayMakefristRoadAgainCoroutine) return; //�R���[�`�����쓮���Ă���Ԃ͈ȉ��̏������s��Ȃ�

            makeTheFirstRoadText.text = $"�G�̏�ɂȂ���悤��\n����" + $"{numOfFristRoadCounter}".PadLeft(2) + "����z�u���Ă�������";
            if (numOfFristRoadCounter <= 0)
            {
                //Debug.Log("numOfFristRoadCounter��0�ȉ��ɂȂ�܂���");
                if (IsReachable2())
                {
                    //Debug.Log($"GameManager��State��RunningGame�ɂ��܂�");
                    GameManager.instance.RunningGame();
                }
                else
                {
                    //Debug.Log("�����G�̏�ɂȂ����Ă��Ȃ�����\n��蒼���Ă�������");
                    makeTheFirstRoadText.text = $"�����G�̏�ɂȂ����Ă��Ȃ�����\n��蒼���Ă�������";
                    StartCoroutine(DisplayMakefristRoadAgainCoroutine());
                }

            }
        }

        MakeRoad(x,y);

    }
    public void MakeRoadByTowerDead(int x, int y) //�^���[���j�󂳂ꂽ�Ƃ��ɌĂ�
    {
        if (x < 1 || y < 1 || x > map.Width - 2 || y > map.Height - 2)
        {
            //Debug.Log($"MakeRoad({x},{y})�̈����Ō@���͈͂̊O���w�肳��܂����B");
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
    IEnumerator DisplayMakefristRoadAgainCoroutine()
    {
        isDisplayMakefristRoadAgainCoroutine = true;
        //�����ɓ��B�����Ƃ��AGameManager.instance.state��State.MakeTheFirstRoad
        yield return new WaitForSeconds(1.2f);
        //�R���[�`���̌���AState.MakeTheFirstRoad�̂܂܁i�Ȃ��Ȃ�΁A���̑J�ڏ�Ԃ�RuuningGame�����AMakeRoadByPointer()��MakeTheFirstRoad�̏�����IsReachable��if����true�̎��ɏ��߂Ĉڂ邩��j
        //GameManager.instance.SetupGame(); //SetupGame�ɖ߂��̂͂悭�Ȃ��B�J�ڏ�Ԃ�MakeTheFirstRoad�̂܂܃}�b�v�̍Ĕz�u���s��

        ReSetupMap();

        isDisplayMakefristRoadAgainCoroutine = false;
    }

    bool IsReachable()
    {
        int initiValue = -10; //PlaceNumAround�ŏd�����Đ�����u���Ȃ��悤�ɂ��邽�߂ɕK�v
        //int roadValue = 1; //road�̃}�X
        int wallValue = -1; //wall�̃}�X
        int _errorValue = -88;
        int[,] cell = new int[mapHeight,mapWidth]; //���B�ł��邩�ǂ����𔻒肷��p��2�����z��
        Vector2Int startPos = allysCastlePos;
        Vector2Int targetPos = enemysCastlePos;

        Queue<Vector2Int> searchQue = new Queue<Vector2Int>();
        int i = 1; //1����n�܂邱�Ƃɒ���
        bool isComplete = false;
        int maxDistance = 0;

        //������
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                    cell[y, x] = initiValue;  
            }
        }

        //map�̕ǂ̃}�X��wallValue�ɂ���B
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (map.GetValue(x, y) % GameManager.instance.wallID == 0)
                {
                    cell[y,x] = wallValue;  //�������ꂪinitiValue�݂����Ȗ���������
                }
            }
        }

        //������}�X�ɐ��������ԂɐU���Ă���
        Debug.Log($"WaveletSearch�����s���܂� startPos:{startPos}");
        WaveletSearch();

        //�f�o�b�O�p
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
        //Debug.Log($"cell�̒��g��\n{debugCell}");

        

        if (CanGetCloseToTheCastle())
        {
            Debug.Log("IsReachable��true�ł�");
            return true;
        }
        else
        {
            Debug.Log("IsReachable��false�ł�");
            return false; 
        }


        ////////////////////////////////////////////////////////////////////

        //�ȉ����[�J���֐�
        int GetValue(int x, int y)
        {
            if (IsOutRangeOfMap(x, y))
            {
                Debug.LogError($"�̈�O�ɒl���擾���悤�Ƃ��܂��� (x,y):({x},{y})");
                return _errorValue;
            }
            if (x == 0 || y == 0 || x == map.Width - 1 || y == map.Height - 1)
            {
                return wallValue; //�[�͕ǈ����ɂ���
            }
            return cell[y,x];
        }

        void SetValue(int x, int y, int value)
        {
            if (IsOutRangeOfMap(x, y))
            {
                Debug.LogError($"�̈�O�ɒl��ݒ肵�悤�Ƃ��܂��� (x,y):({x},{y})");
                return;
            }
            cell[y, x] = value; 
        }

        void WaveletSearch()
        {
            SetValue(startPos.x,startPos.y,0); //startPos�̕����������͂̔�����s��Ȃ����߁A�����Ōʂɐݒ肷�� 0�Ƃ��Ă���̂�0�Ԗڂ̃}�X������
            searchQue.Enqueue(startPos);

            while (!isComplete)
            {
                int loopNum = searchQue.Count; //�O�̃��[�v�ŃL���[�ɒǉ����ꂽ���𐔂���
                Debug.Log($"i:{i}�̂Ƃ�loopNum:{loopNum}");
                for (int k = 0; k < loopNum; k++)
                {
                    Debug.Log($"PlaceNumAround({searchQue.Peek()})�����s���܂�");
                    if (isComplete) break;
                    PlaceNumAround(searchQue.Dequeue());
                }
                i++; //�O�̃��[�v�ŃL���[�ɒǉ����ꂽ�������������ꂽ��A�C���f�b�N�X�𑝂₵�Ď��̃��[�v�Ɉڂ�

                if (i > 100) //�������[�v��h���p   100�܂ŒT�����ē���������Ȃ��̂Ȃ�΁A�����𒆒f����
                {
                    isComplete = true;
                    Debug.Log("SearchShortestRoute��while���Ń��[�v��100��s��ꂽ���ߏ������I�����܂�");
                }

            }
        }

        void PlaceNumAround(Vector2Int centerPos)
        {
            Vector2Int inspectPos;

            //8�}�X���肷��i�^�񒆂̃}�X�̔���͕K�v�Ȃ��j
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0) continue; //�^�񒆂̃}�X�͔�΂�

                    inspectPos = centerPos + new Vector2Int(x, y);

                    if (IsOutRangeOfMap(inspectPos.x, inspectPos.y)) continue; //map�O�̃}�X�̔���͔�΂�

                    //Debug.Log($"centerPos:{centerPos},inspectPos:{inspectPos}�̂Ƃ��ACanMove(centerPos, inspectPos):{CanMove(centerPos, inspectPos)}");
                    if (GetValue(inspectPos.x, inspectPos.y) == initiValue && CanMoveDiagonally(centerPos, inspectPos))
                    {
                        SetValue(inspectPos.x,inspectPos.y,i);
                        if(GetValue(inspectPos.x, inspectPos.y)==0) Debug.LogError($"GetValue({inspectPos.x}, {inspectPos.y})��0�ł�");

                        searchQue.Enqueue(inspectPos);
                        Debug.Log($"({inspectPos})��{i}�ɂ��A�T���p�L���[�ɒǉ����܂����B");
                    }
                    if (inspectPos == targetPos)
                    {
                        isComplete = true;
                        maxDistance = i - 1;
                        Debug.Log($"isComplete��true�ɂ��܂����BmaxDistance:{maxDistance}");
                        break; //�T���I��
                    }
                }
            }
        }

        bool CanMoveDiagonally(Vector2Int prePos, Vector2Int afterPos)
        {
            Vector2Int directionVector = afterPos - prePos;

            //�΂߈ړ��̎��Ƀu���b�N�̊p���ړ����邱�Ƃ͂ł��Ȃ�
            if (directionVector.x != 0 && directionVector.y != 0)
            {
                //���������̔���
                if (GetValue(prePos.x + directionVector.x, prePos.y) == wallValue)
                {
                    return false;
                }

                //���������̔���
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

            //8�}�X���肷��i�^�񒆂̃}�X�̔���͕K�v�Ȃ��j
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0) continue; //�^�񒆂̃}�X�͔�΂�

                    inspectPos = targetPos + new Vector2Int(x, y);

                    if (IsOutRangeOfMap(inspectPos.x, inspectPos.y)) continue; //map�O�̃}�X�̔���͔�΂�

                    if (GetValue(inspectPos.x, inspectPos.y) != initiValue && GetValue(inspectPos.x, inspectPos.y) != wallValue && CanMoveDiagonally(targetPos, inspectPos))
                     //�����̒l�ł��Ȃ��@���@�ǂ̒l�ł��Ȃ��@���@targetPos����inspectPos�ֈړ����邱�Ƃ��ł���
                    {
                        result = true;
                    }
                }
            }

            if (map.GetValue(allysCastlePos.x,allysCastlePos.y) % GameManager.instance.wallID == 0) //�����͖{����map���Q�Ƃ��Ē��ׂ�
            {
                Debug.LogWarning("�L�����N�^�[�̃X�|�[���n�_�ɓ����Ȃ�����false�ł�");
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
        var adjacentPosList = new List<Vector2Int>();//targetPos�ɗאڂ���A�ǂłȂ��}�X


        //targetPos�̎���8�}�X�𔻒肵�āAadjacentPosList�����߂�
        Vector2Int inspectPos;
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x == 0 && y == 0) continue; //�^�񒆂̃}�X�͔�΂�

                inspectPos = targetPos + new Vector2Int(x, y);

                if (IsOutRangeOfMap(inspectPos.x, inspectPos.y)) continue; //map�O�̃}�X�̔���͔�΂�

                if (map.GetValue(inspectPos.x, inspectPos.y) % GameManager.instance.wallID !=0)
                {
                    adjacentPosList.Add(inspectPos);
                }
            }
        }

        if(adjacentPosList.Count == 0)
        {
            Debug.Log("�G�̏�̎��ӂɕǂ̃}�X�����Ȃ�����IsReachable��false�ł�");
            return false;
        }

        if (map.GetValue(allysCastlePos) % GameManager.instance.wallID == 0)
        {
            Debug.Log("�L�����N�^�[�̃X�|�[���n�_���ǂ̂���false�ł�");
            result = false;
        }

        foreach (Vector2Int adjacentPos in adjacentPosList)
        {
            if (Function.SearchShortestNonDiagonalRoute(startPos, adjacentPos) == null)
            {
                result = false;
                continue;
            }

            if (Function.SearchShortestNonDiagonalRoute(startPos ,adjacentPos).Count >=(mapWidth-3)+(mapHeight-3)-1) //���Ȃ��Ƃ��ŒZ�ꍇ��胋�[�g�������Ȃ� (-3�͏邪����Ƃ���͕ǂ��m�肵�Ă��邱�Ƃɒ���)
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

    //�R���X�g���N�^
    public MapData(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Map�̕��܂��͍�����0�ȉ��ɂȂ��Ă��܂�");
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

        FillAll(GameManager.instance.wallID); //map�̏�������wallID�ōs��
    }

    //�v���p�e�B
    public int Width { get { return _width; } } //�e�N���X�̃����o���g�p
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
            Debug.LogError($"IsOutOfRange({x},{y})��true�ł�");
            return _outOfRangeValue;
        }
        if (IsOnTheEdge(x,y))
        {
            //Debug.Log($"IsOnTheEdge({x},{y})��true�ł�");
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
            Debug.LogError("�̈�O�̒l���K�����悤�Ƃ��܂���");
            return _outOfRangeValue;
        }
        return _values[index];
    }

    public List<CharacterMGR> GetCharacterMGRList(int x,int y)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})��true�ł�");
            return null; //��O�p�̐�����ݒ�ł��Ȃ����߁Anull��Ԃ�
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
            Debug.LogError("�̈�O�̒l���K�����悤�Ƃ��܂���");
            return null; //��O�p�̐�����ݒ�ł��Ȃ����߁Anull��Ԃ�
        }
        return _characterMGRs[index];
    }
    public Facility GetFacility(int x,int y)
    {
        if (IsOutOfDataRange(x, y))
        {
            Debug.LogError($"IsOutOfDataRange({x},{y})��true�ł�");
            return null; //��O�p�̐�����ݒ�ł��Ȃ����߁Anull��Ԃ��B
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
            Debug.LogError("�̈�O�̒l���K�����悤�Ƃ��܂���");
            return null; //��O�p�̐�����ݒ�ł��Ȃ����߁Anull��Ԃ�
        }
        return _facilities[index];
    }
    //Setter
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"IsOutOfRange({x},{y})��true�ł�");
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
            Debug.LogError($"IsOutOfDataRange({x},{y})��true�ł�");
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
            Debug.LogError($"IsOutOfDataRange({x},{y})��true�ł�");
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
            Debug.LogError($"IsOutOfDataRange({x},{y})��true�ł�");
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
            Debug.LogError($"MultiplySetValue({x},{y})�ŗ̈�O�ɒl{value}��ݒ肵�悤�Ƃ��܂���");
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
            Debug.LogError($"DivisionalSetValue({x},{y})�ŗ̈�O�ɒl{value}��ݒ肵�悤�Ƃ��܂���");
            return;
        }
        if (GetValue(x,y)% value !=0)
        {
            Debug.LogError($"DivisionalSetValue(vector:{vector},value:{value})�ŗ]�肪�o�����ߎ��s�ł��܂���");
            return;
        }

        _values[ToSubscript(x, y)] /= value;
    }



    //�Y������ϊ�����
    int ToSubscript(int x, int y)
    {
        return x + (y * _width);
    }

    public Vector2Int DivideSubscript(int subscript)
    {
        int xSub = subscript % _width;
        int ySub = (subscript - xSub) / _width; //�����͊���Z
        return new Vector2Int(xSub, ySub);
    }

    bool IsOutOfRange(int x, int y) //edge�̊O���B�܂�A�f�[�^��edgeValue����Ȃ�
    {
        if (x < -1 || x > _width) { return true; }
        if (y < -1 || y > _height) { return true; }

        //map�̒�
        return false;
    }

    bool IsOnTheEdge(int x, int y)
    {
        if (x == -1 || x == _width) { return true; }
        if (y == -1 || y == _height) { return true; }
        return false;
    }

    bool IsOutOfDataRange(int x,int y) //���W(0,0)�`(mapWidht-1,mapHeight-1)�̃f�[�^�����݂���̈�̊O��
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


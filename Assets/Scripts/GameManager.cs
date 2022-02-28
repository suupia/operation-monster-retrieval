using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] public MapMGR mapMGR; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] public InputMGR inputMGR;
    [SerializeField] public DebugMGR debugMGR;
    [SerializeField] public PointerMGR pointerMGR;
    [SerializeField] public SelectCharacterButtonMGR[] selectCharacterButtonMGR; //4�Z�b�g����
    [SerializeField] public TimerMGR timerMGR;
    [SerializeField] public EnergyMGR energyMGR;
    [SerializeField] public SaveMGR saveMGR;

    [SerializeField] public GameObject selectStageCanvas; //SetActive�ŕ\���𐧌䂷��̂ŃQ�[���I�u�W�F�N�g���Ǝ擾����K�v������ �C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] public GameObject menuCanvas; //SetActive�ŕ\���𐧌䂷��̂ŃQ�[���I�u�W�F�N�g���Ǝ擾����K�v������ �C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] public StatusCanvasMGR statusCanvasMGR;

    [SerializeField] GameObject resultCanvas; //SetActive�ŕ\���𐧌䂷��̂ŃQ�[���I�u�W�F�N�g���Ǝ擾����K�v������ �C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] Text resultText;
    //[SerializeField] GameObject resultTextGO; //SetActive�ŕ\���𐧌䂷��̂ŃQ�[���I�u�W�F�N�g���Ǝ擾����K�v������ �C���X�y�N�^�[��ŃZ�b�g����
    //Text resultText;


    int numOfCharacterInCombat = 4; //�퓬�ɎQ�����郂���X�^�[�̎�ނ�4���
    int[] idsOfCharactersInCombat; //�퓬�ɎQ�����Ă��郂���X�^�[��ID (numOfCharacterTypes�̕������v�f��p�ӂ���)
    public int[] IDsOfCharactersInCombat
    {
        get { return idsOfCharactersInCombat; }
    } //getter�̂�
    [SerializeField] AutoRouteData autoRouteData; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] ManualRouteData manualRouteData; //�C���X�y�N�^�[��ŃZ�b�g����
    public AutoRouteData[] autoRouteDatas;
    public ManualRouteData[] manualRouteDatas;


    public readonly int wallID = 3;
    public readonly int groundID = 2;
    public readonly int facilityID = 5;
    public readonly int characterID = 11;

    public GameObject[] characterPrefabs; 
    CharacterMGR[] characterDatabase; //���chraracterPrefabs��Character�^�ɒ��������́B�f�[�^�x�[�X�Ƃ��Ďg���B

    int dragNum; //�h���b�O�����ԍ���ێ����Ă������߂ɕK�v

    int characterCounter =0; //�L�����N�^�[�����̃X�|�[���������𐔂���
    float characterDisplacement= 0.03f; //�L�����N�^�[���X�|�[�������Ƃ��ɂǂꂭ�炢�Y���邩�����߂�(10��ň������悤�ɂ���)

    public CharacterMGR.Mode[] characterMode; //�L�����N�^�[�̎�ނ��Ƃ̑��샂�[�h���i�[����

    bool[] isSpawnCharacterArray; //�R���[�`���p

    [SerializeField] State _state; //�f�o�b�O���₷���悤��SerializeField�ɂ��Ă���
    public State state
    {
        get { return _state; }
        set
        {

            _state = value;
        }
    } //�v���p�e�B
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


    //���W�ϊ�
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
    public void SetCharacterMode(int characterTypeNum,CharacterMGR.Mode mode)
    {
        if(characterTypeNum<0 || this.numOfCharacterInCombat < characterTypeNum)
        {
            Debug.LogError($"SetCharacterMode�̈���characterTypeNum��{characterTypeNum}�ɂȂ��Ă��܂�");
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
        //resultText = resultTextGO.GetComponent<Text>();

        idsOfCharactersInCombat = new int[numOfCharacterInCombat];

        isSpawnCharacterArray = new bool[numOfCharacterInCombat];
        for (int i =0;i< isSpawnCharacterArray.Length;i++)
        {
            isSpawnCharacterArray[i] = false;
        }

        autoRouteDatas = new AutoRouteData[numOfCharacterInCombat];
        manualRouteDatas = new ManualRouteData[numOfCharacterInCombat];

        characterDatabase = new CharacterMGR[characterPrefabs.Length];
        for(int i=0; i < characterPrefabs.Length; i++)
        {
            //characterDatabase[i] = characterPrefabs[i].GetComponent<CharacterMGR>();
            characterDatabase[i] = characterPrefabs[i].GetComponent<CharacterMGR>().Clone(); //�f�B�[�v�R�s�[������

            //�L�����N�^�[�̃��x�����t�@�C������ǂݍ���
            characterDatabase[i].SetInitiLevel(saveMGR.GetCharacterLevel(i));
            

        }

        characterMode = new CharacterMGR.Mode[numOfCharacterInCombat];
        for(int i = 0; i<characterMode.Length; i++)
        {
            characterMode[i] = CharacterMGR.Mode.Auto; //�f�t�H���g��AutoMode
        }

        for (int i=0;i<numOfCharacterInCombat;i++)
        {
            autoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(),mapMGR.GetMapHeight());
            manualRouteDatas[i] = new ManualRouteData(); //����manualRouteData���Ȃ�
        }



        StartSelectingStage();

    }
    

    public void StartSelectingStage()
    {
        state = State.SelectingStage;

        resultCanvas.SetActive(false);
        selectStageCanvas.SetActive(true);
        //����PlayingGame�ɔ����đO�̐퓬�̃f�[�^�������Ń��Z�b�g����i���͓��ɏ������Ƃ͂Ȃ��j
    }

    public  void SetupGame()
    {
        state = State.SetupGame;
        selectStageCanvas.SetActive(false);

        foreach(SelectCharacterButtonMGR MGR in selectCharacterButtonMGR)
        {
            MGR.InitiSelectCharacterButton();
        }
        timerMGR.InitiTimer();
        energyMGR.InitiEnergy();

        //SetCharacterTypeIDInCombat();

        mapMGR.SetupMap();

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
    public void StartShowingResults( bool isWin)
    {
        state = State.ShowingResults;

        //resultTextGO.SetActive(true);
        resultCanvas.SetActive(true);

        if (isWin)
        {
            int EXPGained = mapMGR.GetEXPOfTheStage() ;

            //�o���l��^����
            statusCanvasMGR.EXPRetained += EXPGained;

            resultText.text = $"�����I\nEXP{EXPGained}���l���I";


        }
        else
        {
            //resultText.text = "�s�k";
            resultText.text = "�s�k";

        }

        //Save������
        saveMGR.SaveEXPAmount(statusCanvasMGR.EXPRetained);
        if (saveMGR.GetStagesCleardNum() < mapMGR.GetStageNum())
        {
            saveMGR.SaveStagesCleardNum(mapMGR.GetStageNum());
        }

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
        for (int i = 0; i < mapMGR.characterSpawnPoss.Length; i++) //characterSpawnPoss��1�����Ȃ��̂ŁAfor���[�v�ɈӖ��͂Ȃ�
        {

            if (mapMGR.GetMapValue(mapMGR.characterSpawnPoss[i]) % GameManager.instance.groundID != 0)
            {
                Debug.Log($"{mapMGR.characterSpawnPoss[i]}��mapValue��groundID���܂܂�Ȃ����߁A�����X�^�[���X�|�[���ł��܂���B");
                return;
            }
            if (energyMGR.CurrentEnergy < characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost())
            {
                Debug.Log($"Energy������Ȃ����߃����X�^�[���X�|�[���ł��܂���B�ienergyMGR.CurrentEnergy :{energyMGR.CurrentEnergy},Cost:{characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost()}�j");
                return;
            }

            Debug.Log("SpawnCharacterCoroutine�����s���܂�");
            StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss[i], buttonNum));

        }
    }

    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector, int buttonNum)
    {
        isSpawnCharacterArray[buttonNum] = true;

        int characterTypeID = IDsOfCharactersInCombat[buttonNum];

        Vector3 displacement = new Vector3(characterDisplacement * (characterCounter%7)-3*characterDisplacement, 0.5f*(characterDisplacement * (characterCounter % 7) - 3 * characterDisplacement), 0); //�L�����N�^�[���������炷 y�����̃Y����x�����̃Y����0.5�{
        Debug.Log($"displacement:{displacement}");

        GameObject characterGO = Instantiate(characterPrefabs[characterTypeID], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0) + displacement, Quaternion.identity);

        CharacterMGR characterMGR = characterGO.GetComponent<CharacterMGR>();

        //�X�|�[����Cost�������
        energyMGR.CurrentEnergy -= characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost();

        //�L�����N�^�[�̃f�[�^�������œn��
        characterMGR.SetCharacterData(buttonNum,characterTypeID);

        mapMGR.MultiplySetMapValue(vector, characterID);
        mapMGR.GetMap().AddCharacterMGR(vector,characterMGR);

        characterCounter++;

        //�L�����N�^�[�̃��[�h�����߂�
        characterMGR.SetMode(characterMode[buttonNum]);

        //yield return new WaitForSeconds(1f); //�N�[���^�C���͓K��

        float time = 0;
        while(time < characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCoolTime())
        {
            time += Time.deltaTime;
            selectCharacterButtonMGR[buttonNum].RefreshGauge(time / characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCoolTime());
            yield return null;
        }

        isSpawnCharacterArray[buttonNum] = false;
    }

    public void DragCharacterData(int dragNum)
    {
        this.dragNum = dragNum;
    }

    public void DropCharacterData(int dropNum)
    {
        IDsOfCharactersInCombat[dropNum] = dragNum;
        //Debug.Log($"IDsOfCharactersInCombat[{dropNum}]:{IDsOfCharactersInCombat[dropNum] }");
    }

    public int[,] CalcSearchRangeArray(int advancingDistance, int lookingForValue, int notLookingForValue, int centerValue) //�}�X�ڂɒu����~�`�̍��G�͈͂��v�Z���āA2�����z��ŕԂ�
    {
        int t = lookingForValue; //���G�͈�
        int f = notLookingForValue; //���G�͈͊O
        int o = centerValue; //���_

        int size = 2 * (advancingDistance + 1) + 1;
        int[,] resultArray = new int[size, size];

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (i + j == advancingDistance || i + j == advancingDistance + 1 //����
                   || i - j == advancingDistance + 1 || i - j == advancingDistance + 2 //�E��
                   || -i + j == advancingDistance + 1 || -i + j == advancingDistance + 2 //����
                   || i + j == 3 * (advancingDistance + 1) || i + j == 3 * (advancingDistance + 1) + 1 //�E��
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

    public bool CanAttackTarget(Vector2Int gridPos, int attackRange,int targetID,out Vector2Int targetPos) //�ł��߂��U���Ώۂ��̍��W��Ԃ� �i//���݂��Ȃ��Ƃ���Vector2Int.zero��Ԃ��j
    {
        int lookingForValue = 1; //���G�͈͂̒l
        int notLookingForValue = 0; //���G�͈͊O�̒l
        int centerValue = 0; //���_�̒l

        Vector2Int vector; //���[�v���Ŏg���A(i,j)�����[���h���W�ɒ���������
        List<Vector2Int> nearestTargetList = new List<Vector2Int>();

        int[,] searchRangeArray;
        int maxRange = attackRange;


        //���G�͈͓��̍U���Ώۂ̈ʒu��List�ɒǉ�����
        for (int k = 0; k < maxRange; k++) //k�͒��S�̃}�X���牽�}�X�܂ŕ����邩��\��
        {
            //Debug.Log($"{k}��ڂ̃��[�v���J�n���܂�");

            searchRangeArray = CalcSearchRangeArray(k, lookingForValue, notLookingForValue, centerValue);
            for (int j = 0; j < searchRangeArray.GetLength(0); j++)
            {
                for (int i = 0; i < searchRangeArray.GetLength(1); i++)
                {
                    vector = new Vector2Int(gridPos.x - (k + 1) + i, gridPos.y - (k + 1) + j); //���[���h���W�ɕϊ�����

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
                Debug.Log($"nearestTargetList[0]={nearestTargetList[0]}");
                break;
            }
        }


        // 2  1
        // 4  3 �ƗD�揇�ʂ�����i�U���͈͓��ɑ��݂��邩�𔻒肷�邾���Ȃ炢��Ȃ����A���O���o�������ɍ������Ȃ��悤�ɗD�揇�ʂ����Ă����j
        //target�����������ɂ���Ƃ��ɗD�揇�ʂ����Č������邽�߂� List�̒��g���\�[�g����
        nearestTargetList.Sort((a, b) => b.y - a.y); //�܂�y���W�Ɋւ��č~���Ń\�[�g����
        nearestTargetList.Sort((a, b) => b.x - a.x); //����x���W�Ɋւ��č~���Ń\�[�g����

        if (nearestTargetList.Count > 0)
        {
            targetPos = nearestTargetList[0];
            //Debug.Log("nearestTargetList:" + string.Join(",", nearestTargetList) + $"\ntargetPos:{targetPos}");
            return true;
        }
        else
        {
            targetPos = Vector2Int.zero; //null�̑���
            //Debug.Log("nearestTargetList:" + string.Join(",", nearestTargetList) + $"\ntargetPos:{targetPos}");
            return false;
        }
    }

    public int CalcDamage(int atk)
    {
        return atk; //�Ƃ肠�������͍U���͂����̂܂ܕԂ�����
    }

    public void InitiManualRouteData()
    {
        foreach(ManualRouteData m in manualRouteDatas)
        {
            m.ResetManualRouteData();
        }
    }
}

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
    [SerializeField] public SelectStageButtonMGR[] selectStageButtonMGRs; //7�Z�b�g����
    [SerializeField] public SelectCharacterButtonMGR[] selectCharacterButtonMGRs; //4�Z�b�g����
    [SerializeField] public CharacterInReserveMGR[] characterInReserveMGRs; //�Ƃ肠����9�Z�b�g����
    [SerializeField] public CharacterInCombatMGR[] characterInCombatMGRs; //4�Z�b�g����
    [SerializeField] public TimerMGR timerMGR;
    [SerializeField] public EnergyMGR energyMGR;
    [SerializeField] public TowerPosDataMGR towerPosDataMGR;
    [SerializeField] public SaveMGR saveMGR;
    [SerializeField] public CharacterSkillsDataMGR characterSkillsDataMGR;
    [SerializeField] public CurveToMouseMGR curveToMouseMGR;
    [SerializeField] public MusicMGR musicMGR;
    //[SerializeField] public CharacterSkillsMGR characterSkillsMGR; 

    [SerializeField] public GameObject selectStageCanvas; //SetActive�ŕ\���𐧌䂷��̂ŃQ�[���I�u�W�F�N�g���Ǝ擾����K�v������ �C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] public GameObject menuCanvas; //SetActive�ŕ\���𐧌䂷��̂ŃQ�[���I�u�W�F�N�g���Ǝ擾����K�v������ �C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] public StatusCanvasMGR statusCanvasMGR;
    [SerializeField] GameObject frameCanvas; //SetActive�ŕ\���𐧌䂷��̂ŃQ�[���I�u�W�F�N�g���Ǝ擾����K�v������ �C���X�y�N�^�[��ŃZ�b�g����

    [SerializeField] GameObject resultCanvas; //SetActive�ŕ\���𐧌䂷��̂ŃQ�[���I�u�W�F�N�g���Ǝ擾����K�v������ �C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] Text resultText;

    [SerializeField] public float gameSpeed = 1; //�f�o�b�O�p�@�R���[�`���̒��Ƃ��Ŏg���A�Q�[���X�s�[�h�𑬂�����

    bool initializationFlag = false;
    public bool InitializationFlag
    {
        get { return initializationFlag; }
    }

    int numOfCharacterInCombat = 4; //�퓬�ɎQ�����郂���X�^�[�̎�ނ�4���
    int[] idsOfCharactersInCombat; //�퓬�ɎQ�����Ă��郂���X�^�[��ID (numOfCharacterTypes�̕������v�f��p�ӂ���)
    public int[] IDsOfCharactersInCombat //�v���p�e�B
    {
        get { return idsOfCharactersInCombat; }
    } //getter�̂�

    [SerializeField] int[] characterIDsThatCanBeUsed;
    public int[] CharacterIDsThatCanBeUsed  //8�p�ӂ���(�ő��Stage7�̒l7�����邩��) �N���A�����X�e�[�W�������Ɏ���āA�g����L�����N�^�[��ID�̍ő�l��Ԃ��z��(���̔z��͒P�������ł��邱�Ƃɒ���)
    {
        get { return characterIDsThatCanBeUsed; }
    }


    int maxCharacterNum = 50; //�t�B�[���h�ɏo����L�����N�^�[�̍ő吔 characterID��11�̎���23�̂܂ŏo���AcharacterID��2�̎���50�̏o���Ă����Ȃ��B�@�Ƃ肠�����A�ɂ�񂱑�푈�Ɠ����悤��50�ɂ��Ă���
    int maxRobotNum = 30;   //30�͓K���Ȑ���
    public int MaxCharacterNum //Getter�̂�
    {
        get { return maxCharacterNum; }
    }
    [SerializeField] int currentCharacterNum = 0; //�t�B�[���h��ɂ���L�����N�^�[�̐� �L�����N�^�[�����炷�̂ƁA��ɏo�����������߂邽�߂ɕK�v�i�f�o�b�O�p��SerializeField�ɂ��Ă���j
    [SerializeField] int currentRobotNum = 0; //�t�B�[���h��ɂ��郍�{�b�g�̐� ���{�b�g�����炷�̂ƁA��ɏo�����������߂邽�߂ɕK�v�i�f�o�b�O�p��SerializeField�ɂ��Ă���j

    public int CurrentCharacterNum
    {
        get { return currentCharacterNum; }
        set
        {
            currentCharacterNum = value;
        }
    }
    public int CurrentRobotNum
    {
        get { return currentRobotNum; }
        set { currentRobotNum = value; }
    }
    int maxTowerNum; //�e�X�e�[�W�ł̃^���[�̍ő�l(�X�e�[�W���Ƃɕς��)
    public int MaxTowerNum //Getter�̂�
    {
        get { return maxTowerNum; }
    }
    [SerializeField] int currentTowerNum; //�����c���Ă���^���[�̐��@�f�o�b�O�p��SerializeField�ɂ��Ă���
    public int CurrentTowerNum
    {
        get { return currentTowerNum; }
        set
        {
            currentTowerNum = value;
            //Debug.Log($"currentTowerNum��{currentTowerNum}�ɂ��܂���");
        }
    }


    int stagesClearedNum = 0;
    public int StagesClearedNum
    { //�v���p�e�B�@�ǂ��̃X�e�[�W�܂ŃN���A���������L�^���Ă���
        get { return stagesClearedNum; }
        set
        {
            //Debug.Log($"StagesClearedNum��Setter���J�n���܂�");

            //StageButton���X�V����
            if (value - stagesClearedNum < 0)
            {
                Debug.LogError("StagesCleardNum�̒l���������Ȃ�悤�ɒl���������܂���");
                return;
            }
            stagesClearedNum = value;
            for (int i = 0; i < selectStageButtonMGRs.Length; i++)
            {
                selectStageButtonMGRs[i].UpdateSelectStageButtonMGR();
            }

            //CharacterInReverse��CharacterInCombat���X�V����
            for (int i = 0; i < characterInReserveMGRs.Length; i++)  //CharacterInReserve�̐��͍ő�12
            {
                characterInReserveMGRs[i].UpdateCharacterInReserve();
            }
        }
    }


    [SerializeField] AutoRouteData autoRouteData; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] ManualRouteData manualRouteData; //�C���X�y�N�^�[��ŃZ�b�g����
    public AutoRouteData[] characterAutoRouteDatas;
    public ManualRouteData[] characterManualRouteDatas;
    public AutoRouteData[] robotAutoRouteDatas;


    public readonly int wallID = 5;
    public readonly int groundID = 11;
    public readonly int towerID = 7;
    public readonly int allyCastleID = 13;
    public readonly int enemyCastleID = 17;
    public readonly int characterID = 2;
    public readonly int robotID = 3;

    public GameObject[] characterPrefabs;
    public GameObject[] robotPrefabs;
    CharacterMGR[] characterDatabase; //���chraracterPrefabs��Character�^�ɒ��������́B�f�[�^�x�[�X�Ƃ��Ďg���B

    int dragNum; //�h���b�O�����ԍ���ێ����Ă������߂ɕK�v
    bool dragFlag; //CharacterInReserve����CharacterInCombat�ւ̃h���b�O�݂̂�L���ɂ��邽�߂ɕK�v
    public bool DragFlag
    {
        get { return dragFlag; }
        set { dragFlag = value; }
    }

    public int copyingSelectCharacterButtonNum = -1; //ManualRoute���R�s�[����Ƃ��A�h���b�O�����ԍ���ێ����Ă������߂ɕK�v
    public bool copyingManualRoute; //ManualRoute���R�s�[����Ƃ��ASelectCharacterButton��I�����Ă����true�ɂ���
    public int mouseEnteredSelectCharacterButtonNum = -1;

    float characterDisplacement = 0.03f; //�L�����N�^�[���X�|�[�������Ƃ��ɂǂꂭ�炢�Y���邩�����߂�(10��ň������悤�ɂ���)

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
    public void SetCharacterMode(int characterTypeNum, CharacterMGR.Mode mode)
    {
        if (characterTypeNum < 0 || this.numOfCharacterInCombat < characterTypeNum)
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
        if (InitializationFlag == false) Initialization();
    }

    public void Initialization() //Start�֐��̕ς��B���̊֐��̏������ŎQ�Ƃ��������߁AInitializationFlag(bool)��p�ӂ��A������ĂԂƂ��ɏ���������Ă��Ȃ���Ώ���������悤�ɂ���
    {
        initializationFlag = true;

        idsOfCharactersInCombat = new int[numOfCharacterInCombat];

        isSpawnCharacterArray = new bool[numOfCharacterInCombat];
        for (int i = 0; i < isSpawnCharacterArray.Length; i++)
        {
            isSpawnCharacterArray[i] = false;
        }

        characterAutoRouteDatas = new AutoRouteData[numOfCharacterInCombat];
        characterManualRouteDatas = new ManualRouteData[numOfCharacterInCombat];
        robotAutoRouteDatas = new AutoRouteData[robotPrefabs.Length];

        for (int i = 0; i < numOfCharacterInCombat; i++)
        {
            characterAutoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(), mapMGR.GetMapHeight());
            characterManualRouteDatas[i] = new ManualRouteData(); //����manualRouteData���Ȃ�
        }
        for (int i = 0; i < robotPrefabs.Length; i++)
        {
            robotAutoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(), mapMGR.GetMapHeight());
        }

        characterDatabase = new CharacterMGR[characterPrefabs.Length];
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            //characterDatabase[i] = characterPrefabs[i].GetComponent<CharacterMGR>();
            characterDatabase[i] = characterPrefabs[i].GetComponent<CharacterMGR>().Clone(); //�f�B�[�v�R�s�[������

            //�L�����N�^�[�̃��x�����t�@�C������ǂݍ���
            characterDatabase[i].SetInitiLevel(saveMGR.GetCharacterLevel(i));

        }

        characterMode = new CharacterMGR.Mode[numOfCharacterInCombat];
        for (int i = 0; i < characterMode.Length; i++)
        {
            characterMode[i] = CharacterMGR.Mode.Auto; //�f�t�H���g��AutoMode
        }



        StagesClearedNum = saveMGR.GetStagesCleardNum();

        statusCanvasMGR.InitiStatusCanvasMGR();

        for (int i = 0; i < IDsOfCharactersInCombat.Length; i++) //CharacterInCombat�̏��Ԃ��t�@�C������ǂݍ���
        {
            IDsOfCharactersInCombat[i] = saveMGR.GetCharacterInCombatID(i);
        }


        StartSelectingStage();

    }
    public void StartSelectingStage()
    {
        state = State.SelectingStage;

        musicMGR.StopAllBGM();

        resultCanvas.SetActive(false);
        selectStageCanvas.SetActive(true);
        //����PlayingGame�ɔ����đO�̐퓬�̃f�[�^�������Ń��Z�b�g����i���͓��ɏ������Ƃ͂Ȃ��j

        frameCanvas.SetActive(true);      //SelectStageCanvas��MenuCanvas�ł�FrameCanvas��\������
    }

    public void SetupGame()
    {
        state = State.SetupGame;
        selectStageCanvas.SetActive(false);

        foreach (SelectCharacterButtonMGR MGR in selectCharacterButtonMGRs)
        {
            MGR.InitiSelectCharacterButton();
        }
        timerMGR.InitiTimer();
        energyMGR.InitiEnergy();

        //SetCharacterTypeIDInCombat();

        frameCanvas.SetActive(false);    //�퓬��ʂł�FrameCanvas���\���ɂ���

        mapMGR.SetupMap();
        musicMGR.StartStageBGM(mapMGR.GetStageNum());

        CurrentCharacterNum = 0;
        CurrentRobotNum = 0;
        maxTowerNum = mapMGR.GetMaxTowerNum();
        CurrentTowerNum = maxTowerNum;

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
    public void StartShowingResults(bool isWin)
    {
        state = State.ShowingResults;

        //resultTextGO.SetActive(true);
        resultCanvas.SetActive(true);

        if (isWin)
        {
            int EXPGained = mapMGR.GetEXPOfTheStage();

            //�o���l��^����
            statusCanvasMGR.EXPRetained += EXPGained;

            resultText.text = $"�����I\nEXP{EXPGained}���l���I";


        }
        else
        {
            //resultText.text = "�s�k";
            resultText.text = "�s�k";

        }

        Debug.LogWarning($"StagesClearedNum:{StagesClearedNum}");
        //StagesClearedNum���X�V����
        if (StagesClearedNum < mapMGR.GetStageNum() + 1)
        {
            StagesClearedNum = mapMGR.GetStageNum() + 1;
            Debug.LogWarning($"StagesClearedNum���X�V����:{StagesClearedNum}�@�ɂ��܂���");

        }


        //Save������
        saveMGR.SaveEXPAmount(statusCanvasMGR.EXPRetained);
        saveMGR.SaveStagesCleardNum(StagesClearedNum);

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

        if (mapMGR.GetMapValue(mapMGR.characterSpawnPoss) % GameManager.instance.groundID != 0)
        {
            Debug.Log($"{mapMGR.characterSpawnPoss}��mapValue��groundID���܂܂�Ȃ����߁A�����X�^�[���X�|�[���ł��܂���B");
            return;
        }
        if (energyMGR.CurrentEnergy < characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost())
        {
            Debug.Log($"Energy������Ȃ����߃����X�^�[���X�|�[���ł��܂���B�ienergyMGR.CurrentEnergy :{energyMGR.CurrentEnergy},Cost:{characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost()}�j");
            return;
        }
        if (currentCharacterNum >= maxCharacterNum)
        {
            Debug.Log($"currentCharacterNum:{currentCharacterNum}��maxCharacterNum:{maxCharacterNum}�ȏ�ł��邽�߁A�X�|�[���ł��܂���");
            return;
        }

        Debug.Log("SpawnCharacterCoroutine�����s���܂�");
        StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss, buttonNum));


    }


    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector, int buttonNum)
    {
        isSpawnCharacterArray[buttonNum] = true;

        int characterTypeID = IDsOfCharactersInCombat[buttonNum];

        Vector3 displacement = new Vector3(characterDisplacement * (currentCharacterNum % 7) - 3 * characterDisplacement, 0.5f * (characterDisplacement * (currentCharacterNum % 7) - 3 * characterDisplacement), 0); //�L�����N�^�[���������炷 y�����̃Y����x�����̃Y����0.5�{
        Debug.Log($"displacement:{displacement}");

        GameObject characterGO = Instantiate(characterPrefabs[characterTypeID], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0) + displacement, Quaternion.identity);

        CharacterMGR characterMGR = characterGO.GetComponent<CharacterMGR>();

        //�X�|�[����Cost�������
        energyMGR.CurrentEnergy -= characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCost();

        //�L�����N�^�[�̃f�[�^�������œn��
        characterMGR.SetCharacterData(buttonNum, characterTypeID);

        mapMGR.MultiplySetMapValue(vector, characterID);
        mapMGR.GetMap().AddUnit(vector, characterMGR);

        currentCharacterNum++;

        //�L�����N�^�[�̃��[�h�����߂�
        characterMGR.SetMode(characterMode[buttonNum]);


        float time = 0;
        while (time < characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCoolTime()) //�N�[���^�C���̎��Ԃ����~�߂�
        {
            time += Time.deltaTime * GameManager.instance.gameSpeed;
            selectCharacterButtonMGRs[buttonNum].RefreshGauge(time / characterDatabase[IDsOfCharactersInCombat[buttonNum]].GetCoolTime());

            while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //�|�[�Y���͎~�߂�

            yield return null;
        }

        isSpawnCharacterArray[buttonNum] = false;
    }
    public void SpawnRobot()
    {
        if (mapMGR.GetMapValue(mapMGR.robotSpawnPoss) % GameManager.instance.groundID != 0)
        {
            Debug.Log($"{mapMGR.robotSpawnPoss}��mapValue��groundID���܂܂�Ȃ����߁A���{�b�g���X�|�[���ł��܂���B");
            return;
        }

        if (currentRobotNum >= maxRobotNum)
        {
            Debug.Log($"currentRobotNum:{currentRobotNum}��maxRobotNum:{maxRobotNum}�ȏ�ł��邽�߁A�X�|�[���ł��܂���");
            return;
        }

        Debug.Log("SpawnRobotCoroutine�����s���܂�");
        StartCoroutine(SpawnRobotCoroutine(mapMGR.robotSpawnPoss, 0));

    }
    private IEnumerator SpawnRobotCoroutine(Vector2Int vector, int robotTypeID)
    {

        Vector3 displacement = new Vector3(characterDisplacement * (currentRobotNum % 7) - 3 * characterDisplacement, 0.5f * (characterDisplacement * (currentRobotNum % 7) - 3 * characterDisplacement), 0); //���{�b�g���������炷 y�����̃Y����x�����̃Y����0.5�{
        Debug.Log($"displacement:{displacement}");

        GameObject robotGO = Instantiate(robotPrefabs[robotTypeID], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0) + displacement, Quaternion.identity);

        RobotMGR robotMGR = robotGO.GetComponent<RobotMGR>();



        //���{�b�g��autoRote������������
        robotMGR.SetCharacterData(robotTypeID);

        mapMGR.MultiplySetMapValue(vector, robotID);
        mapMGR.GetMap().AddUnit(vector, robotMGR);

        currentRobotNum++;

        //�N�[���^�C���̏����͂Ƃ肠�����R�����g�A�E�g
        //float time = 0;
        //while (time < characterDatabase[IDsOfCharactersInCombat[robotTypeNum]].GetCoolTime()) //�N�[���^�C���̎��Ԃ����~�߂�
        //{
        //    time += Time.deltaTime * GameManager.instance.gameSpeed;
        //    selectCharacterButtonMGRs[robotTypeNum].RefreshGauge(time / characterDatabase[IDsOfCharactersInCombat[robotTypeNum]].GetCoolTime());

        //    while (GameManager.instance.state == GameManager.State.PauseTheGame) { yield return null; } //�|�[�Y���͎~�߂�

        //    yield return null;
        //}

        yield return new WaitForSeconds(0); ; //�����Ԃ��Ȃ��Ƃ܂����̂œK���Ȗ߂�l��Ԃ�

    }
    public void DragCharacterData(int dragNum)
    {
        this.dragNum = dragNum;
        Debug.Log($"dragNum:{dragNum}");
    }

    public void DropCharacterData(int dropNum)
    {
        IDsOfCharactersInCombat[dropNum] = dragNum;
        Debug.Log($"IDsOfCharactersInCombat[{dropNum}]:{IDsOfCharactersInCombat[dropNum] }");
    }
    public int CalcDamage(int atk)
    {
        return atk; //�Ƃ肠�������͍U���͂����̂܂ܕԂ�����
    }

    public void InitiManualRouteData()
    {
        foreach (ManualRouteData m in characterManualRouteDatas)
        {
            m.ResetManualRouteData();
        }
    }
}

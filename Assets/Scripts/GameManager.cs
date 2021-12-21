using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.NonSerialized] public MapMGR mapMGR;
    [System.NonSerialized] public InputMGR inputMGR;
    [System.NonSerialized] public DebugMGR debugMGR;
    [System.NonSerialized] public PointerMGR pointerMGR;

    int numOfCharacterTypes = 4; //�퓬�ɎQ�����郂���X�^�[�̎�ނ�4���
    [SerializeField] AutoRouteData autoRouteData; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] ManualRouteData manualRouteData; //�C���X�y�N�^�[��ŃZ�b�g����
    public AutoRouteData[] autoRouteDatas;
    public ManualRouteData[] manualRouteDatas;


    public readonly int wallID = 3;
    public readonly int groundID = 2;
    public readonly int facilityID = 5;
    public readonly int characterID = 11;

    public GameObject[] characterPrefabs; //�z��ɂ��Ă���͉̂��B���ۂɂ̓f�[�^�x�[�X�������ǂݎ���ăC���X�^���X�����邩��v���n�u�͈�ł悢

    int characterCounter =0; //�L�����N�^�[�����̃X�|�[���������𐔂���
    float characterDisplacement= 0.03f; //�L�����N�^�[���X�|�[�������Ƃ��ɂǂꂭ�炢�Y���邩�����߂�(10��ň������悤�ɂ���)

    public CharacterMGR.Mode[] characterMode; //�L�����N�^�[�̎�ނ��Ƃ̑��샂�[�h���i�[����

    bool isSpawnCharacter = false;

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

    //Setter
    public void SetCharacterMode(int characterTypeNum,CharacterMGR.Mode mode)
    {
        if(characterTypeNum<0 || this.numOfCharacterTypes < characterTypeNum)
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
        mapMGR = GameObject.Find("Tilemap").GetComponent<MapMGR>();
        inputMGR = GameObject.Find("InputMGR").GetComponent<InputMGR>();
        debugMGR = GameObject.Find("DebugMGR").GetComponent<DebugMGR>();
        pointerMGR = GameObject.Find("Pointer").GetComponent<PointerMGR>();
        Debug.LogWarning($"pointerMGR�����������܂����BpointerMGR.transform.position:{pointerMGR.transform.position}");

        autoRouteDatas = new AutoRouteData[numOfCharacterTypes];
        manualRouteDatas = new ManualRouteData[numOfCharacterTypes];

        characterMode = new CharacterMGR.Mode[numOfCharacterTypes];
        for(int i = 0; i<characterMode.Length; i++)
        {
            characterMode[i] = CharacterMGR.Mode.Auto; //�f�t�H���g��AutoMode
        }

        for (int i=0;i<numOfCharacterTypes;i++)
        {
            autoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(),mapMGR.GetMapHeight());
            manualRouteDatas[i] = new ManualRouteData(); //����manualRouteData���Ȃ�
        }

        mapMGR.SetupMap();

    }

    public void SpawnCharacter(int CharacterTypeNum)
    {
        if (isSpawnCharacter)
        {
            return;
        }
        for (int i = 0; i < mapMGR.characterSpawnPoss.Length; i++)
        {
            if (mapMGR.GetMapValue(mapMGR.characterSpawnPoss[i]) % GameManager.instance.groundID == 0)
            {
                Debug.Log("SpawnCharacterCoroutine�����s���܂�");
                StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss[i], CharacterTypeNum)); //�Ƃ肠����characterID�̈�����0�ɂ��Ă���
            }

        }
    }

    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector, int characterTypeNum)
    {
        isSpawnCharacter = true;

        Vector3 displacement = new Vector3(characterDisplacement * (characterCounter%10)-3*characterDisplacement, characterDisplacement * (characterCounter % 10) - 3 * characterDisplacement, 0); //�L�����N�^�[���������炷
        Debug.LogWarning($"displacement:{displacement}");

        GameObject characterGO = Instantiate(characterPrefabs[characterTypeNum], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0) + displacement, Quaternion.identity);
        CharacterMGR characterMGR = characterGO.GetComponent<CharacterMGR>();

        //�L�����N�^�[�̃f�[�^�������œn��
        characterMGR.SetCharacterData(characterTypeNum); //����CharacterTypeID�Ƃ���0��n���Ă����BAutoRouteData��[0]���Q�Ƃ���悤�ɂȂ�B

        mapMGR.MultiplySetMapValue(vector, characterID);
        mapMGR.GetMap().AddCharacterMGR(vector,characterMGR);

        characterCounter++;

        //�L�����N�^�[�̃��[�h�����߂�
        characterMGR.SetMode(characterMode[characterTypeNum]);

        yield return new WaitForSeconds(1f); //�N�[���^�C���͓K��

        isSpawnCharacter = false;
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

    public bool CanAttackTarget(Vector2Int gridPos, int attackRange,int targetID,out Vector2Int targetPos) //�ł��߂��U���Ώۂ��̍��W��Ԃ��i���݂��Ȃ��Ƃ���Vector2Int.zero��Ԃ��j
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



}

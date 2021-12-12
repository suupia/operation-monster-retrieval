using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.NonSerialized] public MapMGR mapMGR;
    [System.NonSerialized] public InputMGR inputMGR;
    [System.NonSerialized] public DebugMGR debugMGR;
    [System.NonSerialized] public PointerMGR pointerMGR;

    [SerializeField] int numOfCharacterTypes = 5; //�Ƃ肠����5�Ƃ��Ă���
    [SerializeField] AutoRouteData autoRouteData; //�C���X�y�N�^�[��ŃZ�b�g����
    [SerializeField] ManualRouteData manualRouteData; //�C���X�y�N�^�[��ŃZ�b�g����
    public AutoRouteData[] autoRouteDatas;
    public ManualRouteData[] manualRouteDatas;


    public readonly int wallID = 3;
    public readonly int groundID = 2;
    public readonly int towerID = 5;
    public readonly int characterID = 11;

    public GameObject[] characterPrefabs;

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

        autoRouteDatas = new AutoRouteData[numOfCharacterTypes];  //�Ƃ肠����5�̕��̃��[�g��p�ӂ��Ă���
        manualRouteDatas = new ManualRouteData[numOfCharacterTypes];

        for (int i=0;i<numOfCharacterTypes;i++)
        {
            autoRouteDatas[i] = new AutoRouteData(mapMGR.GetMapWidth(),mapMGR.GetMapHeight());
            manualRouteDatas[i] = new ManualRouteData(); //����manualRouteData���Ȃ�
        }

        mapMGR.SetupMap();

    }

    private void Update()
    {
        SpawnCharacter();
    }

    private void SpawnCharacter()
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
                StartCoroutine(SpawnCharacterCoroutine(mapMGR.characterSpawnPoss[i], 0)); //�Ƃ肠����characterID�̈�����0�ɂ��Ă���
            }

        }
    }

    private IEnumerator SpawnCharacterCoroutine(Vector2Int vector, int characterTypeNum)
    {
        isSpawnCharacter = true;

        GameObject characterGO; //�X�R�[�v�ɒ��Ӂ@���̃��\�b�h�̒��ł����g���Ȃ�
        CharacterMGR characterMGR;

        characterGO = Instantiate(characterPrefabs[characterTypeNum], new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0), Quaternion.identity);
        characterMGR = characterGO.GetComponent<CharacterMGR>();

        //�L�����N�^�[�̃f�[�^�������œn��
        characterMGR.SetCharacterData(0); //����CharacterTypeID�Ƃ���0��n���Ă����BAutoRouteData��[0]���Q�Ƃ���悤�ɂȂ�B
        

        mapMGR.MultiplySetMapValue(vector, characterID);
        yield return new WaitForSeconds(200f); //�Ƃ肠�����f�o�b�O���₷���悤�ɒ������Ă���

        isSpawnCharacter = false;
    }

    public Vector2 RotateVector(Vector2 vector, float degreeMeasure)
    {
        return Quaternion.Euler(0, 0, -degreeMeasure) * vector;
    }

}

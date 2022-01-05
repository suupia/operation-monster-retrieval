using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVLoader : MonoBehaviour
{
    TextAsset csv;

    List<string[]> csvDataList = new List<string[]>(); //csv��string�ł��̂܂ܓǂݍ��񂾂���

    //��static�ł��邱�Ƃɒ���
    public static readonly List<MonsterData> monsterDataList = new List<MonsterData>(); //csvDataList��string�^�������₷���悤��int��float�^�ɂ��A�K�v�Ȃ��v�f(���O�Ƃ����ږ��Ƃ�)���폜��������

    int numOfRows; //csv�̍s�̐�
    int numOfColumns; //csv�̗�̐�

    void Start()
    {
        csv = Resources.Load("CSV/MonsterDatabase") as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            csvDataList.Add(line.Split(','));
        }

        numOfRows = csvDataList.Count;
        numOfColumns = csvDataList[0].Length; //��\�Ƃ���1�s�ڂ̗v�f�̐��𐔂���
        Debug.Log($"numOfRows : {numOfRows}");
        Debug.Log($"numOfColumns : {numOfColumns}");



        for (int i = 1; i < numOfRows; i++)
        {
            monsterDataList.Add(new MonsterData(csvDataList[i]));
            Debug.Log($"monsterDataList.Count : {monsterDataList.Count}");
            //monsterDataList[i][0] = 3;
            Debug.Log($"monsterDataList[i] : {monsterDataList[i-1]}");

            Debug.Log($"ID,HP,ATK,AttackInterval,AttackRange,SPD,CoolTime,Cost : {monsterDataList[i - 1].ID},{monsterDataList[i - 1].HP},{monsterDataList[i - 1].ATK},{monsterDataList[i - 1].AttackInterval},{monsterDataList[i - 1].AttackRange},{monsterDataList[i - 1].SPD},{monsterDataList[i - 1].CoolTime},{monsterDataList[i - 1].Cost}");

        }

    }
}public struct MonsterData
{
    public readonly int ID;
    public readonly int HP;
    public readonly int ATK;
    public readonly float AttackInterval;
    public readonly int AttackRange;
    public readonly float SPD;
    public readonly float CoolTime;
    public readonly int Cost;

   public MonsterData(string[] strings){
        ID = int.Parse(strings[1]);
        HP = int.Parse(strings[2]);
        ATK = int.Parse(strings[3]);
        AttackInterval = float.Parse(strings[4]);
        AttackRange = int.Parse(strings[5]);
        SPD = float.Parse(strings[6]);
        CoolTime = float.Parse(strings[7]);
        Cost = int.Parse(strings[8]);
    }

}
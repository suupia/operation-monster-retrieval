using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveMGR : MonoBehaviour
{

    string filePath;
    SaveData saveData;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        Debug.Log($"�Z�[�u�f�[�^�̃t�@�C���p�X��{filePath}");
        saveData = new SaveData();
        Load();

    }

    //Getter
    public int GetStagesCleardNum()
    {
        return saveData.satagesCleardNum;
    }
    public int GetEXPAmount()
    {
        return saveData.EXPAmount;
    }
    public int GetCharacterLevel(int index)
    {
        return saveData.characterLevel[index];
    }

    public int GetCharacterInCombatID(int index)
    {
        return saveData.characterInCombatID[index];
    }
    
    //Setter
    public void SaveStagesCleardNum(int stageNum)
    {
        saveData.satagesCleardNum = stageNum;
        Save();
    }
    public void SaveEXPAmount(int amount)
    {
        saveData.EXPAmount = amount;
        Save();
    }

    public void SaveCharacterLevel(int index,int level)
    {
        saveData.characterLevel[index] = level;
        Save();
    }

    public void SaveCharacterInCombatID(int index,int id)
    {
        saveData.characterInCombatID[index] = id;
        Save();
    }


    // �ȗ��B�ȉ���Save�֐���Load�֐����Ăяo���Ďg�p���邱��

    void Save()
    {
        string json = JsonUtility.ToJson(saveData);
        using (StreamWriter streamWriter = new StreamWriter(filePath)) //using�\���ɂ����Dispose()�iClose()�Ɠ����悤�Ȃ��́j�������I�ɌĂ΂��
        {
            streamWriter.Write(json); 
            streamWriter.Flush();
        }
            


    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string data = streamReader.ReadToEnd();
                streamReader.Close();
                saveData = JsonUtility.FromJson<SaveData>(data);
            }
                
        }
    }

}

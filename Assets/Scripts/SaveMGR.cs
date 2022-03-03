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
        Debug.Log($"セーブデータのファイルパスは{filePath}");
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


    // 省略。以下のSave関数やLoad関数を呼び出して使用すること

    void Save()
    {
        string json = JsonUtility.ToJson(saveData);
        using (StreamWriter streamWriter = new StreamWriter(filePath)) //using構文によってDispose()（Close()と同じようなもの）が自動的に呼ばれる
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

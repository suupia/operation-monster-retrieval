using System;


[Serializable]
public class SaveData
{
    public int satagesCleardNum; //どのステージまでクリアしたか
    public int EXPAmount; //経験値
    public int[] characterLevel = new int[16]; //キャラクターのレベル
    public int[] characterInCombatID = new int[4]; //CharacterInCombatの配置されたID

}

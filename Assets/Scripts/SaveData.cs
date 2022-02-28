using System;


[Serializable]
public class SaveData
{
    public string testString;

    public int satagesCleardNum; //どのステージまでクリアしたか
    public int EXPAmount; //経験値
    public int[] characterLevel = new int[16]; //キャラクターのレベル
}

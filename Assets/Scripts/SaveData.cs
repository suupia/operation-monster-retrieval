using System;


[Serializable]
public class SaveData
{
    public int satagesCleardNum; //�ǂ̃X�e�[�W�܂ŃN���A������
    public int EXPAmount; //�o���l
    public int[] characterLevel = new int[16]; //�L�����N�^�[�̃��x��
    public int[] characterInCombatID = new int[4]; //CharacterInCombat�̔z�u���ꂽID
    public float bgmVolume; //BGM�̃{�����[��
    public float seVolume; //SE�̃{�����[��

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataManager : Singleton<MasterDataManager>
{
    //public MasterCharacterData MasterCharacterData = null;

    /// <summary>
    /// �C���X�^���X�̐���
    /// </summary>
    public static void CreateInstance()
    {
        _instance = new MasterDataManager();
    }


    public void LoadMaster()
    {
        //MasterCharacterData = ResourceManager.LoadOnView<MasterCharacterData>("MasterData/MasterCharacterData");
    }
}
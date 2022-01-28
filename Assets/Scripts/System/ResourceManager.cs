using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    /// <summary>
    /// インスタンスの生成
    /// </summary>
    public static void CreateInstance()
    {
        _instance = new ResourceManager();
    }

    public static T LoadOnView<T>( string path ) where T : Object
    {
        return Resources.Load<T>(path);
    }
}

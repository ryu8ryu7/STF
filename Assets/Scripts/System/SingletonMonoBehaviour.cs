using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }
}

public class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Type t = typeof(T);
                _instance = (T)FindObjectOfType(t);

                if (_instance == null)
                {
                    Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }
            }
            return _instance;
        }
    }

    public static bool HasInstance()
    {
        return _instance != null;
    }

}
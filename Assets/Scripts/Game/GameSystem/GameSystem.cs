using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameSystem
/// </summary>
public class GameSystem : SingletonMonoBehavior<GameSystem>
{
    /// <summary>
    /// フレームレート
    /// </summary>
    public const int FRAME_RATE = 60;

    /// <summary>
    /// 初期化
    /// </summary>
    private bool _isInitialized = false;

    /// <summary>
    /// インスタンス生成
    /// </summary>
    public static void CreateInstance()
    {
        if (HasInstance() == true)
        {
            return;
        }

        _instance = new GameObject("GameSystem").AddComponent<GameSystem>();
        _instance.Initialize();
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if( _isInitialized )
        {
            return;
        }

        UpdaterManager.CreateInstance();
        ResourceManager.CreateInstance();
        MasterDataManager.CreateInstance();
        MasterDataManager.Instance.LoadMaster();

        Application.targetFrameRate = FRAME_RATE;
        Screen.SetResolution(1920, 1080, true);

        _isInitialized = true;
    }

    private void Update()
    {
        UpdaterManager.Instance.Update();
    }
}

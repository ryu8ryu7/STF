using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdaterManager : Singleton<UpdaterManager>
{
    public enum Priority
    {
        Camera = 10000,

        Character = 20000,
    }



    private List<Updater> _updaterList = new List<Updater>();

    /// <summary>
    /// インスタンスの生成
    /// </summary>
    public static void CreateInstance()
    {
        _instance = new UpdaterManager();
    }

    public void AddUpdater( Updater updater )
    {
        if( _updaterList.Contains( updater ) )
        {
            return;
        }

        _updaterList.Add(updater);
        Sort();
    }

    public void RemoveUpdater( Updater updater )
    {
        _updaterList.Remove(updater);
        Sort();
    }

    private void Sort()
    {
        _updaterList.Sort((a, b) => { return a.UpdatePriority - b.UpdatePriority; });
    }

    public void Update()
    {
        for( int i = 0, count = _updaterList.Count; i < count; i++ )
        {
            _updaterList[i].PreAlterUpdate();
        }

        for (int i = 0, count = _updaterList.Count; i < count; i++)
        {
            _updaterList[i].AlterUpdate();
        }
    }
}

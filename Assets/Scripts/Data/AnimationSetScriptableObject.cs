using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// アニメーションの登録データ
/// </summary>
[Serializable]
public class AnimationSet
{
    [Serializable]
    public class ClipSet
    {
        public AnimationClip AnimationClip = null;
        public bool IsApplyRootMotion = false;
        public bool IsMoveManual = false;
        public float MoveManualSpeed = 0.1f;

        public bool IsLeftIK = false;
        public int LeftIKIndex = 1;
        public bool IsRightIK = false;
        public int RightIKIndex = 1;
        public bool IsSameAnime = false;
        public float LerpTime = 0.1f;
    }

    public AnimationSetScriptableObject.AnimationSetNameLabel Label = AnimationSetScriptableObject.AnimationSetNameLabel.None;
    public bool IsLoopMotion = false;
    public ClipSet[] ClipSetArray = null;

}

[CreateAssetMenu(fileName = "AnimationSet", menuName = "Game/Animation/Create AnimationSet")]
public class AnimationSetScriptableObject : ScriptableObject
{
    public enum AnimationSetNameLabel
    {
        None,

        Idle01,

        Walk01,

        Run01,

        HandGun_Shot01,
        HandGun_Shot02,
        HandGun_Shot03,

        HandGun_Cover01,
    }

    /// <summary>
    /// 保存されたアニメーションのセット
    /// </summary>
    public List<AnimationSet> AnimationSetList = new List<AnimationSet>();

    /// <summary>
    /// ファイルの識別子
    /// </summary>
    private int _setId = -1;
    public int SetId { get { return _setId; } }

    public void Initialize()
    {
        if( _setId >= 0 )
        {
            return;
        }

        string numString = name.Replace("AnimationSet", string.Empty );
        if( int.TryParse( numString, out _setId ) == false )
        {
            Debug.LogError("AnimationSetScriptableObject:ファイル名が正しくありません");
        }
    }

    public AnimationSet Get(AnimationSetNameLabel label)
    {
        Initialize();

        for( int i = 0, count = AnimationSetList.Count; i < count; i++ )
        {
            if(AnimationSetList[i].Label == label )
            {
                return AnimationSetList[i];
            }
        }

        return null;
    }

    public void Sort()
    {
        AnimationSetList.Sort((a, b) => { return (int)a.Label - (int)b.Label; });
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(AnimationSetScriptableObject))]//拡張するクラスを指定
public class AnimationSetScriptableObjectEditor : Editor
{

    /// <summary>
    /// InspectorのGUIを更新
    /// </summary>
    public override void OnInspectorGUI()
    {
        //targetを変換して対象を取得
        AnimationSetScriptableObject animationSetScriptableObject = target as AnimationSetScriptableObject;

        base.OnInspectorGUI();

        //ボタンを表示
        if (GUILayout.Button("Sort"))
        {
            animationSetScriptableObject.Sort();
        }
    }

}


#endif

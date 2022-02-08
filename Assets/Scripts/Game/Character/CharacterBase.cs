using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class CharacterBase : Updater
{
    [SerializeField]
    private CharacterController _characterController = null;

    private CameraLookDown _cameraLookDown = null;
    public CameraLookDown CameraLookDown { set { _cameraLookDown = value; } }

    public void Initialize()
    {
        _characterController = GetComponent<CharacterController>();

        InitializePosition();
        InitializeAnimation();

        LoadAnimation( LoadAnimationSetData(10001) );
        PlayAnimation(_animationSetScriptableObject.Get(AnimationSetScriptableObject.AnimationSetNameLabel.Idle01));

        _updatePriority = (int)UpdaterManager.Priority.Character;
    }

    public override void PreAlterUpdate()
    {
        base.PreAlterUpdate();

        PreUpdateControl();
    }

    public override void AlterUpdate()
    {
        base.AlterUpdate();

        UpdateInput();
        UpdateAnimation();
        UpdateFollowPosition();
        UpdateFollow();
        UpdateMove();
    }

    private void OnDestroy()
    {
        UpdaterManager.Instance.RemoveUpdater(this);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CharacterBase))]
    public partial class CharacterBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CharacterBase chara = target as CharacterBase;
            OnInspectorControl();
            OnInspectorIkController();
            EditorUtility.SetDirty(chara);
        }
    }

#endif
}

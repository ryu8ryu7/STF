using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class CharacterBase : MonoBehaviour
{
    [SerializeField]
    private CharacterController _characterController = null;

    private CameraLookDown _cameraLookDown = null;
    public CameraLookDown CameraLookDown { set { _cameraLookDown = value; } }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        InitializePosition();
        InitializeAnimation();

        LoadAnimation( LoadAnimationSetData(10001) );
        PlayAnimation(_animationSetScriptableObject.Get(AnimationSetScriptableObject.AnimationSetNameLabel.Idle01));
    }

    private void Update()
    {
        UpdateInput();
        UpdateAnimation();
        UpdateFollow();
        UpdateMove();
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

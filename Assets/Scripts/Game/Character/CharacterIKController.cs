using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Animator))]
public partial class CharacterBase
{
    public Transform RightHandObj = null;
    public bool IsRightIK = false;
    public Transform LeftHandObj = null;
    public bool IsLeftIK = false;
    public Transform LookObj = null;
    public bool IsLookIK = false;
    public bool IsLookBody = true;

    // IK を計算するためのコールバック
    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {

            // IK が有効ならば、位置と回転を直接設定します
            if (IsLookIK)
            {
                // すでに指定されている場合は、視線のターゲット位置を設定します
                if (LookObj != null)
                {
                    _animator.SetLookAtPosition(LookObj.position);
                    _animator.SetLookAtWeight(1, IsLookBody ? 1 : 0, 1, 0, 0.5f);
                }
            }
            else
            {
                _animator.SetLookAtWeight(0);
            }

            if (IsRightIK)
            {
                // 指定されている場合は、右手のターゲット位置と回転を設定します
                if (RightHandObj != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
                }
            }
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }

            if (IsLeftIK)
            {
                if (LeftHandObj != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandObj.position);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandObj.rotation);
                }
            }
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }

#if UNITY_EDITOR
    public partial class CharacterBaseEditor : Editor
    {
        private bool _isFoldIKControl = false;
        protected void OnInspectorIkController()
        {
            CharacterBase chara = target as CharacterBase;

            if (_isFoldIKControl = EditorGUILayout.Foldout(_isFoldIKControl, "IKController"))
            {
                EditorGUILayout.BeginHorizontal();
                chara.IsLeftIK = EditorGUILayout.Toggle("IsLeftIK", chara.IsLeftIK);
                chara.LeftHandObj = EditorGUILayout.ObjectField(chara.LeftHandObj, typeof(Transform), true) as Transform;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                chara.IsRightIK = EditorGUILayout.Toggle("IsRightIK", chara.IsRightIK);
                chara.RightHandObj = EditorGUILayout.ObjectField(chara.RightHandObj, typeof(Transform), true) as Transform;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                chara.IsLookIK = EditorGUILayout.Toggle("IsLookIK", chara.IsLookIK);
                chara.LookObj = EditorGUILayout.ObjectField(chara.LookObj, typeof(Transform), true) as Transform;
                chara.IsLookBody = EditorGUILayout.Toggle("IsLookBody", chara.IsLookBody);
                EditorGUILayout.EndHorizontal();
            }
        }

    }

#endif
}
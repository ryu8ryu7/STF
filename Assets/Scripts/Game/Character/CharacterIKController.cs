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

    // IK ���v�Z���邽�߂̃R�[���o�b�N
    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {

            // IK ���L���Ȃ�΁A�ʒu�Ɖ�]�𒼐ڐݒ肵�܂�
            if (IsLookIK)
            {
                // ���łɎw�肳��Ă���ꍇ�́A�����̃^�[�Q�b�g�ʒu��ݒ肵�܂�
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
                // �w�肳��Ă���ꍇ�́A�E��̃^�[�Q�b�g�ʒu�Ɖ�]��ݒ肵�܂�
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
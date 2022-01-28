using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class CharacterBase
{
    public enum MoveType
    {
        None,

        LookDownFreeMove,   // 見下ろし自由移動
        TargetMove          // 目標地点誘導
        

    }

    protected MoveType _moveType = MoveType.LookDownFreeMove;

    protected Vector3 _direction = Vector3.zero;

    [SerializeField]
    protected float _moveSpeed = 0.1f;

    #region TargetMove

    protected Vector3 _targetPos = Vector3.zero;

    protected bool _isMoveEnd = true;

    #endregion TargetMove

    protected virtual void UpdateInput()
    {
        switch( _moveType )
        {
            case MoveType.LookDownFreeMove:
                Control_LookDownFreeMove();
                break;
            case MoveType.TargetMove:
                Control_TargetMove();
                break;
        }

    }

    protected virtual void Control_LookDownFreeMove()
    {
        _direction.x = 0;
        _direction.y = -0.1f;
        _direction.z = 0;

        if (_currentAnimationSet.Label == AnimationSetScriptableObject.AnimationSetNameLabel.HandGun_Cover01)
        {
            if (Input.GetKeyDown("joystick button 0"))
            {
                PlayEndAnimation();
            }
        }
        else
        {
            //L Stick
            float lsh = Input.GetAxis("L_Stick_H");
            float lsv = Input.GetAxis("L_Stick_V");
            AnimationSetScriptableObject.AnimationSetNameLabel currentLabel = AnimationSetScriptableObject.AnimationSetNameLabel.Idle01;
            AnimationSetScriptableObject.AnimationSetNameLabel nextLabel = AnimationSetScriptableObject.AnimationSetNameLabel.None;
            if ((lsh != 0) || (lsv != 0))
            {
                _direction.x = lsh;
                _direction.z = -lsv;
                float ang = Utility.GetAngle(_direction) + _cameraLookDown.CameraYaw + 180.0f;
                float magnitude = _direction.magnitude;
                _direction.x = Utility.Sin(ang) * _moveSpeed;
                _direction.z = Utility.Cos(ang) * _moveSpeed;
                _characterController.transform.localRotation = Quaternion.Euler(0, Utility.GetAngle(_direction), 0);
                currentLabel = AnimationSetScriptableObject.AnimationSetNameLabel.Run01;
            }

            if( Input.GetKeyDown("joystick button 0") )
            {
                currentLabel = AnimationSetScriptableObject.AnimationSetNameLabel.HandGun_Cover01;
                nextLabel = AnimationSetScriptableObject.AnimationSetNameLabel.Idle01;
            }

            PlayAnimation(
                _animationSetScriptableObject.Get(currentLabel), 
                nextLabel != AnimationSetScriptableObject.AnimationSetNameLabel.None ? _animationSetScriptableObject.Get(nextLabel) : null );

        }


        _characterController.Move(_direction);
    }

    protected virtual void Control_TargetMove()
    {
        _direction.x = 0;
        _direction.y = 0;
        _direction.z = 0;

        //L Stick
        float lsh = Input.GetAxis("L_Stick_H");
        float lsv = Input.GetAxis("L_Stick_V");
        if ((lsh != 0) || (lsv != 0))
        {
            Vector3 temp;
            temp.x = lsh;
            temp.y = 0;
            temp.z = -lsv;
            float ang = Utility.GetAngle(_direction) + _cameraLookDown.CameraYaw + 180.0f;
            float magnitude = _direction.magnitude;
            temp.x = Utility.Sin(ang) * _moveSpeed;
            temp.z = Utility.Cos(ang) * _moveSpeed;
        }
    }

#if UNITY_EDITOR
    public partial class CharacterBaseEditor : Editor
    {
        private bool _isFoldControl = false;
        protected void OnInspectorControl()
        {
            CharacterBase chara = target as CharacterBase;

            if (_isFoldControl = EditorGUILayout.Foldout(_isFoldControl, "Control"))
            {
                chara._moveSpeed = EditorGUILayout.FloatField("_moveSpeed", chara._moveSpeed);
            }
        }

    }

#endif
}

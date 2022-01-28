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

        LookDownFreeMove,   // �����낵���R�ړ�
        TargetMove          // �ڕW�n�_�U��
        

    }

    protected MoveType _moveType = MoveType.LookDownFreeMove;

    protected Vector3 _direction = Vector3.zero;

    [SerializeField]
    protected float _moveSpeed = 0.1f;

    [SerializeField]
    protected bool _isControl = false;
    public bool IsControl { get { return _isControl; } set { _isControl = value; } }

    protected CharacterBase _leadCharacter = null;
    public CharacterBase LeadCharacter { get { return _leadCharacter; } set { _leadCharacter = value; } }

    #region TargetMove

    protected Vector3 _targetPos = Vector3.zero;

    protected bool _isMoveEnd = true;

    #endregion TargetMove

    public void SetCameraLookDown( CameraLookDown cameraLookDown )
    {
        _cameraLookDown = cameraLookDown;
    }

    protected virtual void UpdateInput()
    {
        if(_isControl == false)
        {
            _direction.x = 0;
            _direction.y = -0.1f;
            _direction.z = 0;
            PlayAnimation(_animationSetScriptableObject.Get( AnimationSetScriptableObject.AnimationSetNameLabel.Idle01));
            return;
        }

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
            //if (Input.GetKeyDown("joystick button 0"))
            //{
            //    PlayEndAnimation();
            //}
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

            //if( Input.GetKeyDown("joystick button 0") )
            //{
            //    currentLabel = AnimationSetScriptableObject.AnimationSetNameLabel.HandGun_Cover01;
            //    nextLabel = AnimationSetScriptableObject.AnimationSetNameLabel.Idle01;
            //}


            PlayAnimation(
                _animationSetScriptableObject.Get(currentLabel), 
                nextLabel != AnimationSetScriptableObject.AnimationSetNameLabel.None ? _animationSetScriptableObject.Get(nextLabel) : null );

        }
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

    protected virtual void UpdateFollow()
    {
        if( _isControl == true )
        {
            return;
        }

        if(_leadCharacter == null)
        {
            return;
        }

    }

    protected void UpdateMove()
    {
        _characterController.Move(_direction);
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
                chara._isControl = EditorGUILayout.Toggle("_isControl", chara._isControl);
                chara._moveSpeed = EditorGUILayout.FloatField("_moveSpeed", chara._moveSpeed);
            }
        }

    }

#endif
}

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

    [SerializeField]
    protected bool _isControl = false;
    public bool IsControl { get { return _isControl; } set { _isControl = value; } }

    private bool _isFollowing = false;

    private CharacterBase _leader = null;

    private Vector3 _leadPos = Vector3.zero;

    protected List<CharacterBase> _followCharacterList = new List<CharacterBase>();
    public List<CharacterBase> FollowCharacterList { get { return _followCharacterList; } }

    private float _followTime = 0.0f;

    #region TargetMove

    protected Vector3 _targetPos = Vector3.zero;

    protected bool _isMoveEnd = true;

    #endregion TargetMove

    public void SetCameraLookDown( CameraLookDown cameraLookDown )
    {
        _cameraLookDown = cameraLookDown;
    }

    protected virtual void PreUpdateControl()
    {
        _direction.x = 0;
        _direction.y = -0.1f;
        _direction.z = 0;

    }

    protected virtual void UpdateInput()
    {
        if(_isControl == false)
        {
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

    private const float FOLLOW_LENGTH = 5.0f;
    private const float FOLLOW_GOAL_LENGTH = 2.0f;
    private const float FOLLOW_END_LENGTH = 0.3f;
    private const float FOLLOW_TIMER = 3.0f;

    protected virtual void UpdateFollowPosition()
    {
        if (_followCharacterList.Count <= 0)
        {
            // 自分がリーダーではない
            return;
        }

        CharacterBase leader = _followCharacterList[0];

        for( int i = 1, count = _followCharacterList.Count; i < count; i++ )
        {
            CharacterBase follower = _followCharacterList[i];
            follower.Follow(leader);
            leader = follower;
        }

    }

    public bool Follow( CharacterBase leader)
    {
        if( _isFollowing == true )
        {
            return true;
        }

        _leader = leader;

        Vector3 sub = _leader.transform.localPosition - transform.localPosition;

        if (sub.magnitude > FOLLOW_LENGTH)
        {
            _leadPos = _leader.transform.localPosition - sub.normalized * FOLLOW_GOAL_LENGTH;
            _isFollowing = true;
        }
        

        return _isFollowing;
    }

    protected virtual void UpdateFollow()
    {
        if( _isControl == true )
        {
            return;
        }

        _followTime += Utility.GetGameTime();
        if (_followTime > FOLLOW_TIMER)
        {
            _followTime = 0.0f;
            _isFollowing = false;
        }

        if ( _isFollowing == false )
        {
            PlayAnimation(_animationSetScriptableObject.Get(AnimationSetScriptableObject.AnimationSetNameLabel.Idle01));
            return;
        }

        if( ( _leadPos - transform.localPosition).magnitude < FOLLOW_END_LENGTH )
        {
            _followTime = 0.0f;
            _isFollowing = false;
            if (Follow(_leader) == false) 
            {
                PlayAnimation(_animationSetScriptableObject.Get(AnimationSetScriptableObject.AnimationSetNameLabel.Idle01));
                return;
            }
        }

        Vector3 sub = _leadPos - transform.localPosition;
        sub.Normalize();
        _direction.x = sub.x * _moveSpeed;
        _direction.z = sub.z * _moveSpeed;

        _characterController.transform.localRotation = Quaternion.Euler(0, Utility.GetAngle(_direction), 0);
        PlayAnimation(_animationSetScriptableObject.Get(AnimationSetScriptableObject.AnimationSetNameLabel.Run01));

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public partial class CharacterBase
{
    /// <summary>
    /// �����ɍĐ�����A�j���̐�
    /// </summary>
    private const int ANIMATION_COUNT = 3;

    /// <summary>
    /// �A�j���[�^
    /// </summary>
    private Animator _animator = null;
    public Animator Animator { get { return _animator; } }

    /// <summary>
    /// �A�j���[�V�����Đ�
    /// </summary>
    private PlayableGraph _playableGraph;

    /// <summary>
    /// �A�j���[�V�����Đ��̃A�E�g�v�b�g
    /// </summary>
    private AnimationPlayableOutput _playableOutput;

    /// <summary>
    /// �A�j���[�V���������~�L�T�[
    /// </summary>
    private AnimationMixerPlayable _mixerPlayable;

    /// <summary>
    /// �Đ�����A�j���[�V�����N���b�v
    /// </summary>
    private AnimationClipPlayable[] _animationClipPlayableArray = new AnimationClipPlayable[ANIMATION_COUNT];

    /// <summary>
    /// 
    /// </summary>
    private bool[] _connectFlagArray = new bool[ANIMATION_COUNT];
    private AnimationSetScriptableObject _animationSetScriptableObject = null;

    [SerializeField]
    private float[] _animationTimeArray = new float[ANIMATION_COUNT];
    public float AnimationTime
    {
        get { return _animationTimeArray[GetCurrentAnimationIndex(0)]; }
        set
        {
            _animationTimeArray[GetCurrentAnimationIndex(0)] = value;
            _animationClipPlayableArray[GetCurrentAnimationIndex(0)].SetTime(_animationTimeArray[GetCurrentAnimationIndex(0)]);
        }
    }
    private bool _isPauseAnimation = false;
    public bool IsPauseAnimation { get { return _isPauseAnimation; } set { _isPauseAnimation = value; } }
    public AnimationClipPlayable CurrentAnimationClipPlayable { get { return _animationClipPlayableArray[GetCurrentAnimationIndex(0)]; } }
    private float _animationLerpTime = 0.0f;
    private float _animationCurrntLerpTime = 0.0f;
    private float _animationWeight = 0.0f;
    private AnimationSet _currentAnimationSet = null;
    public AnimationSet CurrentAnimationSet { get { return _currentAnimationSet; } }
    private AnimationSet _lastAnimationSet = null;
    private AnimationSet _nextAnimationSet = null;
    private Action _onEndAnimation = null;

    /// <summary>
    /// ���݂̃A�j���[�V�����Đ��i�K
    /// </summary>
    private int _clipIndex = 0;

    private int _currentAnimationIndex = 0;
    private int GetCurrentAnimationIndex(int index)
    {
        index = _currentAnimationIndex + index;
        if (index >= ANIMATION_COUNT) index -= ANIMATION_COUNT;
        else if (index < 0) index += ANIMATION_COUNT;
        return index;
    }
    private int _mixCount = 0;
    private float _subAnimationWeight = 0.0f;

    /// <summary>
    /// �S�L�������ʂŕێ�����
    /// </summary>
    [SerializeField]
    private static Dictionary<int, AnimationSetScriptableObject> _animationSetDataDict = new Dictionary<int, AnimationSetScriptableObject>();
    public Dictionary<int, AnimationSetScriptableObject> AnimationSetDataDict { get { return _animationSetDataDict; } }

    public class AnimationCommand
    {
        public AnimationSetScriptableObject AnimationSetData = null;
        public string AnimationName = string.Empty;
    }

    private Dictionary<string, AnimationCommand> _freeMoveAnimationCommandDict = new Dictionary<string, AnimationCommand>();

    /// <summary>
    /// ���[�V�����̃L�����Z���|�C���g�C�x���g
    /// </summary>
    private Action _onMotionCancelEvent = null;
    public Action OnMotionCancelEvent { set { _onMotionCancelEvent = value; } get { return _onMotionCancelEvent; } }

    /// <summary>
    /// Hit�C�x���g
    /// </summary>
    private Action _onMotionHitEvent = null;
    public Action OnMotionHitEvent { set { _onMotionHitEvent = value; } }

    #region Animation 

    /// <summary>
    /// �A�j���[�V�����֌W������
    /// </summary>
    private void InitializeAnimation()
    {
        // �A�j���[�^�[�̎擾
        _animator = GetComponent<Animator>();

        if( _animator == null )
        {
            return;
        }

        // IK�g�p�A���̃A�j���[�^�R���g���[���[���g�p
        _animator.runtimeAnimatorController = ResourceManager.LoadOnView<RuntimeAnimatorController>("3D/Character/Animation/IKAnimatorController");

        // Root���[�V�����͎g��Ȃ�
        _animator.applyRootMotion = false;

        _playableGraph = PlayableGraph.Create();

        _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);

        _mixerPlayable = AnimationMixerPlayable.Create(_playableGraph, ANIMATION_COUNT);

        _playableOutput.SetSourcePlayable(_mixerPlayable);
    }

    /// <summary>
    /// �A�j���[�V�����Z�b�g�̃��[�h
    /// </summary>
    /// <param name="setId"></param>
    /// <returns></returns>
    public static AnimationSetScriptableObject LoadAnimationSetData(int setId = 10001)
    {
        AnimationSetScriptableObject animationSetData = null;
        if (_animationSetDataDict.TryGetValue(setId, out animationSetData) == false)
        {
            string path = string.Format("3D/Character/Animation/AnimationSet{0:D5}", setId);
            animationSetData = ResourceManager.LoadOnView<AnimationSetScriptableObject>(path);
            _animationSetDataDict.Add(setId, animationSetData);
        }

        return animationSetData;
    }

    /// <summary>
    /// �A�j���[�V�����Z�b�g�̃��[�h
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public AnimationSetScriptableObject LoadAnimation(AnimationSetScriptableObject data)
    {
        AnimationSetScriptableObject animationSetData = null;
        if (_animationSetDataDict.TryGetValue(data.SetId, out animationSetData) == false)
        {
            _animationSetDataDict.Add(data.SetId, data);
        }

        _animationSetScriptableObject = data;

        return animationSetData;
    }

    /// <summary>
    /// �A�j���[�V�����̍Đ�
    /// </summary>
    /// <param name="setData"></param>
    /// <param name="animationName"></param>
    /// <param name="nextAnimationName"></param>
    /// <param name="lerpTime"></param>
    public void PlayAnimation(AnimationSetScriptableObject setData, string animationName, string nextAnimationName = "", float lerpTime = -1.0f, bool isForce = false)
    {
        AnimationSet animationSet = null, nextAnimationSet = null;
        int count = setData.AnimationSetList.Count;
        for (int i = 0; i < count; i++)
        {
            if (animationName.Equals(setData.AnimationSetList[i].Label))
            {
                animationSet = setData.AnimationSetList[i];
                if (nextAnimationName == string.Empty)
                {
                    break;
                }
                if (nextAnimationSet != null)
                {
                    break;
                }
            }
            else if (nextAnimationName.Equals(setData.AnimationSetList[i].Label))
            {
                nextAnimationSet = setData.AnimationSetList[i];
                if (animationSet != null)
                {
                    break;
                }
            }
        }


        PlayAnimation(animationSet, nextAnimationSet, lerpTime, isForce);
    }

    /// <summary>
    /// �A�j���[�V�����̍Đ�
    /// </summary>
    /// <param name="animationSet"></param>
    /// <param name="nextAnimationSet"></param>
    /// <param name="lerpTime"></param>
    public void PlayAnimation(AnimationSet animationSet, AnimationSet nextAnimationSet = null, float lerpTime = -1.0f, bool isForce = false, Action onEndAction = null)
    {
        if (lerpTime < 0.0f)
        {
            // �擪������������Ă���
            lerpTime = animationSet.ClipSetArray[0].LerpTime;
        }

        if (_lastAnimationSet == animationSet)
        {
            if (animationSet.ClipSetArray[_clipIndex].IsSameAnime == false)
            {
                if (isForce)
                {
                    lerpTime = 0.0f;
                }
                else
                {
                    return;
                }
            }
        }

        _currentAnimationSet = animationSet;
        _nextAnimationSet = nextAnimationSet;
        _lastAnimationSet = animationSet;

        _onEndAnimation = onEndAction;

        _clipIndex = 0;

        PlayAnimation(lerpTime);

    }

    /// <summary>
    /// �A�j���[�V�����Đ�
    /// </summary>
    /// <param name="lerpTime"></param>
    private void PlayAnimation(float lerpTime)
    {
        AnimationSet.ClipSet currentClipSet = _currentAnimationSet.ClipSetArray[(int)_clipIndex];
        AnimationClip clip = currentClipSet.AnimationClip;

        bool isLerping = false;
        if (_animationLerpTime <= 0.0f)
        {
            _animationWeight = 0.0f;
            for (int i = -1; i > -ANIMATION_COUNT; i--)
            {
                _playableGraph.Disconnect(_mixerPlayable, GetCurrentAnimationIndex(i));
                _connectFlagArray[GetCurrentAnimationIndex(i)] = false;

                if (_animationClipPlayableArray[GetCurrentAnimationIndex(i)].IsValid())
                {
                    _animationClipPlayableArray[GetCurrentAnimationIndex(i)].Destroy();
                }
                else
                {
                    // ����̍Đ��ŕ�Ԃ�����̂��Ȃ�
                    _animationWeight = 1.0f;
                }
            }
        }
        else
        {
            if (_connectFlagArray[GetCurrentAnimationIndex(1)] == true)
            {
                // ���ꂩ��Ȃ��悪���Ɏg���Ă���ꍇ�i�A�łȂǂŁj
                _playableGraph.Disconnect(_mixerPlayable, GetCurrentAnimationIndex(1));
                _connectFlagArray[GetCurrentAnimationIndex(1)] = false;

                if (_animationClipPlayableArray[GetCurrentAnimationIndex(1)].IsValid())
                {
                    _animationClipPlayableArray[GetCurrentAnimationIndex(1)].Destroy();
                }
            }

            _subAnimationWeight = _animationWeight;

            _animationWeight = 0.0f;

            isLerping = true;
        }

        _animationLerpTime = lerpTime;
        if (lerpTime > 0.0f)
        {
            _animationWeight = 0.0f;
            _animationCurrntLerpTime = 0.0f;
        }

        // index��i�߂�
        if (isLerping == false)
        {
            _mixCount = 2;
        }
        else
        {
            _mixCount = 3;
        }
        _currentAnimationIndex = GetCurrentAnimationIndex(1);
        _animationClipPlayableArray[GetCurrentAnimationIndex(0)] = AnimationClipPlayable.Create(_playableGraph, clip);
        _playableGraph.Connect(_animationClipPlayableArray[GetCurrentAnimationIndex(0)], 0, _mixerPlayable, GetCurrentAnimationIndex(0));
        _connectFlagArray[GetCurrentAnimationIndex(0)] = true;
        if (_mixCount == 2)
        {
            _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(-2), 0);
            _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(-1), 1 - _animationWeight);
            _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(0), _animationWeight);
        }
        else
        {
            _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(-2), (1 - _animationWeight) * (1 - _subAnimationWeight));
            _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(-1), (1 - _animationWeight) * _subAnimationWeight);
            _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(0), _animationWeight);
        }
        CheckIkMotion();

        // ���[�g���ړ������邩�`�F�b�N
        _animator.applyRootMotion = currentClipSet.IsApplyRootMotion;

        _playableGraph.Play();

        _animationTimeArray[GetCurrentAnimationIndex(0)] = 0.0f;
        _animationClipPlayableArray[GetCurrentAnimationIndex(0)].Pause();

        //_lastAnimationSet = animationSet;
    }

    /// <summary>
    /// IK�����郂�[�V�������Đ����Ă���ꍇ�ɂ�IK���A�N�e�B�u�ɂ���
    /// </summary>
    private void CheckIkMotion()
    {
        if (_currentAnimationSet == null)
        {
            return;
        }

        AnimationSet.ClipSet currentClipSet = _currentAnimationSet.ClipSetArray[(int)_clipIndex];
        Transform ikParent = null;

        // IK
        {
            IsLeftIK = currentClipSet.IsLeftIK;

            switch (currentClipSet.LeftIKIndex)
            {
                default:
                    ikParent = _positionList[(int)PositionObjName.WeaponR01_LHand01];
                    break;
                case 2:
                    ikParent = _positionList[(int)PositionObjName.WeaponR01_LHand02];
                    break;
                case 3:
                    ikParent = _positionList[(int)PositionObjName.WeaponR01_LHand03];
                    break;
            }
            LeftHandObj = ikParent;
        }

        {
            IsRightIK = currentClipSet.IsRightIK;

            switch (currentClipSet.RightIKIndex)
            {
                default:
                    ikParent = _positionList[(int)PositionObjName.WeaponL02_RHand01];
                    break;
                case 11:
                    ikParent = _positionList[(int)PositionObjName.WeaponR01_RHand01];
                    break;
            }
            RightHandObj = ikParent;
        }

    }

    /// <summary>
    /// ���[�V�����̃A�b�v�f�[�g
    /// </summary>
    private void UpdateAnimation()
    {
        if (_animationClipPlayableArray[GetCurrentAnimationIndex(0)].IsValid() == false)
        {
            return;
        }

        if (_animationLerpTime > 0.0f)
        {
            _animationCurrntLerpTime += Utility.GetGameTime();
            if (_animationCurrntLerpTime > _animationLerpTime)
            {
                _animationWeight = 1.0f;
                _animationLerpTime = 0.0f;
            }
            else
            {
                _animationWeight = _animationCurrntLerpTime / _animationLerpTime;
            }
            if (_mixCount == 2)
            {
                _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(-2), 0);
                _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(-1), 1 - _animationWeight);
                _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(0), _animationWeight);
            }
            else
            {
                _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(-2), (1 - _animationWeight) * (1 - _subAnimationWeight));
                _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(-1), (1 - _animationWeight) * _subAnimationWeight);
                _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(0), _animationWeight);
            }
        }
        else
        {
            for (int i = -1; i > -ANIMATION_COUNT; i--)
            {
                _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(i), 0);
            }
            _mixerPlayable.SetInputWeight(GetCurrentAnimationIndex(0), 1);
        }

        if (_isPauseAnimation == false)
        {
            for (int i = 0; i < _animationTimeArray.Length; i++)
            {
                if (_animationClipPlayableArray[i].IsValid())
                {
                    _animationTimeArray[i] += Time.deltaTime;
                    _animationClipPlayableArray[i].SetTime(_animationTimeArray[i]);
                }
            }
        }

        if (_nextAnimationSet != null)
        {
            AnimationSet.ClipSet currentClipSet = _currentAnimationSet.ClipSetArray[(int)_clipIndex];

            // �A�j���[�V�����Đ��I�����Ă����ꍇ
            if (_animationClipPlayableArray[GetCurrentAnimationIndex(0)].GetTime() >= currentClipSet.AnimationClip.length - _animationLerpTime)
            {
                // Loop�A�j���ł͂Ȃ��ꍇ
                if (_animationClipPlayableArray[GetCurrentAnimationIndex(0)].GetAnimationClip().isLooping == false )
                {
                    if (_currentAnimationSet.ClipSetArray.Length <= _clipIndex + 1 )
                    {
                        // �Ō�̃A�j�����ALoop�A�j������Ȃ��Ƃ��͎���Set�Ɉړ�
                        _onEndAnimation?.Invoke();
                        PlayAnimation(_nextAnimationSet);
                        _nextAnimationSet = null;
                    }
                    else
                    {
                        // �܂���������̂Ői�߂�
                        _clipIndex++;
                        PlayAnimation(_currentAnimationSet.ClipSetArray[(int)_clipIndex].LerpTime);
                    }
                }
            }
        }
    }

    public void PlayEndAnimation()
    {
        int finalClipIndex = _currentAnimationSet.ClipSetArray.Length - 1;

        if ( finalClipIndex - _clipIndex > 1)
        {
            // ���ݍĐ����Ă���A�j���ƍŏI�A�j���Ƃł͂Ȃ��肪�Ȃ��̂ŕ⊮���Ȃ����Ƃɂ��Ă݂�
            _clipIndex = finalClipIndex;
            PlayAnimation(0.0f);
        }
        else
        {
            _clipIndex = finalClipIndex;
            PlayAnimation(_currentAnimationSet.ClipSetArray[_clipIndex].LerpTime);
        }

    }

    public float GetAnimationRate()
    {
        if (_currentAnimationSet == null)
        {
            return -1.0f;
        }
        return _animationTimeArray[GetCurrentAnimationIndex(0)] / _currentAnimationSet.ClipSetArray[(int)_clipIndex].AnimationClip.length;
    }

    /// <summary>
    /// ���R�ړ��p�̃R�}���h�o�^
    /// </summary>
    /// <param name="animationSetData"></param>
    /// <param name="animationName"></param>
    public void RegistFreeMoveCommand(string commandName, AnimationSetScriptableObject animationSetData, string animationName)
    {
        AnimationCommand command = new AnimationCommand();
        command.AnimationSetData = animationSetData;
        command.AnimationName = animationName;
        _freeMoveAnimationCommandDict.Add(commandName, command);
    }
    public void PlayFreeMoveCommand(string commandName)
    {
        AnimationCommand command = _freeMoveAnimationCommandDict[commandName];
        PlayAnimation(command.AnimationSetData, command.AnimationName);
    }

    #endregion Animation

}

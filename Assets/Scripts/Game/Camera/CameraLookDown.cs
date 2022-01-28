using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookDown : MonoBehaviour
{
    private bool _isEnable = false;
    public bool IsEnable { get { return _isEnable; } }

    private Camera _camera = null;
    public Camera Camera { get { return _camera; } set { _camera = value; } }

    private CharacterBase _character = null;

    private Vector3 _currentPos = Vector3.zero;

    private Vector3 _targetPos = Vector3.zero;

    private Vector3 _cameraPos = Vector3.zero;

    private float _cameraYaw = 180.0f;
    public float CameraYaw { get { return _cameraYaw; } }

    [SerializeField]
    private float _targetYaw = 180.0f;

    private float _cameraPitch = 45.0f;

    [SerializeField]
    private float _targetPitch = 45.0f;

    private float _cameraDistance = 10.0f;

    [SerializeField]
    private float _targetDistance = 10.0f;

    [SerializeField]
    private float _cameraDelay = 0.08f;

    public static CameraLookDown CreateCamera()
    {
        GameObject obj = new GameObject("CameraLookDown");
        Camera camera = obj.AddComponent<Camera>();
        CameraLookDown cameraLookDown = obj.AddComponent<CameraLookDown>();
        cameraLookDown.Camera = camera;
        return cameraLookDown;
    }

    public void SetCharacter( CharacterBase character )
    {
        _character = character;
        _character.CameraLookDown = this;
    }

    private void UpdateCamera()
    {
        _targetPos = _character.transform.localPosition;
        _currentPos = (_targetPos - _currentPos) * _cameraDelay + _currentPos;
        _cameraYaw = (_targetYaw - _cameraYaw) * _cameraDelay + _cameraYaw;
        _cameraPitch = (_targetPitch - _cameraPitch) * _cameraDelay + _cameraPitch;
        _cameraDistance = (_targetDistance - _cameraDistance) * _cameraDelay + _cameraDistance;

        _cameraPos = _currentPos;

        _cameraPos.x += Utility.Sin(_cameraYaw) * _cameraDistance * Utility.Cos(_cameraPitch);
        _cameraPos.z += Utility.Cos(_cameraYaw) * _cameraDistance * Utility.Cos(_cameraPitch);
        _cameraPos.y += Utility.Sin(_cameraPitch) * _cameraDistance + 1.0f;

        _camera.transform.localPosition = _cameraPos;
        _camera.transform.localRotation = Quaternion.Euler(_cameraPitch, _cameraYaw + 180.0f, 0);
    }

    private void UpdateControl()
    {
        float trigger = Input.GetAxis("L_R_Trigger");
        if ( trigger > 0.05f )
        {
            _targetYaw += Utility.GetGameTime() * 64.0f;
        }
        else if( trigger < -0.05f )
        {
            _targetYaw -= Utility.GetGameTime() * 64.0f;
        }
    }

    private void Update()
    {
        UpdateControl();
        UpdateCamera();
    }

}

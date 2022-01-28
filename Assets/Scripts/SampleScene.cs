using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    private CameraLookDown _cameraLookDown = null;

    [SerializeField]
    private CharacterBase _character = null;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;
        _cameraLookDown = CameraLookDown.CreateCamera();
        _cameraLookDown.SetCharacter(_character);
    }

    // Update is called once per frame
    void Update()
    {
        Utility.Update();
    }
}

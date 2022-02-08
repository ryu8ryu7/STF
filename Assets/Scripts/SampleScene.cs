using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    private CameraLookDown _cameraLookDown = null;

    [SerializeField]
    private List<CharacterBase> _playerCharacterList = null;

    // Start is called before the first frame update
    void Awake()
    {
        GameSystem.CreateInstance();

        Application.targetFrameRate = 60;
        _cameraLookDown = CameraLookDown.CreateCamera();

        CharacterBase character = CharacterLoader.LoadModel();
        character.Initialize();
        _playerCharacterList.Add(character);

        character.IsControl = true;
        character.SetCameraLookDown(_cameraLookDown);
        _cameraLookDown.SetCharacter(character);

        //for (int i = 0, count = _playerCharacterList.Count; i < count; i++)
        //{
        //    _playerCharacterList[i].SetCameraLookDown(_cameraLookDown);
        //    if (i == 0)
        //    {
        //        _playerCharacterList[i].IsControl = true;
        //        _cameraLookDown.SetCharacter(_playerCharacterList[i]);
        //    }
        //    else
        //    {
        //        _playerCharacterList[i].IsControl = false;
        //    }
        //}

        //MakeCharacterList(_playerCharacterList[0]);
    }

    private void MakeCharacterList(CharacterBase leader)
    {
        leader.FollowCharacterList.Clear();

        leader.FollowCharacterList.Add(leader);

        for (int i = 0, count = _playerCharacterList.Count; i < count; i++)
        {
            if( _playerCharacterList[i] == leader )
            {
                continue;
            }

            _playerCharacterList[i].FollowCharacterList.Clear();
            leader.FollowCharacterList.Add(_playerCharacterList[i]);
        }
    }

    private void UpdateSystem()
    {
        if (Input.GetKeyDown("joystick button 1"))
        {
            if (_playerCharacterList[0].IsControl)
            {
                _playerCharacterList[0].IsControl = false;
                _cameraLookDown.SetCharacter(null);
            }
            else
            {
                _playerCharacterList[0].IsControl = true;
                _cameraLookDown.SetCharacter(_playerCharacterList[0]);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        Utility.Update();
        UpdateSystem();
    }
}

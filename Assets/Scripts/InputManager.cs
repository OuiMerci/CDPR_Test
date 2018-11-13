using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Fields
    private const float AXIS_MIN_VALUE = 0.8f;
    private PlayerBehaviour _player = null;
    private GameManager _gameManager;

    #endregion
    // Use this for initialization
    void Start () {
        _player = PlayerBehaviour.Instance;
        _gameManager = GameManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
        switch(_gameManager.GameState)
        {
            case GameManager.GameStates.Playing:
                CheckLaserInput();
                CheckHoldThrowInput();
                CheckCallInput();
                CheckCheatCodes();
                break;

            case GameManager.GameStates.Pause:
                break;

            default:
                Debug.Log("Error : GameState unkown.");
                break;
        }
        
	}

    private void CheckLaserInput()
    {
        if (Input.GetAxis("Laser") > AXIS_MIN_VALUE || Input.GetButton("Laser"))
        {
            // update the laser pointer position and state
            _player.UpdateLaserPointer();
        }
        else if (_player.LaserIsOn == true) // There is no input to activate the laser pointer --> update its state if it hasn't been done yet
        {
            _player.TurnOffLaserPointer();
        }
    }

    private void CheckHoldThrowInput()
    {
        if (Input.GetButtonDown("Hold"))
        {
            if(_player.IsHoldingBuddy)
            {
                // Start Throw Logic
                _player.ThrowBuddy();
            }
            else
            {
                // Start Holding logic
                _player.TryStartHoldBuddy();
            }
            
        }
    }

    private void CheckCallInput()
    {
        if (Input.GetButtonDown("CallBuddy"))
        {
            // Try to start buddy follow Logic
            _player.CallBuddy();
        }
    }

    private void CheckCheatCodes()
    {
        if (Input.GetButtonDown("NextRoom"))
        {
            // Teleport to next room
            _gameManager.TeleportToNextRoom();
        }
        else if (Input.GetKeyDown("1"))
        {
            // Teleport to room 1
            _gameManager.TeleportToRoom(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            // Teleport to room 2
            _gameManager.TeleportToRoom(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            // Teleport to room 3
            _gameManager.TeleportToRoom(2);
        }
        else if (Input.GetKeyDown("4"))
        {
            // Teleport to room 4
            _gameManager.TeleportToRoom(3);
        }
        else if (Input.GetKeyDown("5"))
        {
            // Teleport to room 5
            _gameManager.TeleportToRoom(4);
        }
        else if (Input.GetKeyDown("6"))
        {
            // Teleport to room 5
            _gameManager.TeleportToRoom(5);
        }
    }
}

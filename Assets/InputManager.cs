using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Fields
    private const float AXIS_MIN_VALUE = 0.8f;
    private PlayerBehaviour _player = null;

    #endregion
    // Use this for initialization
    void Start () {
        _player = PlayerBehaviour.Instance;
	}
	
	// Update is called once per frame
	void Update () {
        CheckInput();
	}

    private void CheckInput()
    {
        if (Input.GetAxis("Laser") > AXIS_MIN_VALUE || Input.GetButton("Laser") == false)
        {
            // update the laser pointer position and state
            _player.UpdateLaserPointer();
        }
        else if (_player.LaserIsOn == true) // There is no input to activate the laser pointer --> update its state if it hasn't been done yet
        {
            
        }
    }
}

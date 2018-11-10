using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    #region Fields
    private Light _light;
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
        _light = GetComponent<Light>();
        EnableLight(false);
	}

    private void OnEnable()
    {
        EventManager.OnLaserStateToggle += EnableLight;
    }

    private void OnDisable()
    {
        EventManager.OnLaserStateToggle -= EnableLight;
    }

    private void EnableLight(bool isOn)
    {
        _light.enabled = isOn;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    #region Fields
    [SerializeField] private GameObject _laserDisplay; // Object that represents the laser impact
    private Light _light;
    private PlayerBehaviour _player;
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
        _light = GetComponent<Light>();
        _player = PlayerBehaviour.Instance;
        ShowLaser(false);
    }

    private void OnEnable()
    {
        EventManager.OnLaserStateToggle += ShowLaser; //Show Laser or not depending on events
    }

    private void OnDisable()
    {
        EventManager.OnLaserStateToggle -= ShowLaser;
    }

    private void ShowLaser(bool isOn)
    {
        //_light.enabled = isOn;
        _player.LaserDisplaySphere.SetActive(isOn);
    }
    #endregion
}

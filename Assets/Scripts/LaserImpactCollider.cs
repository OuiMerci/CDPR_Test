using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserImpactCollider : MonoBehaviour {

    #region Fields
    private Collider _collider; //The collider used by buddies to check if the laser impact is visible
    #endregion

    #region Methods
    // Use this for initialization
    void Start()
    {
        _collider = GetComponent<Collider>();
        EnableCollider(false);
    }

    private void OnEnable()
    {
        EventManager.OnLaserStateToggle += EnableCollider; //Activate the collider or not depending on events
    }

    private void OnDisable()
    {
        EventManager.OnLaserStateToggle -= EnableCollider;
    }

    private void EnableCollider(bool isOn)
    {
        _collider.enabled = isOn;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserImpactCollider : MonoBehaviour {

    #region Fields
    private Collider _collider;
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
        EventManager.OnLaserStateToggle += EnableCollider;
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

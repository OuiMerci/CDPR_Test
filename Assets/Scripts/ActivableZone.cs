using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivableZone : ActivableObject {

    #region Fields
    [SerializeField] private bool _disableOnActivation; // Is this a one-time use only zone ?
    #endregion

    #region Methods

    private void OnTriggerEnter(Collider other)
    {
        // When a character enters the trigger, adds a point
        UpdateActivationPoints(+1);
    }

    private void OnTriggerExit(Collider other)
    {
        // When a character leaves the trigger, adds a point
        UpdateActivationPoints(-1);
    }

    public override void Activate()
    {
        if(_activated == false) // Only activate if it's deactivated
        {
            base.Activate();

            if (_disableOnActivation)
                gameObject.SetActive(false);
        }
    }

    public override void Deactivate()
    {
        if(_activated == true) // Only deactivate if it's activated
        {
            base.Deactivate();
        }
    }
    #endregion
}
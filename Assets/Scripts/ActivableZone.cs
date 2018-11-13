using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivableZone : ActivableObject {

    #region Fields
    [SerializeField] private bool _disableOnActivation;
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
		
	}
	
	private void OnActivation()
    {
        Debug.Log("Object activated : " + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        UpdateActivationPoints(+1);
    }

    private void OnTriggerExit(Collider other)
    {
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
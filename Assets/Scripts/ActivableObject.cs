using UnityEngine;
using System.Collections;

public abstract class ActivableObject : MonoBehaviour
{
    #region Fields
    [SerializeField] protected ActivableObject[] _linkedObjects;  //List of the objects that will be activated by this one
    [SerializeField] protected bool _disableParentsOnActivation;  //If (_activationPoinyNeeded > 1), the parents don't know when to deactivate themselves
    [SerializeField] protected ActivableObject[] _parentObjects; //List of the objects that activate this one
    [SerializeField] protected bool _activated; //Is this object activated ?
    [SerializeField] protected int _activationPointNeeded; // the amount of points this object need to be activated
    [SerializeField] protected int _activationPoints; //the amount of point this object currently has

    private bool _previousState;
    #endregion

    #region Methods
    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        // If an object is set as activated by default, set its _activationPoints to what it needs
        if (_activated)
        {
            _activationPoints = _activationPointNeeded;
        }
        else
        {
            _activationPoints = 0;
        }
    }

    /// <summary>
    /// Used to add or remove points to this object
    /// </summary>
    /// <param name="change"></param>
    public void UpdateActivationPoints(int change)
    {
        _activationPoints += change;

        UpdateStatus();
    }

    /// <summary>
    /// Used when the amount of point has changed, checks if it activated or deactivated the object
    /// </summary>
    void UpdateStatus()
    {
        if (_activationPoints >= _activationPointNeeded)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    /// <summary>
    /// (unused) Check if the status has changed.
    /// </summary>
    /// <returns></returns>
    protected bool CheckStatusChange()
    {
        if (_previousState == _activated)
            return false;
        else
        {
            _previousState = _activated;
            return true;
        }
    }

    /// <summary>
    /// (unused) Set a specific amount of points
    /// </summary>
    /// <param name="value"></param>
    protected void SetActivationPoints(int value)
    {
        _activationPoints = value;
    }

    /// <summary>
    /// Update the amount of point the "linked objects" have
    /// </summary>
    /// <param name="value"></param>
    public void UpdateLinkedObjects(int value)
    {
        foreach (ActivableObject child in _linkedObjects)
        {
            child.UpdateActivationPoints(value);
        }
    }

    public bool GetActiveBool()
    {
        return _activated;
    }

    protected void SetActiveBool(bool b)
    {
        _activated = b;
    }

    /// <summary>
    /// Start the activation logic for this object.
    /// </summary>
    public virtual void Activate()
    {
        _activated = true;

        if(_linkedObjects.Length > 0)
        {
            UpdateLinkedObjects(+1);
        }

        if (_disableParentsOnActivation)
        {
            foreach(ActivableObject parent in _parentObjects)
            {
                parent.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Start the deactivation logic for this object.
    /// </summary>
    public virtual void Deactivate()
    {
        _activated = false;

        if (_linkedObjects.Length > 0)
        {
            UpdateLinkedObjects(-1);
        }
    }
    #endregion
}

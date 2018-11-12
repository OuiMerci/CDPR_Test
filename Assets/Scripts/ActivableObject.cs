using UnityEngine;
using System.Collections;

public abstract class ActivableObject : MonoBehaviour
{
    #region Fields
    [SerializeField] protected ActivableObject[] _linkedObjects;
    [SerializeField] protected bool _activated;
    [SerializeField] protected int _activationPointNeeded;
    [SerializeField] protected int _activationPoints;

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

    public void UpdateActivationPoints(int change)
    {
        _activationPoints += change;

        UpdateStatus();
    }

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

    protected void SetActivationPoints(int value)
    {
        _activationPoints = value;
    }

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

    public virtual void Activate()
    {
        _activated = true;

        if(_linkedObjects.Length > 0)
        {
            UpdateLinkedObjects(+1);
        }
    }

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

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    #region Fields
    private static EventManager _instance;

    public delegate void LaserStateToggle(bool isOn);
    public static event LaserStateToggle OnLaserStateToggle;
    #endregion

    #region Properties
    static public EventManager Instance
    {
        get { return _instance; }
    }
    #endregion

    #region Methods

    private void Awake()
    {
        _instance = this;
    }

    static public void FireLaserStateEvent(bool isOn)
    {
        OnLaserStateToggle(isOn);
    }
    #endregion


}
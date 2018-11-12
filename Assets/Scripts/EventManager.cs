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

    public delegate void RabbitSafeEvent(HarmlessNPC rabbit);
    public static event RabbitSafeEvent OnRabbitSafe;
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
        if(OnLaserStateToggle != null) //Make sure the event has at least one listener
            OnLaserStateToggle(isOn);
    }

    static public void FireRabbitSafeEvent(HarmlessNPC rabbit)
    {
        if (OnRabbitSafe != null) //Make sure the event has at last one listener
            OnRabbitSafe(rabbit);
    }
    #endregion


}
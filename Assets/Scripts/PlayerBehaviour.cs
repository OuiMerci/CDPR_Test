using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    #region Fields
    [SerializeField] private Transform _laserPointer; //Used to update, the laser that controls the pet's position
    [SerializeField] private int _laserPointerMask; // We use this to avoid collision with the player's capsule
    public BuddyBehaviour buddy;

    const float LASER_MAX_DISTANCE = 100; // Define a const for raycast maxDistance
    private static PlayerBehaviour _instance;
    private bool _laserIsOn = false;
    private Camera _cam = null;
    #endregion Fields

    #region Properties
    static public PlayerBehaviour Instance
    {
        get { return _instance; }
    }

    public bool LaserIsOn
    {
        get { return _laserIsOn; }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        _laserPointerMask = ~(1 << 9);
        _cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void UpdateLaserPointer()
    {
        _laserPointer.position = _cam.transform.position;
        _laserPointer.eulerAngles = _cam.transform.eulerAngles;

        if (_laserIsOn == false)
        {
            _laserIsOn = true;
            EventManager.FireLaserStateEvent(true);
        }
    }

    /// <summary>
    /// Gets the destination hit by the Laser Pointer.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="isHittingBuddy"></param>
    /// <returns> Returns true if a valid collider is hit, otherwise, returns false (the value of destination is then invalid).</returns>
    public bool RequestLaserRaycast(out Vector3 destination, out bool isHittingBuddy)
    {
        RaycastHit hit;

        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, LASER_MAX_DISTANCE, _laserPointerMask))
        {
            if(hit.collider.tag == "Buddy")
            {
                isHittingBuddy = true;
            }
            else
            {
                isHittingBuddy = false;
            }

            _laserPointer.position = _cam.transform.position;
            _laserPointer.eulerAngles = _cam.transform.eulerAngles;

            destination = hit.point;

            return true;
        }

        isHittingBuddy = false;
        destination = Vector3.zero;
        return false;
    }

    public void TurnOffLaserPointer()
    {
        if(LaserIsOn == true)
        {
            _laserIsOn = false;
            EventManager.FireLaserStateEvent(false);
        }
    }
    #endregion
}

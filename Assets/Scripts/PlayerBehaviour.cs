using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    #region Fields
    [SerializeField] private Transform _laserPointer; //Used to update, the laser that controls the pet's position
    [SerializeField] private Transform _laserImpactCollider; // This collider is used for easier detection with raycasts
    [SerializeField] private Transform _buddyHolder; // The transforms that controls the position and orientation of buddy while we hold it
    [SerializeField] private LayerMask _laserPointerMask; // We use this to avoid collision with the player's capsule or the laserImpactCollider
    [SerializeField] private float _buddyHoldMaxDistance; // Maximum distance at which the player can start holding Buddy
    [SerializeField] private float _buddyThrowForce; // Maximum distance at which the player can start holding Buddy

    private static PlayerBehaviour _instance;
    private bool _laserIsOn = false;
    private Camera _cam = null;
    private float _raycastDistance;
    private BuddyBehaviour _buddy;
    private bool _isHoldingBuddy;

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

    public bool IsHoldingBuddy
    {
        get { return _isHoldingBuddy; }
    }

    public Transform BuddyHolder
    {
        get { return _buddyHolder; }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        //_laserPointerMask = ~(1 << 9);
        _buddy = BuddyBehaviour.Instance;
        _cam = Camera.main;
        _raycastDistance = GameManager.Instance.MAX_RAYCAST_DISTANCE;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(_cam.transform.position, _cam.transform.forward * 50);
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
        RaycastHit resultHit;

        if(GetImpactPoint(out resultHit))
        {
            if (resultHit.collider.tag == "Buddy")
            {
                isHittingBuddy = true;
            }
            else
            {
                isHittingBuddy = false;
            }

            _laserImpactCollider.transform.position = resultHit.point;
            destination = resultHit.point;

            return true;
        }

        isHittingBuddy = false;
        destination = Vector3.zero;
        return false;        
    }

    private bool GetImpactPoint(out RaycastHit resultHit)
    {
        // initialize out value
        resultHit = new RaycastHit();

        // Raycast to get what the laser is touching
        RaycastHit hit;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, _raycastDistance, _laserPointerMask))
        {
            // Check if the laser hits the mirror, if yes, we use another raycast to compute the reflection hit
            if (hit.collider.tag == "Mirror")
            {
                RaycastHit hitReflect;
                Vector3 reflectDirection = Vector3.Reflect(_cam.transform.forward, hit.collider.transform.forward); //Computes the reflection, which is the direction of the new raycast
                Debug.DrawRay(hit.point, reflectDirection * 10, Color.green);
                Debug.DrawRay(_cam.transform.position, _cam.transform.forward * 10, Color.red);

                // The new raycast, going from the mirror, in the direction of the reflection
                if (Physics.Raycast(hit.point, reflectDirection, out hitReflect, _raycastDistance, _laserPointerMask))
                {
                    // Overwrite the laser pointer position, the light is now coming from the mirror
                    _laserPointer.position = hit.point;
                    _laserPointer.forward = reflectDirection;

                    resultHit = hitReflect;
                    return true;
                }

                return false;
            }

            resultHit = hit;
            return true;
        }

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

    public void TryStartHoldBuddy()
    {
        Debug.Log("Try Start Holding buddy");
        float currentDistance = Vector3.Distance(transform.position, _buddy.transform.position);

        if(_buddy.IsVisible && currentDistance <= _buddyHoldMaxDistance)
        {
            _isHoldingBuddy = true;
            _buddy.StartHold();
        }
    }

    public void ThrowBuddy()
    {
        Vector3 throwForce = _cam.transform.forward * _buddyThrowForce;
        Debug.Log("ThrowForce : " + throwForce + " cam forward : " + _cam.transform.forward.normalized + " budduTF : " + _buddyThrowForce);
        Debug.DrawRay(_buddyHolder.position, _cam.transform.forward, Color.green, 60);
        //Debug.DrawRay(_buddyHolder.position, throwForce, Color.red, 60);
        _isHoldingBuddy = false;
        _buddy.ApplyThrow(throwForce);
    }

    public void CancelHold()
    {
        _isHoldingBuddy = false;
    }

    // Ask Buddy to follow the player
    public void CallBuddy()
    {
        // add sound here
        _buddy.TryFollowPlayer();
        Debug.Log("CALLING BUDDY");
    }


    #endregion
}

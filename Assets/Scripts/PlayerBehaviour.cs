using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : Character {

    #region Fields
    [SerializeField] private Transform _laserPointer; //Used to update, the laser that controls the pet's position
    [SerializeField] private Transform _laserImpactCollider; // This collider is used for easier detection with raycasts
    [SerializeField] private Transform _buddyHolder; // The transforms that controls the position and orientation of buddy while we hold it
    [SerializeField] private LayerMask _laserPointerMask; // We use this to avoid collision with the player's capsule or the laserImpactCollider
    [SerializeField] private float _buddyHoldMaxDistance; // Maximum distance at which the player can start holding Buddy
    [SerializeField] private float _buddyThrowForce; // How far will the player throw Buddy
    [SerializeField] private GameObject _laserDisplay;
    [SerializeField] private float _laserDisplaySizeRatio = 0;


    private static PlayerBehaviour _instance;
    private bool _laserIsOn = false;
    private Camera _cam = null;
    private float _raycastDistance;
    private List<BuddyBehaviour> _buddyList;
    private bool _isHoldingBuddy;
    private BuddyBehaviour _heldBuddy;
    private bool _onMovingBlock;

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

    public GameObject LaserDisplaySphere
    {
        get { return _laserDisplay; }
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
        _buddyList = new List<BuddyBehaviour>();
        _cam = Camera.main;
        _raycastDistance = GameManager.Instance.MAX_RAYCAST_DISTANCE;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(_cam.transform.position, _cam.transform.forward * 50);
	}

    public void UpdateLaserPointer()
    {
        // Update the spotlight position
        // (As is makes the other lights very buggy for me, the light comopnent is disabled. I replaced it by an ugly red sphere.)
        //_laserPointer.position = _cam.transform.position;
        //_laserPointer.eulerAngles = _cam.transform.eulerAngles;

        // Update the sphere position, no more light bugs, but much more raycast requests
        Vector3 dest = Vector3.zero;
        bool hittingBuddy = false;
        RequestLaserRaycast(out dest, out hittingBuddy);

        _laserDisplay.transform.position = dest;
        float sizeRatio = Vector3.Distance(transform.position, _laserDisplay.transform.position) * _laserDisplaySizeRatio;

        _laserDisplay.transform.localScale = new Vector3 (sizeRatio, sizeRatio, sizeRatio);

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
                    //_laserPointer.position = hit.point;
                    //_laserPointer.forward = reflectDirection;

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
        BuddyBehaviour buddyToHold = null; ;
        float shortestDistance = _buddyHoldMaxDistance + 1;

        foreach(BuddyBehaviour buddy in _buddyList)
        {
            if (buddy.IsVisible)
            {
                float distanceFromPlayer = Vector3.Distance(transform.position, buddy.transform.position);

                if(distanceFromPlayer < shortestDistance)
                {
                    shortestDistance = distanceFromPlayer;
                    buddyToHold = buddy;
                }
            }
        }

        if(buddyToHold != null)
        {
            _isHoldingBuddy = true;
            _heldBuddy = buddyToHold;
            _heldBuddy.StartHold();
        }
    }

    public void ThrowBuddy()
    {
        Vector3 throwForce = _cam.transform.forward * _buddyThrowForce;

        _isHoldingBuddy = false;
        _heldBuddy.ApplyThrow(throwForce);
        _heldBuddy = null; //We're not holding anyone now, Reset _heldBuddy value
    }

    public void CancelHold()
    {
        _isHoldingBuddy = false;
    }

    // Ask Buddies to follow the player
    public void CallBuddy()
    {
        // add sound here
        foreach (BuddyBehaviour buddy in _buddyList)
        {
            buddy.TryFollowPlayer();
        }
    }

    public void AddNewBuddy(BuddyBehaviour buddy)
    {
        _buddyList.Add(buddy);
    }

    public void RemoveBuddy(BuddyBehaviour buddy)
    {
        _buddyList.Remove(buddy);
    }

    public void StartMovingBlock()
    {
        _onMovingBlock = true;
        //_rigidbody.isKinematic = true;
    }
    #endregion
}

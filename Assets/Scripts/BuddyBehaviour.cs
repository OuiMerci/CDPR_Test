using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuddyBehaviour : MonoBehaviour {

    #region Fields
    private enum BuddyStates
    {
        Normal,
        Hold,
        Chasing,
        Thrown
    }

    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private Animator _anim;
    [SerializeField] private LayerMask _laserVisionMask;
    [SerializeField] private Renderer _renderer;

    static private BuddyBehaviour _instance;
    private NavMeshAgent _navAgent;
    private PlayerBehaviour _player;
    private float _raycast_distance;
    private BuddyStates _state;
    private Rigidbody _rigidBody;

    private int _isRunningHash = Animator.StringToHash("IsRunning");
    private int _isStoppedHash = Animator.StringToHash("IsStopped");
    #endregion

    #region Properties
    static public BuddyBehaviour Instance
    {
        get { return _instance; }
    }

    public bool IsVisible
    {
        get { return _renderer.isVisible; }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _instance = this;
        _state = BuddyStates.Normal;
    }

    // Use this for initialization
    void Start () {
        _navAgent = GetComponent<NavMeshAgent>();
        _rigidBody = GetComponent<Rigidbody>();
        _player = PlayerBehaviour.Instance;
        _raycast_distance = GameManager.Instance.MAX_RAYCAST_DISTANCE;
        SetBuddyStopped(true);
	}

    private void OnEnable()
    {
        EventManager.OnLaserStateToggle += OnLaserStateToggle;
    }

    private void OnDisable()
    {
        EventManager.OnLaserStateToggle -= OnLaserStateToggle;
    }

    // Update is called once per frame
    void Update () {
        switch(_state)
        {
            case BuddyStates.Normal:
                if (_player.LaserIsOn)
                {
                    TryFollowLaserPointer();
                }

                break;

            case BuddyStates.Hold:
                transform.position = _player.BuddyHolder.position;
                transform.eulerAngles = _player.BuddyHolder.eulerAngles;
                break;

            case BuddyStates.Thrown:
                break;

            case BuddyStates.Chasing:
                break;

            default:
                Debug.Log("Error : BuddyState Unknown");
                break;
        }
	}

    private void OnLaserStateToggle(bool isOn)
    {
        if(isOn == false)
        {
            SetBuddyStopped(true);
        }
    }

    private void TryFollowLaserPointer()
    {
        Vector3 destination;
        bool isHittingBuddy;
        bool validHit = _player.RequestLaserRaycast(out destination, out isHittingBuddy);

        if(validHit)
        {
            if(isHittingBuddy)
            {
                SetBuddyStopped(true);
            }
            else
            {
                IsDestinationVisible(destination);
            }
        }
    }

    private void IsDestinationVisible(Vector3 laserHit)
    {
        RaycastHit hit;
        Vector3 direction = (laserHit - _raycastOrigin.position).normalized;
        if (Physics.Raycast(_raycastOrigin.position, direction, out hit, _raycast_distance, _laserVisionMask))
        {
            if(hit.collider.tag == "Laser")
            {
                GoToDestination(hit.point, false);
            }
            else
            {
                Debug.Log("Buddy doesn't see the Laser Pointer because of object : " + hit.collider.name);
                // Add a reaction to not seeing the dot ? Currently, nothing.
            }
        }
    }

    private void GoToDestination(Vector3 dest, bool running)
    {
        _navAgent.SetDestination(dest);

        // Check if Buddy is close enougth to the his destination.
        if(CheckDestinationReached())
        {
            SetBuddyStopped(true);
            return;
        }

        SetBuddyStopped(false);
        _navAgent.speed = running ? _runSpeed : _walkSpeed;

        if(running)
        {
            _navAgent.speed = _runSpeed;
            _anim.SetBool(_isRunningHash, true);
        }
        else
        {
            _navAgent.speed = _walkSpeed;
            _anim.SetBool(_isRunningHash, false);
        }
    }

    private bool CheckDestinationReached()
    {
        if (Vector3.Distance(_navAgent.destination, _navAgent.transform.position) <= _navAgent.stoppingDistance)
        {
            if (_navAgent.hasPath == false|| _navAgent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }

    private void SetBuddyStopped(bool isStopped)
    {
        _navAgent.isStopped = isStopped;
        _anim.SetBool(_isStoppedHash, isStopped);
    }

    private void ChangeBuddyState(BuddyStates newState)
    {
        _state = newState;
    }

    public void StartHold()
    {
        Debug.Log("Start Holding buddy");
        ChangeBuddyState(BuddyStates.Hold);
        _navAgent.enabled = false;
        // rigid body kinematic ?
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision : " + collision.collider.name);
    }

    public void ApplyThrow(Vector3 force)
    {
        ChangeBuddyState(BuddyStates.Thrown);
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.AddForce(force, ForceMode.Impulse);
    }
    #endregion
}

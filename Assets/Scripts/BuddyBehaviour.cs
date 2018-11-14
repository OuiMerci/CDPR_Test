using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuddyBehaviour : Character {

    #region Fields
    private enum BuddyStates
    {
        Normal,
        Held,
        Chasing,
        Thrown
    }

    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private Animator _anim;
    [SerializeField] private LayerMask _laserVisionMask;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _playerFollowStopDistance;
    [SerializeField] private float _tempDrag; //If Drag = 0, buddy may slide on the floor when thrown, it at 1 or more, it will fall too slowly. This value is used to apply a temp drag when a buddy hits the floor 

    static private BuddyBehaviour _instance;
    private NavMeshAgent _navAgent;
    private PlayerBehaviour _player;
    private float _raycast_distance;
    private BuddyStates _state; // Stores which state the rabbit is currently in
    private Rigidbody _rigidBody;
    private bool _followingPlayer;
    private float _baseStopDistance; // This stored the original stoppingDistance value (its value is not the same when following the player)
    private NavMeshPath _pathChecker; // Used when checking if a path to the player is available
    private GameManager _gameManager;
    private HarmlessNPC _chasedRabbit; // Used to store the instance of a followed rabbit

    private int _isRunningHash = Animator.StringToHash("IsRunning"); // Use hash to avoid using a string, for optimisation
    private int _isStoppedHash = Animator.StringToHash("IsStopped");
    #endregion

    #region Properties
    //static public BuddyBehaviour Instance
    //{
    //    get { return _instance; }
    //}

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
        _baseStopDistance = _navAgent.stoppingDistance;
        _pathChecker = new NavMeshPath();
        _gameManager = GameManager.Instance;

        SetBuddyStopped(true);
	}

    private void OnEnable()
    {
        EventManager.OnLaserStateToggle += OnLaserStateToggle;
        EventManager.OnRabbitSafe += OnRabbitSafe;
    }

    private void OnDisable()
    {
        EventManager.OnLaserStateToggle -= OnLaserStateToggle;
        EventManager.OnRabbitSafe -= OnRabbitSafe;
    }

    // Update is called once per frame
    void Update () {
        switch(_state)
        {
            case BuddyStates.Normal:

                if (CheckRabbits())
                    break;

                if (_player.LaserIsOn)
                {
                    TryFollowLaserPointer();
                }
                else if (_followingPlayer)
                {
                    GoToDestination(_player.transform.position, true, true);
                }
                break;

            case BuddyStates.Held:

                if (CheckRabbits())
                {
                    _player.CancelHold();
                    EndThrow(false);
                    break;
                }

                transform.position = _player.BuddyHolder.position;
                transform.eulerAngles = _player.BuddyHolder.eulerAngles;
                break;

            case BuddyStates.Thrown:
                break;

            case BuddyStates.Chasing:
                GoToDestination(_chasedRabbit.transform.position, true);
                break;

            default:
                Debug.Log("Error : BuddyState Unknown");
                break;
        }
	}

    // Events
    private void OnLaserStateToggle(bool isOn)
    {
        if(isOn == false)
        {
            SetBuddyStopped(true);
        }
    }

    private void OnRabbitSafe(HarmlessNPC rabbit)
    {
        if(rabbit == _chasedRabbit)
        {
            ChangeBuddyState(BuddyStates.Normal);
            _chasedRabbit = null;
            SetBuddyStopped(true);
        }
    }

    /// <summary>
    /// If the laser pointer is visible, ask this buddy to follow it.
    /// </summary>
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
                if(IsObjectVisible(destination, "Laser"))
                    GoToDestination(destination, false);
            }
        }
    }

    /// <summary>
    /// Check if an object is visible by this buddy
    /// </summary>
    /// <param name="laserHit"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    private bool IsObjectVisible(Vector3 laserHit, string tag)
    {
        RaycastHit hit;
        Vector3 direction = (laserHit - _raycastOrigin.position).normalized;
        if (Physics.Raycast(_raycastOrigin.position, direction, out hit, _raycast_distance, _laserVisionMask))
        {
            //Debug.DrawRay(_raycastOrigin.position, direction * 20);
            if (hit.collider.tag == tag)
            {
                return true;
            }
            // Add a reaction to not seeing the dot ? Currently, nothing.
            return false;
        }

        return false;
    }

    /// <summary>
    /// Set a destination for this buddy, while setting relevant variables (animation, speed...)
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="running"></param>
    /// <param name="following"></param>
    private void GoToDestination(Vector3 dest, bool running, bool following = false)
    {
        if (_navAgent.enabled == false)
        {
            Debug.Log("GoToDestination -> navAgent is disabled");
            return;
        }

        ResetDrag();

        // If a destination is set for buddy, he will stop following the player, except when the variable "following" is specified
        if(following == false)
        {
            _followingPlayer = false;
            _navAgent.stoppingDistance = _baseStopDistance;
        }

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

    /// <summary>
    /// Checks if a navmesh path to the player is available.
    /// </summary>
    /// <param name="dest">Destination to test</param>
    /// <returns>If a path is available the function returns true, otherwise is returns false.</returns>
    private bool CheckPathAvailable(Vector3 dest)
    {
        _navAgent.CalculatePath(_player.transform.position, _pathChecker);
        if(_pathChecker.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check if this buddy reached his destination.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Used when we need this buddy to stop (ie : when its destination is reach or it is held by the player)
    /// </summary>
    /// <param name="isStopped"></param>
    private void SetBuddyStopped(bool isStopped)
    {
        if (_navAgent.enabled == false)
        {
            Debug.Log("SetBuddyStopped -> navAgent is disabled");
            return;
        }

        _navAgent.isStopped = isStopped;
        _anim.SetBool(_isStoppedHash, isStopped);
    }

    /// <summary>
    /// Changes the state of this buddy to a specific state
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeBuddyState(BuddyStates newState)
    {
        _state = newState;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_state == BuddyStates.Thrown)
        {
            if(collision.collider.tag == "Floor")
            {
                EndThrow();
            }
            else if(collision.collider.tag == "BlockTop")
            {
                EndThrow(true, false);
            }
        }
    }

    /// <summary>
    /// Reset variable for going back to the normal state
    /// </summary>
    /// <param name="changeState"></param>
    private void EndThrow(bool changeState = true, bool _activateNavagent = true)
    {
        _navAgent.enabled = _activateNavagent;
        _rigidBody.drag = _tempDrag;

        if(changeState)
            ChangeBuddyState(BuddyStates.Normal);
    }

    /// <summary>
    /// Check if any rabbit of this GameRoom is visible
    /// </summary>
    /// <returns></returns>
    private bool CheckRabbits()
    {
        GameRoom activeRoom = _gameManager.ActiveRoom;
        if(activeRoom != null)
        {
            foreach (HarmlessNPC rabbit in _gameManager.ActiveRoom.RabbitList)
            {
                if (rabbit.IsSafe == false && IsObjectVisible(rabbit.transform.position, "Rabbit"))
                {
                    Debug.Log("RABBIT VISIBLE");
                    StartChasingRabbit(rabbit);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Set variables for the Chasing state
    /// </summary>
    /// <param name="rabbit"></param>
    private void StartChasingRabbit(HarmlessNPC rabbit)
    {
        _chasedRabbit = rabbit;
        ChangeBuddyState(BuddyStates.Chasing);
    }

    /// <summary>
    /// When a new destination is set or when the player hold buddy, we make sure to reset the drag to 0
    /// </summary>
    private void ResetDrag()
    {
        _rigidBody.drag = 0;
    }

    /// <summary>
    /// Set variables to start the Held state
    /// </summary>
    public void StartHold()
    {
        if (_state == BuddyStates.Normal)
        {
            ChangeBuddyState(BuddyStates.Held);
            SetBuddyStopped(true);
            _navAgent.enabled = false;
            _followingPlayer = false;
            ResetDrag();
        }
    }

    /// <summary>
    /// Applies a force the buddy's rigibody in a force and direction set by the player
    /// </summary>
    /// <param name="force"></param>
    public void ApplyThrow(Vector3 force)
    {
        ChangeBuddyState(BuddyStates.Thrown);
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.AddForce(force, ForceMode.Impulse);
    }

    /// <summary>
    /// If a navmeshPath to the player exists, start following the player
    /// </summary>
    public void TryFollowPlayer()
    {
        if(_state == BuddyStates.Normal && CheckPathAvailable(_player.transform.position))
        {
            _followingPlayer = true;
            _navAgent.stoppingDistance = _playerFollowStopDistance;
        }
    }

    public override void StartFollowBlockMovement()
    {
        base.StartFollowBlockMovement();
        Debug.Log("OOOOOOOOOO ok disabled 1");
        _navAgent.enabled = false;
    }

    public override void EndFollowBlockMovement()
    {
        base.EndFollowBlockMovement();
        _navAgent.enabled = true;
    }
    #endregion
}

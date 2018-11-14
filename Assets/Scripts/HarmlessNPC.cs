using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HarmlessNPC : ActivableObject {

    #region Fields
    [SerializeField] private Transform _activatedDestination; // Where this npc goes on activation
    [SerializeField] private Transform _deactivatedDestination; // Where this npc goes on deactivation
    private Animator _anim;
    private NavMeshAgent _navAgent;
    private int _moveHash = Animator.StringToHash("Move"); // Use hash to avoid using a string, for optimisation
    private bool _moving;
    private bool _isSafe;

    //We use two colliders to control the activation/deactivation, so we keep track of characters that enter the activation zone
    //When a character leaves the deactivation zone, we update the activationPoints only if he's in that list
    //For this prototype, two booleans "_playerinside" and "_buddyInside" would be enough, but in the case i'd add other characters, at list better
    private List<GameObject> _charactersToCheck;
    #endregion

    #region Properties
    public bool IsSafe
    {
        get { return _isSafe; }
    }

    public List<GameObject> CharactersToCheck
    {
        get { return _charactersToCheck; }
    }
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
        _anim = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _charactersToCheck = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if(_moving) //when this npc is moving, we constantly check if the destination is reached, as it is needed to free chasing buddies 
        {
            CheckDestinationReached();
        }
	}

    /// <summary>
    /// Check if destination is reached, if yes, fire an event
    /// </summary>
    private void CheckDestinationReached()
    {
        if (Vector3.Distance(_navAgent.destination, _navAgent.transform.position) <= _navAgent.stoppingDistance)
        {
            if (_navAgent.hasPath == false || _navAgent.velocity.sqrMagnitude == 0f)
            {
                _anim.SetBool(_moveHash, false);
                _moving = false;

                if(_activated)
                {
                    _isSafe = true;
                    EventManager.FireRabbitSafeEvent(this);
                }
            }
        }
    }

    /// <summary>
    /// Set the destination for this NPC
    /// </summary>
    /// <param name="dest"></param>
    private void GoToDestination(Vector3 dest)
    {
        _navAgent.SetDestination(dest);
        _anim.SetBool(_moveHash, true);
        _moving = true;
    }

    /// <summary>
    /// Does the activation logic for this NPC
    /// </summary>
    public override void Activate()
    {
        if(_activated == false)
        {
            base.Activate();
            GoToDestination(_activatedDestination.position);
        }
    }

    /// <summary>
    /// Does the deactivation logic for this NPC
    /// </summary>
    public override void Deactivate()
    {
        if (_activated == true)
        {
            base.Deactivate();
            _isSafe = false;
            GoToDestination(_deactivatedDestination.position);
        }
    }

    /// <summary>
    /// Add a character to the list of characters that triggered this NPC
    /// </summary>
    /// <param name="character"></param>
    public void AddCharacterToList(GameObject character)
    {
        _charactersToCheck.Add(character);
    }

    /// <summary>
    /// Check if a character triggered this rabbit and hasn't left yet
    /// </summary>
    /// <param name="character"></param>
    public bool IsCharacterInList(GameObject character)
    {
        return _charactersToCheck.Contains(character);
    }
    #endregion
}

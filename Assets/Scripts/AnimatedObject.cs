using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatedObject : ActivableObject {

    #region Fields
    [SerializeField] private Collider _coll;
    [SerializeField] private NavMeshObstacle _navObstacle;
    [SerializeField] private bool _disableCollOnActivation; // Wether or not the collider must be disable on activation
    [SerializeField] private string _activateAnim; // Name of the activation animation
    [SerializeField] private string _deactivateAnim; // Name of the deactivation animation
    [SerializeField] private bool _deactivateObstacle; // When deactivated, this object cuts out the navmesh, path become available when activated
    [SerializeField] private bool _swapAnimations; // Simply swap the activate/deactivate animations. I should also add an easy way to set the initial state, but won't have time for that prototype. :(

    private Animator _anim;
    private bool _animated;
    private int _deactivateHash; // Using a hash is faster than using a string
    private int _activateHash;
    private int _animSwapToActivated = Animator.StringToHash("swapAnimToActivate"); //use the string to get a hash for the animation
    private int _animSwapToDeactivated = Animator.StringToHash("swapAnimToDeactivate");
    #endregion

    #region Methods
    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<Animator>();

        // if the animations need to be swapped, we do it when computing the hashes
        if (_swapAnimations)
        {
            _deactivateHash = Animator.StringToHash(_activateAnim);
            _activateHash = Animator.StringToHash(_deactivateAnim);
        }
        else
        {
            _deactivateHash = Animator.StringToHash(_deactivateAnim);
            _activateHash = Animator.StringToHash(_activateAnim);
        }
    }

    // These are use to avoid animaton clipping if activating/deactivating and object before the anim is over
    public void ActivationAnimEndEvent()
    {
        if (_activated == true)
        {
            _animated = false;
            Debug.Log("ANIM END");
        }
    }

    public void DeactivationAnimEndEvent()
    {
        if (_activated == false)
        {
            _animated = false;
            Debug.Log("ANIM END");
        }
    }

    /// <summary>
    /// Does the activation logic for this item
    /// </summary>
    public override void Activate()
    {
        base.Activate(); // Call the function from the parent class

        // Do not interrupt the "door close" anim
        if (_animated)
        {
            _anim.SetTrigger(_animSwapToActivated);
        }
        else
        {
            _animated = true;
            _anim.Play(_activateHash);
        }

        if(_disableCollOnActivation) // if needed, disable the colliders
        {
            if(_swapAnimations)
            {
                _coll.enabled = true;
            }
            else
            {
                _coll.enabled = false;
            }
        }
            

        if (_deactivateObstacle)
        {
            // if the animations are swapped, we also need to swap the enabled state for the obstacle
            _navObstacle.enabled = _swapAnimations ? true : false;
        }
    }

    /// <summary>
    /// Does the deactivation logic for this object
    /// </summary>
    public override void Deactivate()
    {
        base.Deactivate();

        // Do not interrupt the "door open" animation
        if (_animated && !_disableParentsOnActivation)
        {
            _anim.SetTrigger(_animSwapToDeactivated);
        }
        else
        {
            _animated = true;
            _anim.Play(_deactivateHash);
        }

        if (_disableCollOnActivation) // if needed, disable the colliders
        {
            if (_swapAnimations)
            {
                _coll.enabled = false;
            }
            else
            {
                _coll.enabled = true;
            }
        }

        if (_deactivateObstacle)
        {
            // if the animations are swapped, we also need to swap the enabled state for the obstacle
            _navObstacle.enabled = _swapAnimations ? false: true;
        }
            
    }
    #endregion
}


// mettre des transitions entre close / open
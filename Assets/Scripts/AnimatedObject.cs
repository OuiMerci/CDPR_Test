using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatedObject : ActivableObject {

    #region Fields
    [SerializeField] private Collider _coll;
    [SerializeField] private NavMeshObstacle _navObstacle;
    [SerializeField] private bool _disableCollOnActivation;
    [SerializeField] private string _activateAnim;
    [SerializeField] private string _deactivateAnim;
    [SerializeField] private bool _deactivateObstacle; // When deactivated, this object cuts out the navmesh, path become available when activated
    [SerializeField] private bool _swapAnimations; // Simply swap the activate/deactivate animations. I should also add an easy way to set the initial state, but won't have time for that prototype. :(

    private Animator _anim;
    private bool _animated;
    private int _deactivateHash; // Using hash is faster than using a string
    private int _activateHash;
    private int _animSwapToActivated = Animator.StringToHash("swapAnimToActivate");
    private int _animSwapToDeactivated = Animator.StringToHash("swapAnimToDeactivate");
    #endregion

    #region Methods
    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<Animator>();

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

    public override void Activate()
    {
        base.Activate();

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

        if(_disableCollOnActivation)
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

    public override void Deactivate()
    {
        Debug.Log("Door deactivated");
        base.Deactivate();

        // Do not interrupt the "door open" animation
        if (_animated && !_disableParentsOnActivation)
        {
            Debug.Log("Swap anims !!!");
            _anim.SetTrigger(_animSwapToDeactivated);
        }
        else
        {
            _animated = true;
            _anim.Play(_deactivateHash);
        }

        if (_disableCollOnActivation)
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
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
    [SerializeField] private bool _carvOnDeactivate; // When deactivated, this object cuts out the navmesh, path become available when activated

    private Animator _anim;
    private bool _animated;
    private int _deactivateHash; // Use hash to avoid using a string, for optimisation
    private int _activateHash;
    private int _animSwapToActivated = Animator.StringToHash("swapAnimToActivate");
    private int _animSwapToDeactivated = Animator.StringToHash("swapAnimToDeactivate");
    #endregion

    #region Methods
    // Use this for initialization
    void Start ()
    {
        _anim = GetComponent<Animator>();
        _deactivateHash = Animator.StringToHash(_deactivateAnim);
        _activateHash = Animator.StringToHash(_activateAnim);
        _navObstacle = GetComponent<NavMeshObstacle>();
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
            _coll.enabled = false;

        if (_carvOnDeactivate)
            _navObstacle.carving = false;
    }

    public override void Deactivate()
    {
        Debug.Log("Door deactivated");
        base.Deactivate();

        // Do not interrupt the "door open" animation
        if (_animated)
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
            _coll.enabled = true;

        if (_carvOnDeactivate)
            _navObstacle.carving = true;
    }
    #endregion
}


// mettre des transitions entre close / open
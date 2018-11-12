using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ActivableObject {

    #region Fields
    [SerializeField] private Collider _coll;

    private Animator _anim;
    private bool _animated;
    private int _closeHash = Animator.StringToHash("Door_close");
    private int _openHash = Animator.StringToHash("Door_open");
    private int _animSwapToOpen = Animator.StringToHash("swapAnimToOpen");
    private int _animSwapToClose = Animator.StringToHash("swapAnimToClose");
    #endregion

    #region Methods
    // Use this for initialization
    void Start ()
    {
        _anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
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
            _anim.SetTrigger(_animSwapToOpen);
        }
        else
        {
            _animated = true;
            _anim.Play(_openHash);
        }

        _coll.enabled = false;
    }

    public override void Deactivate()
    {
        Debug.Log("Door deactivated");
        base.Deactivate();

        // Do not interrupt the "door open" animation
        if (_animated)
        {
            Debug.Log("Swap anims !!!");
            _anim.SetTrigger(_animSwapToClose);
        }
        else
        {
            _animated = true;
            _anim.Play(_closeHash);
        }

        _coll.enabled = true;
    }
    #endregion
}


// mettre des transitions entre close / open
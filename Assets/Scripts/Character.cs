using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    protected bool _isOnMovingBlock; // may be used for specific "isOnMovingBlock" check

    //Called when a sliding bloc starts moving
    public virtual void StartFollowBlockMovement()
    {
        _isOnMovingBlock = true;
    }

    // Called when the block reaches ground level
    public virtual void EndFollowBlockMovement()
    {
        _isOnMovingBlock = false;
    }
}

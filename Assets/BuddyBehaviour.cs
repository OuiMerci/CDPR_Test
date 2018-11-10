using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuddyBehaviour : MonoBehaviour {

    // il faut faire un raycast pour vérifier que buddy voit bien le laser
    private NavMeshAgent _navAgent;

    //private int _dashHash = Animator.StringToHash("isDashing");
    //private int _aimingHash = Animator.StringToHash("isAiming");

    // Use this for initialization
    void Start () {
        _navAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
	}
    
    public void SetDest(Vector3 dest)
    {
        _navAgent.SetDestination(dest);
    }
}

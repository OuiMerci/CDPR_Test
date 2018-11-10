using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuddyBehaviour : MonoBehaviour {

    // il faut faire un raycast pour vérifier que buddy voit bien le laser
    private NavMeshAgent _navAgent;
    private PlayerBehaviour _player;
    private int _laserVisionMask;

    //private int _dashHash = Animator.StringToHash("isDashing");
    //private int _aimingHash = Animator.StringToHash("isAiming");

    // Use this for initialization
    void Start () {
        _navAgent = GetComponent<NavMeshAgent>();
        _player = PlayerBehaviour.Instance;
        _laserVisionMask = ~(1 << LayerMask.NameToLayer("Buddy"));
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
	}

    private void OnLaserStateToggle(bool isOn)
    {
        Debug.Log("Buddy : Laser toggle : " + isOn);
    }

    private void TryFollowLaserPointer()
    {
        Vector3 destination;
        bool isHittingBuddy;
        bool validHit = _player.RequestLaserRaycast(out destination, out isHittingBuddy);

        if(validHit)
        {

        }
    }

    private void IsLaserPointerVisible()
    {
        //if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, LASER_MAX_DISTANCE, _laserPointerMask))
        //{
        //}
    }
}

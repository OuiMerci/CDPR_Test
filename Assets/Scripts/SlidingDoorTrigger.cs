using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorTrigger : MonoBehaviour {

    [SerializeField] private SlidingBlock _slidingDoor;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Buddy")
        {
            _slidingDoor.UpdateActivationPoints(+1);
        }
    }
}
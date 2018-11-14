using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextRoomTrigger : MonoBehaviour {

    [SerializeField] private int _roomId;

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.SetActiveRoom(_roomId);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextRoomTrigger : MonoBehaviour {

    [SerializeField] private int _roomId; //The new room ID

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.SetActiveRoom(_roomId);
    }
}

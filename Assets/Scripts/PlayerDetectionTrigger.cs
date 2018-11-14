using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionTrigger : MonoBehaviour {

    [SerializeField] private BuddyBehaviour _buddy; // The buddy linked to this zone

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour.Instance.AddNewBuddy(_buddy);
        _buddy.TryFollowPlayer();
        gameObject.SetActive(false);
    }
}

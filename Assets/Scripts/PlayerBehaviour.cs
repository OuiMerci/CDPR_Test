using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    const float RAYCAST_MAX_DISTANCE = 100; // Define a const for raycast maxDistance

    [SerializeField] private Transform _laserPointer; //Used to update, the laser that controls the pet's position
    [SerializeField] private int _laserPointerMask; // We use this to avoid collision with the player's capsule
    public BuddyBehaviour buddy;

    // Use this for initialization
    void Start () {
        _laserPointerMask = ~(1 << 9);
	}
	
	// Update is called once per frame
	void Update () {
        UpdateLaserPosition();
	}

    private void UpdateLaserPosition()
    {
        Camera cam = Camera.main;
        RaycastHit hit;

        //// si hit collide avec la mummy, on joue l'anim de idle

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, RAYCAST_MAX_DISTANCE, _laserPointerMask))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.green);
            _laserPointer.position = cam.transform.position;
            _laserPointer.eulerAngles = cam.transform.eulerAngles;
        }

        buddy.SetDest(hit.point);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerZone : MonoBehaviour {

    #region Fields
    [SerializeField] private HarmlessNPC _npc; //The NPC related to this zone
    [SerializeField] private bool _checkZoneEnter; // The verification uses one enter trigger and one exit trigger, this boolean define which role this one has
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        // If a character already trigger this and didn't leave since, do nothing
        if(_checkZoneEnter && _npc.CharactersToCheck.Contains(other.gameObject) == false)
        {
            Debug.Log("enter zone : " + other.name);
            _npc.UpdateActivationPoints(+1);
            _npc.CharactersToCheck.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit zone : " + other.name);

        // Only update points if this character has triggered the NPC before exiting
        if (_checkZoneEnter == false && _npc.CharactersToCheck.Contains(other.gameObject))
        {
            Debug.Log("exit zone +++ : " + other.name);
            _npc.UpdateActivationPoints(-1);
            _npc.CharactersToCheck.Remove(other.gameObject);
        }
    }
    #endregion
}
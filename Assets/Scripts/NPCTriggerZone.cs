using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerZone : MonoBehaviour {

    #region Fields
    [SerializeField] private HarmlessNPC _npc;
    [SerializeField] private bool _checkZoneEnter;
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
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

        // Only update the activation points if the character activated the other trigger before
        if (_checkZoneEnter == false && _npc.CharactersToCheck.Contains(other.gameObject))
        {
            Debug.Log("exit zone +++ : " + other.name);
            _npc.UpdateActivationPoints(-1);
            _npc.CharactersToCheck.Remove(other.gameObject);
        }
    }
    #endregion
}
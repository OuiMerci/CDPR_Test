using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingBlockCharacterChecker : MonoBehaviour {

    [SerializeField] private SlidingBlock _slidingBlock;

    private void OnTriggerEnter(Collider other)
    {
        Character script = other.gameObject.GetComponent<Character>();

        if (script != null)
        {
            Debug.Log("Adding " + other.name);
            _slidingBlock.CharactersOnTop.Add(script);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character script = other.gameObject.GetComponent<Character>();

        if (script != null)
        {
            Debug.Log("Removing " + other.name);
            _slidingBlock.CharactersOnTop.Remove(script);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : ActivableObject {
    [SerializeField] private GameObject _endMessage;

    public override void Activate()
    {
        base.Activate();
        _endMessage.SetActive(true);
    }
}

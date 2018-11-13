using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMessageTrigger : MonoBehaviour {

    #region Fields
    [SerializeField] private bool _activateAfterDelay;
    [SerializeField] private float _activationDelay;
    [SerializeField] private bool _deactivateAfterDelay;
    [SerializeField] private float _deactivationDelay;
    [SerializeField] private List<GameObject> _messageList;
    private bool _activationCanceled; // Sometimes, we only want to display a tip if the player doesn't find the solution
    private int _currentMessageIndex = 0;
    private GameObject _currentMessage;
    private Collider _coll;
    #endregion

    #region Methods
    private void Start()
    {
        _coll = GetComponent<Collider>();
    }

    private void ActivateMessage()
    {
        _currentMessage = _messageList[_currentMessageIndex];

        if (_activationCanceled == false)
        {
            _currentMessage.SetActive(true);

            if (_deactivateAfterDelay)
            {
                Invoke("DeactivateMessage", _deactivationDelay);
            }
        }
    }

    private void DeactivateMessage()
    {
        _currentMessage.SetActive(false);

        _currentMessageIndex++;
        if(_currentMessageIndex < _messageList.Count)
        {
            ActivateMessage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_activateAfterDelay)
        {
            Invoke("ActivateMessage", _activationDelay);
        }
        else
        {
            ActivateMessage();
        }

        _coll.enabled = false;
    }
    #endregion
}

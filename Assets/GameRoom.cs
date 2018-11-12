using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom : MonoBehaviour {

    #region Fields
    [SerializeField] private List<HarmlessNPC> _rabbitList;
    [SerializeField] private Transform _playerSpawn;
    [SerializeField] private Transform _buddySpawn;
    #endregion

    #region Properties
    public List<HarmlessNPC> RabbitList
    {
        get { return _rabbitList; }
    }
    #endregion

    #region methods
    public void TeleportCharacters()
    {
        PlayerBehaviour.Instance.transform.position = _playerSpawn.position;
        PlayerBehaviour.Instance.transform.eulerAngles = _playerSpawn.eulerAngles;

        BuddyBehaviour.Instance.transform.position = _buddySpawn.position;
        BuddyBehaviour.Instance.transform.eulerAngles = _buddySpawn.eulerAngles;
    }
    #endregion
}

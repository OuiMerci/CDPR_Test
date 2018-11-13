using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom : MonoBehaviour {

    #region Fields
    [SerializeField] private List<HarmlessNPC> _rabbitList;
    [SerializeField] private Transform _playerSpawn;
    [SerializeField] private List<Transform> _buddySpawns;
    private GameObject _buddyPrefab;
    #endregion

    #region Properties
    public List<HarmlessNPC> RabbitList
    {
        get { return _rabbitList; }
    }
    #endregion

    #region methods
    private void Start()
    {
        _buddyPrefab = GameManager.Instance.BuddyPrefab;
    }

    public void TeleportCharacters()
    {
        PlayerBehaviour.Instance.transform.position = _playerSpawn.position;
        PlayerBehaviour.Instance.transform.eulerAngles = _playerSpawn.eulerAngles;

        foreach (Transform buddySpawn in _buddySpawns)
        {
            Debug.Log("Spawning buddy");
            GameObject.Instantiate(_buddyPrefab, buddySpawn.position, buddySpawn.rotation);
        }
    }
    #endregion
}

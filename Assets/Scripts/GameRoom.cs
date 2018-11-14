using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom : MonoBehaviour {

    #region Fields
    [SerializeField] private List<HarmlessNPC> _rabbitList; // Stores the list of rabbits in this room, used by buddies to know what buddies to raycast to
    [SerializeField] private Transform _playerSpawn; // Where the player is spawned when using a cheat code
    [SerializeField] private List<Transform> _buddySpawns; // Where buddies are spawned when using a cheat code
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

    /// <summary>
    /// Cheat code for faster testing
    /// </summary>
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

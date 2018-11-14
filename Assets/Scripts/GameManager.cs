using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Fields
    public enum GameStates
    {
        Playing,
        Pause,
        Complete
    }
    private GameStates _gameState;

    [SerializeField] private List<GameRoom> _roomList; // Contains the list of the rooms for this level
    [SerializeField] private GameObject _buddyPrefab; // Used to instantiate buddies when using a cheat code
    const float MAX_RAYCAST_DISTANCE_VALUE = 100;   // For this small prototype, all raycast using the same distance is ok
    static private GameManager _instance;
    [SerializeField]  private int _activeRoomIndex; // current room the player is in
    #endregion

    #region Property
    static public GameManager Instance
    {
        get { return _instance; }
    }

    public float MAX_RAYCAST_DISTANCE
    {
        get { return MAX_RAYCAST_DISTANCE_VALUE; }
    }

    public GameStates GameState
    {
        get { return _gameState; }
    }

    public GameRoom ActiveRoom
    {
        get {
            if (_activeRoomIndex < _roomList.Count)
                return _roomList[_activeRoomIndex];
            else
            {
                Debug.LogWarning("Trying to access ActiveRoom, but index is out of range.");
                return null;
            }
        }
    }

    public GameObject BuddyPrefab
    {
        get { return _buddyPrefab; }
    }
    #endregion

    #region
    private void Awake()
    {
        _instance = this;
        SetGameState(GameStates.Playing);
        _activeRoomIndex = 0;
    }

    /// <summary>
    /// Change the state of the game
    /// </summary>
    /// <param name="state"></param>
    public void SetGameState(GameStates state)
    {
        _gameState = state;
    }

    /// <summary>
    /// Change the current room and possibly deactive the previous one (not used)
    /// </summary>
    /// <param name="roomIndex"></param>
    public void SetActiveRoom(int roomIndex)
    {
        _activeRoomIndex = roomIndex;

        //if(roomIndex - 1 >= 0)
        //{
        //    _roomList[roomIndex - 1].gameObject.SetActive(false);
        //}
    }

    /// <summary>
    /// Cheat code, to teleport to a specific room
    /// </summary>
    /// <param name="roomIndex"></param>
    public void TeleportToRoom(int roomIndex)
    {
        Debug.Log("Teleporting to room : " + roomIndex);
        _activeRoomIndex = roomIndex;
        _roomList[roomIndex].TeleportCharacters();
    }

    /// <summary>
    /// Cheat code, to teleport to the next room
    /// </summary>
    public void TeleportToNextRoom()
    {
        Debug.Log("Teleporting to next room : ");
        _activeRoomIndex++;
        _roomList[_activeRoomIndex].TeleportCharacters();
    }
    #endregion
}

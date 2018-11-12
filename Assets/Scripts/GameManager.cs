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

    [SerializeField] private List<GameRoom> _roomList;
    const float MAX_RAYCAST_DISTANCE_VALUE = 100;
    static private GameManager _instance;
    private int _activeRoomIndex;
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
    #endregion

    #region
    private void Awake()
    {
        _instance = this;
        SetGameState(GameStates.Playing);
        _activeRoomIndex = 0;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetGameState(GameStates state)
    {
        _gameState = state;
    }

    public void SetActiveRoom(int roomIndex)
    {
        _activeRoomIndex = roomIndex;
    }
    #endregion
}

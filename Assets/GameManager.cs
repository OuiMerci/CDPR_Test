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
    const float MAX_RAYCAST_DISTANCE_VALUE = 100;
    static private GameManager _instance;
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
    #endregion

    #region
    private void Awake()
    {
        _instance = this;
        SetGameState(GameStates.Playing);
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
    #endregion
}

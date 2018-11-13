using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingBlock : ActivableObject {

    #region Fields
    [SerializeField] private float _upLimit;
    [SerializeField] private float _downLimit;
    [SerializeField] private float _speed;

    private Rigidbody _rigidbody;
    public List<Character> _charactersOnTop;
    public bool _playerOnTop;
    public bool _movingUp;
    public bool _moving;
    #endregion

    #region Fields
    // Use this for initialization
    void Start () {
        _charactersOnTop = new List<Character>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void FixedUpdate()
    {
        if (_moving)
            Move();
    }

    private void OnTriggerEnter(Collider other)
    {
        Character script = other.gameObject.GetComponent<Character>();

        if(script != null)
        {
            Debug.Log("Adding " + other.name);
            _charactersOnTop.Add(script);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character script = other.gameObject.GetComponent<Character>();

        if (script != null)
        {
            Debug.Log("Removing " + other.name);
            _charactersOnTop.Remove(script);
        }
    }

    private void StartMoving(bool up)
    {
        Debug.Log("Start moving : " + up);
        _moving = true;
        _movingUp = up ? true: false;

        foreach(Character character in _charactersOnTop)
        {
            character.StartFollowBlockMovement();
        }
    }

    private void Move()
    {
        Vector3 posDifference;

        if(_movingUp)
        {
            if(transform.position.y >= _upLimit)
            {
                transform.position = new Vector3(transform.position.x, _upLimit, transform.position.z);
                EndMovement(true);
                return;
            }

            posDifference = new Vector3(0, _speed * Time.deltaTime, 0);
        }
        else
        {
            if (transform.position.y <= _downLimit)
            {
                transform.position = new Vector3(transform.position.x, _downLimit, transform.position.z);
                EndMovement(false);
                return;
            }

            posDifference = new Vector3(0, - _speed * Time.deltaTime, 0);
        }

        foreach(Character character in _charactersOnTop)
        {
            character.transform.position += posDifference;
        }
        transform.position += posDifference;
    }

    private void EndMovement(bool up)
    {
        _moving = false;

        foreach (Character character in _charactersOnTop)
        {
            character.EndFollowBlockMovement();
        }
    }

    public override void Activate()
    {
        base.Activate();
        StartMoving(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        StartMoving(false);
    }
    #endregion
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHelicopter : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] private float _responsiveness = 500f;
    [SerializeField] private float _throttleAmount = 25f;
    [SerializeField] private float _rotorSpeedModifier = 10f;
    
    //[SerializeField] private bool startItUp;
    //[SerializeField] private bool IsOnGround = false;

    private float _throttle;
    private float _engineForce;

    private float _roll;
    private float _pitch;
    private float _yaw;

    /*public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            MainRotorController.RotarSpeed = value * 80;
            SubRotorController.RotarSpeed = value * 40;
            HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
            _engineForce = value;
        }
    }*/

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    /*private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }*/

    private void Update()
    {
        HandleInput();

        /*if(playerController.parent != transform && transform.position.y >= 5f && !IsOnGround)
        {
            startItUp = false;
            EngineForce = 0f;
            transform.SetPositionAndRotation(initialPosition, initialRotation);
            HelicopterModel.angularVelocity = Vector3.zero;
        }*/
    }

    private void HandleInput()
    {
        _roll = Input.GetAxis("Roll");
        _pitch = Input.GetAxis("Pitch");
        _yaw = Input.GetAxis("Yaw");

        if (Input.GetKey(KeyCode.Space))
        {
            _throttle += Time.deltaTime * _throttleAmount;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            _throttle -= Time.deltaTime * _throttleAmount;
        }

        _throttle = Mathf.Clamp(_throttle, 0, 100);
    }

    void FixedUpdate()
    {
        _rigidbody.AddForce(transform.up * _throttle, ForceMode.Impulse);
        //_rigidbody.AddRelativeForce(Vector3.up * _throttle);

        _rigidbody.AddTorque(transform.right * _pitch * _responsiveness);
        _rigidbody.AddTorque(-transform.forward * _roll * _responsiveness);
        _rigidbody.AddTorque(transform.up * _yaw * _responsiveness);
    }
}

/*if (startItUp == true)
{
    if (EngineForce <= 50f)
    {
        EngineForce += 0.1f;
    }
}

if (startItUp == false)
{
    if (EngineForce > 0)
    {
        EngineForce -= 0.1f;
    }
}
}*/
    
    /*public void startHelicopter()
    {  
        if(startItUp == true)
        {
            EngineForce = 50f;
        }
        startItUp = !startItUp;
    }
    
    private void OnCollisionEnter()
    {
        IsOnGround = true;
        Debug.Log("on ground");
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
        Debug.Log("not on ground");
    }


    //
    
    public Transform playerController;
    public AudioSource HelicopterSound;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    
    private float leverValue;

    private bool rollLeft = false;
    private bool rollRight = false;
    private bool pitchForward = false;
    private bool pitchback = false;

    public float speed = 0.11f;

    private float force;
   // public float maxHeight = 250;
    
    private float TurnForce = 0f;
    public float TurnForceMultiplier = 5f;

    private float ForwardForce = 0f;
    public float ForwardForceMultiplier = 5f;

    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 15f;
    public float EffectiveHeight = 250f;

    public float turnTiltForcePercent = 1.5f;
    private float turnForcePercent = 0f;
    public float turnForcePercentMultiplier = 0;
    
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public float MaxEngineForce = 350f;
    public bool isAscending = false;
    
    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    
    
}*/
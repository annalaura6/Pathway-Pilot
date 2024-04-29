
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VRHelicopterController : MonoBehaviour
{
    public Transform playerController;

    public AudioSource HelicopterSound;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public bool startItUp;


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
    private float _engineForce;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    public float MaxEngineForce = 300f;

    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            MainRotorController.RotarSpeed = value * 80;
            SubRotorController.RotarSpeed = value * 40;
            HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    public bool IsOnGround = false;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        
        //Reset helicoptor to start position if you bail out and you aren't landed.
        if(playerController.parent != transform && transform.position.y >= 5f && !IsOnGround)
        {
            startItUp = false;
            EngineForce = 0f;
            transform.SetPositionAndRotation(initialPosition, initialRotation);
            HelicopterModel.angularVelocity = Vector3.zero;
        }
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
    }

    private void LiftProcess()
    {

         float lerpUpForce;

         var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
         upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
         lerpUpForce = Mathf.Lerp(0, upForce,speed);
         HelicopterModel.AddRelativeForce(Vector3.up *lerpUpForce);

    }


    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    void FixedUpdate()
    {
        LiftProcess();  
        MoveProcess();
        TiltProcess();

        

        float tempY = 0;
        float tempX = 0;


        // stable forward
        if (hMove.y > 0)
        {
            tempY = -Time.fixedDeltaTime;
        }
        else
            if (hMove.y < 0)
        {
            tempY = Time.fixedDeltaTime;
        }

        // stable lurn
        if (hMove.x > 0)
        {
            tempX = -Time.fixedDeltaTime;
        }
        
        else if (hMove.x < 0)
        {
            tempX = Time.fixedDeltaTime;
        }


        if (startItUp == true) 
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


        if (pitchForward == true) // move forward center stick forward
        {
            if (!IsOnGround)
                tempY = Time.fixedDeltaTime;
        }


        if (pitchback == true)// move back center stick back
        {
            if (!IsOnGround)
                tempY = -Time.fixedDeltaTime;
        }          
                    
        if (rollLeft == true)// move left center stick left
                {
                    if (!IsOnGround) 
                    tempX = -Time.fixedDeltaTime;
                        
                }

        if (rollRight == true)// move right center stick right
        {
            if (!IsOnGround)
                tempX = Time.fixedDeltaTime;
        }

        // I want to change it to normal function without input so i can assign in to a button in the scene
        if (playerController.parent == transform)
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0)    // turn right right trigger
            {
                if (!IsOnGround)
                {
                    turnForcePercent = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) * turnForcePercentMultiplier;
                    force = (turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass;
                    HelicopterModel.AddRelativeTorque(0f, force, 0);
                }
            }

        // I want to change it to normal function without input so i can assign in to a button in the scene
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0) // turn left left trigger
            {
                if (!IsOnGround)
                {
                    turnForcePercent = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) * turnForcePercentMultiplier;
                    force = -(turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass;
                    HelicopterModel.AddRelativeTorque(0f, force, 0);
                }
            }
        }

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);

        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);

    }


    private void OnCollisionEnter()
    {
        IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
    }

    public void PitchRoll(Vector2 LeverVector)
    {

        
        //roll left and right
        if (LeverVector.x < -0.1f)
        {
            TurnForce = LeverVector.x * -TurnForceMultiplier;
            rollRight = false;
            rollLeft = true;
        }
        if (LeverVector.x > 0.1f)
        {
            TurnForce = LeverVector.x * TurnForceMultiplier;
            rollRight = true;
            rollLeft = false;
        }

        if (LeverVector.x >= -0.1f && LeverVector.x < 0.1f)
        {
            rollRight = false;
            rollLeft = false;
        }

        //pitch forward and backward
        if (LeverVector.y < -0.1f)
        {
            ForwardForce = LeverVector.y * -ForwardForceMultiplier;
            pitchback = true;
            pitchForward = false;
        }

        if( LeverVector.y > 0.1f)
        {
            ForwardForce = LeverVector.y * ForwardForceMultiplier;
            pitchback = false;
            pitchForward = true;
        }

        if (LeverVector.y >= -0.1f && LeverVector.y < 0.1f)
        {
            ForwardForce = 0;
            pitchback = false;
            pitchForward = false;
        }

    }

    /*public void MoveUpDown(Vector2 LeverVector)
    {
        if(LeverVector.y < 0)
        {
            leverValue = (LeverVector.y + 1)/2 ;
        }
        else if(LeverVector.y >= 0)
        {
            leverValue = (LeverVector.y / 2) + 0.5f;
        }
        
       
       // EffectiveHeight = leverValue * maxHeight;

        if(startItUp == true )
        {

            if (EngineForce >= 50)
            {
                EngineForce = (leverValue * 300) + 50;
            }
        }
    }*/
    
    public void MoveUpDown(Vector2 LeverVector)
    {
        // Assuming LeverVector.y is normalized between 0 and 1
        if (LeverVector.y >= 0 && startItUp)
        {
            // This maps the lever value (0 to 1) to the engine force range (50 to MaxEngineForce)
            EngineForce = Mathf.Lerp(50, MaxEngineForce, LeverVector.y);
        }
    }

    public void startHelicopter()
    {  
        if(startItUp == true)
        {
            EngineForce = 50f;
        }
        startItUp = !startItUp;
    }
}
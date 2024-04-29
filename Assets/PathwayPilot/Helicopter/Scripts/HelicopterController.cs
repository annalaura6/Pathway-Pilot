using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    public AudioSource HelicopterSound;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 1f;
    public float ForwardForce = 5f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 10f;
    public float EffectiveHeight = 100f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 1.3f;

    private float _engineForce;
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
    public bool IsOnGround = true;

    // Use this for initialization
    void Start()
    {
        //ControlPanel.KeyPressed += OnKeyPressed;
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        LiftProcess();
        MoveProcess();
        TiltProcess();
    }
    
    //Stop action
    public void StopHelicopterAction()
    {
        StopAllCoroutines();
    }

    //Move forward
    public void MoveForward()
    {
        StartCoroutine(MoveForwardCoroutine());
        //hMove.y = 1f;
    }
    
    private IEnumerator MoveForwardCoroutine()
    {
        while (true)
        {
            hMove.y = 0.2f;
            yield return null;
        }
    }
    
    //Move backwards
    public void MoveBackwards()
    {
        hMove.y = -1f;
        StartCoroutine(MoveBackwardsCoroutine());
    }
    
    private IEnumerator MoveBackwardsCoroutine()
    {
        while (true)
        {
            hMove.y = -0.2f;
            yield return null;
        }
    }
    
    public void StopVerticalMovement()
    {
        hMove.y = 0f;
    }
    
    //Turn Left
    public void TurnLeft()
    {
        hMove.x = -0.2f;
    }
    
    
    //Turn Right
    public void TurnRight()
    {
        hMove.x = 0.2f;
    }
    
    public void StopHorizontalMovement()
    {
        hMove.x = 0f;
    }
    
    //Lift up
    public void LiftUp()
    {
        StartCoroutine(IncreaseEngineForce());
    }
    
    private IEnumerator IncreaseEngineForce()
    {
        while (true)
        {
            EngineForce += 0.1f;
            yield return null;
        }
    }
    
    //Lift down
    public void LiftDown()
    {
        StartCoroutine(DecreaseEngineForce());
    }
    
    private IEnumerator DecreaseEngineForce()
    {
        while (EngineForce > 0)
        {
            EngineForce -= 0.12f;
            if (EngineForce < 0) EngineForce = 0;
            yield return null;
        }
    }
    
    //Tilt left
    public void TiltLeft()
    {
        StartCoroutine(TiltLeftCoroutine());
    }
    
    private IEnumerator TiltLeftCoroutine()
    {
        while (true)
        {
            hTilt.x = -TurnTiltForce;
            yield return null;
        }
    }
    
    //Tilt right
    public void TiltRight()
    {
        StartCoroutine(TiltRightCoroutine());
    }
    
    private IEnumerator TiltRightCoroutine()
    {
        while (true)
        {
            hTilt.x = TurnTiltForce;
            yield return null;
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
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void OnCollisionEnter()
    {
        IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
    }
}
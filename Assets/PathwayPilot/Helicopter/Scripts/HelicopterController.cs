using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    public AudioSource HelicopterSound;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 3f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
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

    public void MoveForward()
    {
        hMove.y = 0.2f;
    }

    public void MoveBackwards()
    {
        hMove.y = -0.2f;
    }

    public void StopVerticalMovement()
    {
        hMove.y = 0f;
    }

    public void TurnLeft()
    {
        hMove.x = -0.1f;
    }

    public void TurnRight()
    {
        hMove.x = 0.1f;
    }

    public void StopHorizontalMovement()
    {
        hMove.x = 0f;
    }

    public void LiftUp()
    {
        StartCoroutine(IncreaseEngineForce());
    }

    public void StopLift()
    {
        StopAllCoroutines();
    }

    private IEnumerator IncreaseEngineForce()
    {
        while (true)
        {
            EngineForce += 0.1f;
            yield return null;
        }
    }

    public void LiftDown()
    {
        StartCoroutine(DecreaseEngineForce());
    }

    public void StopLiftDown()
    {
        StopAllCoroutines();
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarMovement : MonoBehaviour
{
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider _FLWheel;
    [SerializeField] private WheelCollider _FRWheel;
    [SerializeField] private WheelCollider _RLWheel;
    [SerializeField] private WheelCollider _RRWheel;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform _FLWheelTransform;
    [SerializeField] private Transform _FRWheelTransform;
    [SerializeField] private Transform _RLWheelTransform;
    [SerializeField] private Transform _RRWheelTransform;

    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _motorForce = 50f;
    [SerializeField] private float _maxSteerAngle = 30f;
    [SerializeField] private float _maxBrakeForce = 300f;
    [SerializeField] private float raycastHeightOffset = 1.0f;
    
    [SerializeField] private float _detectionRange = 50f;

    private int _waypointIndex = 0;
    private bool _isObstacleDetected = false;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        transform.position = _waypoints[0].position;
    }

    void FixedUpdate()
    {
        HandleMotor(_detectionRange);
        HandleSteering();
        UpdateWheels();
    }

    private void HandleMotor(float detectionRange)
    {
        float currentSpeed = _rb.velocity.magnitude;
        float distanceToObstacle = float.MaxValue;

        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * raycastHeightOffset;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * detectionRange;
        Debug.DrawRay(rayStart, forward, Color.green);

        if (Physics.Raycast(rayStart, forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                _isObstacleDetected = true;
                distanceToObstacle = hit.distance;
                Debug.Log($"Obstacle detected at distance: {distanceToObstacle}");
            }
        }
        else
        {
            _isObstacleDetected = false;
        }

        if (_isObstacleDetected)
        {
            float brakeForce = Mathf.Lerp(0, _maxBrakeForce, Mathf.Clamp01((detectionRange - distanceToObstacle) / detectionRange));
            _RLWheel.brakeTorque = brakeForce;
            _RRWheel.brakeTorque = brakeForce;
            
            _RLWheel.motorTorque = 0;
            _RRWheel.motorTorque = 0;
            Debug.Log($"Applying brake force: {brakeForce}");
        }
        else
        {
            _RLWheel.brakeTorque = 0;
            _RRWheel.brakeTorque = 0;
            _RLWheel.motorTorque = _motorForce;
            _RRWheel.motorTorque = _motorForce;
        }
    }
    
    private void HandleSteering()
    {
        if (_waypointIndex < _waypoints.Length)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(_waypoints[_waypointIndex].position);
            float newSteer = (_maxSteerAngle * (relativeVector.x / relativeVector.magnitude));
            _FLWheel.steerAngle = newSteer;
            _FRWheel.steerAngle = newSteer;
        }
    }

    private void UpdateWheels()
    {
        UpdateWheelPose(_FLWheel, _FLWheelTransform);
        UpdateWheelPose(_FRWheel, _FRWheelTransform);
        UpdateWheelPose(_RLWheel, _RLWheelTransform, true);
        UpdateWheelPose(_RRWheel, _RRWheelTransform, true);
    }

    private void UpdateWheelPose(WheelCollider collider, Transform transform, bool rotate = false)
    {
        Vector3 pos;
        Quaternion quat;
        collider.GetWorldPose(out pos, out quat);
        transform.position = pos;
        transform.rotation = quat;

        if (rotate)
        {
            transform.Rotate(Vector3.right, collider.rpm / 60 * 360 * Time.deltaTime);
        }
    }

    void Update()
    {
        if (_waypointIndex < _waypoints.Length && !_isObstacleDetected)
        {
            if (Vector3.Distance(transform.position, _waypoints[_waypointIndex].position) < 1f)
            {
                _waypointIndex++;
            }
        }
    }
}

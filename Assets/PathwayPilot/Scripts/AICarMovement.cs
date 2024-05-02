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

    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _motorForce = 50f;
    [SerializeField] private float _maxSteerAngle = 30f;
    [SerializeField] private float _breakForce = 100f;
    [SerializeField] private float raycastHeightOffset = 1.0f;

    private int _waypointIndex = 0;
    private bool _isObstacleDetected = false;

    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * raycastHeightOffset;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(rayStart, forward, Color.green);

        _isObstacleDetected = Physics.Raycast(rayStart, forward, out hit, 10) && hit.collider.CompareTag("Obstacle");

        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void HandleMotor()
    {
        float motorTorque = _isObstacleDetected ? 0 : _motorForce;
        _RLWheel.motorTorque = motorTorque;
        _RRWheel.motorTorque = motorTorque;
        
        _RLWheel.brakeTorque = _isObstacleDetected ? _breakForce : 0;
        _RRWheel.brakeTorque = _isObstacleDetected ? _breakForce : 0;
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
    }

    private void UpdateWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 pos;
        Quaternion quat;
        collider.GetWorldPose(out pos, out quat);
        transform.position = pos;
        transform.rotation = quat;
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

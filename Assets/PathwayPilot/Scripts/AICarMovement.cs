using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarMovement : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _speed = 5f;
    private int _waypointIndex = 0;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        transform.position = _waypoints[0].position;
    }

    void Update()
    {
        MoveToNextWaypoint();
    }
    
    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);

        if (Physics.Raycast(transform.position, forward, out hit, 10))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                _speed = 0; // Stop the car
            }
        }
        else
        {
            _speed = 5f; // Resume speed
        }

        MoveToNextWaypoint();
    }

    void MoveToNextWaypoint()
    {
        if (_waypointIndex < _waypoints.Length)
        {
            Vector3 dir = _waypoints[_waypointIndex].position - transform.position;
            _rb.MovePosition(transform.position + dir.normalized * (_speed * Time.deltaTime));

            if (Vector3.Distance(transform.position, _waypoints[_waypointIndex].position) < 0.5f)
            {
                _waypointIndex++;
            }
        }
    }
}

using UnityEngine;


//[CreateAssetMenu(fileName = "Steering Settings", menuName = "Steering/SteeringSettings", order = 1)]
public class SteeringSettings : ScriptableObject
{
    [Header("Steering Settings")]
    public float mass = 70.0f;                  // mass in kg
    public float maxDesiredVelocity = 3.0f;     // max desired velocity in m/s
    public float maxSteeringForce = 3.0f;       // max steering force in m/s
    public float maxSpeed = 3.0f;               // max speed in m/s

    [Header("Follow Path")]
    public float m_followPathRadius = 0.2f;     // radius to change waypoint

    [Header("Arrive")]
    public float arriveDistance = 1.0f;         // distance to object when we reach zero velocity in m
    public float slowingDistance = 2.0f;

    [Header("Pursue and evade")]
    public float lookAheadTime = 1.0f;

    [Header("Wander")]
    public float wanderCircleDistance = 5.0f;
    public float wanderCircleRadius = 5.0f;
    public float wanderNoiseAngle = 10.0f;
}

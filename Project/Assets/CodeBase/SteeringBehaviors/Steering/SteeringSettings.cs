using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    [CreateAssetMenu(fileName = "Steering Settings", menuName = "Steering/Steering Settings", order = 1)]
    public class SteeringSettings : ScriptableObject
    {
        public enum FPM { Forwards, Backwards, PingPong, Random }

        [Header("Steering Settings")]
        public float  m_mass                  =  20.0f;        // mass in kg
        public float  m_maxDesiredVelocity    =   3.0f;        // max desired velocity in m/s  
        public float  m_maxSteeringForce      =   3.0f;        // max steering 'force' in m/s  
        public float  m_maxSpeed              =   3.0f;        // max vehicle speed in m/s 
        public float  m_minSpeed              =   0.05f;       // max vehicle speed in m/s 

        [Header("Follow Path")]                 
        public FPM    m_followPathMode        =  FPM.Forwards; // options 
        public bool   m_followPathLooping     =  false;        // looping on or off
        public float  m_followPathRadius      =   2.5f;        // circle radius in m
                                                
        [Header("Flee and Pursue")]             
        public float  m_stopDistance          =  10.0f;        // stop moving when we reach this distance away from the target (zero to disable stopping)
                                                
        [Header("Pursue and Evade")]            
        public float  m_lookAheadTime         =   1.0f;        // look ahead time for pursuit and evade in s
                                                
        [Header("Wander")]                      
        public float  m_wanderCircleDistance  =   5.0f;        // circle distance in m
        public float  m_wanderCircleRadius    =   5.0f;        // circle radius in m
        public float  m_wanderNoiseAngle      =  10.0f;        // noise angle in degrees
                                                
        [Header("Hide")]                        
        public string m_hideLayer             =  "Obstacles";  // the layer name containing all colliders we can hide behind
        public float  m_hideOffset            =   1.0f;        // the distance from surface on the other side of the hide collider
                                                
        [Header("Flock Overlap Sphere")]             
        public string m_flockLayer            =  "Boids";               // the layer name containing all the agents in this flock
        
        [Header("Flock Quad Tree")] 
        public Rect   m_flockBounds           =  new Rect(-50,-50,100,100); // the boundary in which we will look for flocking objects
        public int    m_flockQTMaxSize        =  10;           // the maximum number of nodes per QuatTree quadrant

        [Header("Flock")]
        public float  m_flockVisibilityAngle  =  90.0f;        // the agent visibiliy angle
        
        public float  m_flockAlignmentWeight  =   1.0f;        // the alignment  weight for the agents in this flock (set to zero to ignore alignment)
        public float  m_flockCohesionWeight   =   1.0f;        // the cohesion   weight for the agents in this flock (set to zero to ignore cohesion)
        public float  m_flockSeparationWeight =   1.5f;        // the separation weight for the agents in this flock (set to zero to ignore separation)

        public float  m_flockAlignmentRadius  =   6.0f;        // the flocking alignment  radius
        public float  m_flockCohesionRadius   =   6.0f;        // the flocking cohesion   radius
        public float  m_flockSeparationRadius =   3.0f;        // the flocking separation radius

        [Header("Arrive")]                                     
        public float  m_arriveDistance        =   2.0f;        // distance to object where we reach zero velocity in m (=the stop position)
        public float  m_slowingDistance       =   4.0f;        // distance to the stop position where we start slowing down (zero to disable arrive)
                                              
        [Header("Avoid")]            
        public string m_obstacleLayer         =  "Obstacles";  // the layer name containing all obstacle colliders
        public string m_wallLayer             =  "Walls";      // the layer name containing all walls colliders

        public float  m_avoidDistance         =   3.0f;        // length of feeler ray detecting obstacles and walls in m
        public float  m_avoidMaxForce         =   5.0f;        // max steering 'force' to avoid obstacles and walls in m/s  
        public float  m_avoidAngleThreshold   =   1.0f;        // angle threshold in degrees
    }
}

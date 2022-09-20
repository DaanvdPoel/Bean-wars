using UnityEngine;

namespace Steering
{
    public class Idle : Behavior 
    {
        public override Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position plus desired velocity, and return steering force          
            m_positionTarget  = context.m_position + dt * context.m_velocity;
            m_velocityDesired = Vector3.zero;
            return m_velocityDesired - context.m_velocity;
        }
    }
}

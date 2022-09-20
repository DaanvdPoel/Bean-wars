using UnityEngine;

namespace Steering
{
    public class Keyboard : Behavior 
    {
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // get requested direction from input 
            Vector3 requested_direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            // update target position
            if (requested_direction != Vector3.zero)
                m_positionTarget = context.m_position + requested_direction.normalized * context.m_settings.m_maxDesiredVelocity;
            else
                m_positionTarget = context.m_position;

            // calcute desired velocity and return the steering force
            m_velocityDesired = (m_positionTarget - context.m_position).normalized * context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }
    }
}

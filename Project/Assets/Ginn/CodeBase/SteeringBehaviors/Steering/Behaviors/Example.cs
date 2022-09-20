using UnityEngine;

namespace Steering
{
    public class Example : Behavior
    {
        override public void Start(BehaviorContext context)
        {
            base.Start(context);
            // initialize behavior specific stuff here
        }

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position, e.g.
            m_positionTarget = context.m_position;

            // calcute desired velocity and return the steering force, e.g.
            m_velocityDesired = (m_positionTarget - context.m_position).normalized * context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }

        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // here you draw the stuff specific to this behavior
        }
    }
}
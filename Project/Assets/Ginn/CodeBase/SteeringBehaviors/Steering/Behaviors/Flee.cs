using UnityEngine;

namespace Steering
{
    public class Flee : Behavior 
    {
        readonly GameObject m_target;

        public Flee(GameObject target)
        {
            m_target = target;
        }

        public override Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // stop if we are far enough away from the target
            if (context.m_settings.m_stopDistance > 0)
            if ((m_target.transform.position - context.m_position).sqrMagnitude > context.m_settings.m_stopDistance * context.m_settings.m_stopDistance)
                return Vector3.zero - context.m_velocity;

            // update target position plus desired velocity, and return steering force          
            m_positionTarget  = m_target.transform.position;
            m_velocityDesired = -(m_positionTarget - context.m_position).normalized * 
                                context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }
    }
}

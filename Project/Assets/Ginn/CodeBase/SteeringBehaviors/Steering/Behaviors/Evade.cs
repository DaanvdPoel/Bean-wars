using UnityEngine;

namespace Steering
{
    public class Evade : Behavior 
    {
        readonly private GameObject m_target;     // the target object

        private Vector3 m_previousTargetPosition; // previous target position in m
        private Vector3 m_currentTargetPosition;  // previous target position in m

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public Evade(GameObject target)
        {
            m_target                 = target;
            m_previousTargetPosition = target.transform.position;
            m_currentTargetPosition  = target.transform.position;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private Vector3 GetFuturePosition(float dt, BehaviorContext context)
        {
            // update current target position 
            m_previousTargetPosition = m_currentTargetPosition;
            m_currentTargetPosition  = m_target.transform.position;

            // calculate target velocity 
            Vector3 targetVelocity = (m_currentTargetPosition - m_previousTargetPosition) / dt;

            // return the target position in the near future
            return m_currentTargetPosition + targetVelocity * context.m_settings.m_lookAheadTime;
        }

        public override Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // stop if we are far enough away from the target
            if (context.m_settings.m_stopDistance > 0)
            if ((m_target.transform.position - context.m_position).sqrMagnitude > context.m_settings.m_stopDistance * context.m_settings.m_stopDistance)
                return Vector3.zero - context.m_velocity;

            // update target position plus desired velocity, and return steering force          
            m_positionTarget  = GetFuturePosition(dt, context);
            m_velocityDesired = -(m_positionTarget - context.m_position).normalized * context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }
    }
}

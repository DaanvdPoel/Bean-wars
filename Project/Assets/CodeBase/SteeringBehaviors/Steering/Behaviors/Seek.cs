using UnityEngine;

namespace Steering
{
    public class Seek : Behavior 
    {
        readonly GameObject m_target;

        public Seek(GameObject target)
        {
            m_target = target;
        }

        public override Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position plus desired velocity, and return steering force          
            m_positionTarget  = m_target.transform.position;
            if (ArriveEnabled(context) && WithinArriveSlowingDistance(context, m_positionTarget))
                m_velocityDesired = CalculateArriveSteeringForce(context, m_positionTarget);
            else
                m_velocityDesired = (m_positionTarget - context.m_position).normalized * context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }

        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            if (ArriveEnabled(context))
                OnDrawArriveGizmos(context);
        }
    }
}

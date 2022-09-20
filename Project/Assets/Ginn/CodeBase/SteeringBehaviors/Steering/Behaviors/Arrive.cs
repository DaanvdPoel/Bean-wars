using UnityEngine;

namespace Steering
{
    public class Arrive : Behavior 
    {
        readonly GameObject m_target;

        public Arrive(GameObject target)
        {
            m_target = target;
        }

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position 
            m_positionTarget     = m_target.transform.position;
            m_positionTarget.y   = context.m_position.y;

            // calculate actual stop offset 
            Vector3 stopVector   = (context.m_position - m_positionTarget).normalized * context.m_settings.m_arriveDistance;
            Vector3 stopPosition = m_positionTarget + stopVector;

            // calculate the target offset and distance
            Vector3 targetOffset = stopPosition - context.m_position;
            float   distance     = targetOffset.magnitude;

            // calculate the ramped speed and clip it
            float   rampedSpeed  = context.m_settings.m_maxDesiredVelocity * (distance / context.m_settings.m_slowingDistance);
            float   clippedSpeed = Mathf.Min(rampedSpeed, context.m_settings.m_maxDesiredVelocity);

            // update desired velocity and return steering force
            if (distance > 0.001f)
                m_velocityDesired = (clippedSpeed / distance) * targetOffset;
            else
                m_velocityDesired = Vector3.zero;
            return m_velocityDesired - context.m_velocity;
        }

        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);

            // draw a circle around the target so we can see where we start breaking and where we must stop
            DrawGizmos.DrawWireDisc(m_target.transform.position, context.m_settings.m_arriveDistance , Color.yellow);
            DrawGizmos.DrawWireDisc(m_target.transform.position, context.m_settings.m_slowingDistance, Color.yellow);
        }
    }
}

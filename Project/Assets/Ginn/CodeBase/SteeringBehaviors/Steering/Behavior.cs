using UnityEngine;

namespace Steering
{
    public abstract class Behavior: IBehavior
    {
        [Header("Behavior Runtime")]
        public Vector3 m_positionTarget     = Vector3.zero; // target position
        public Vector3 m_velocityDesired    = Vector3.zero; // desired velocity

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public virtual void Start(BehaviorContext context)
        {
            m_positionTarget = context.m_position;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public abstract Vector3 CalculateSteeringForce(float dt, BehaviorContext context);

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public virtual void OnDrawGizmos(BehaviorContext context)
        {
            // draw a line to the target position
#if false
            DrawGizmos.DrawLine(context.m_position, m_positionTarget, Color.yellow);
#endif

            // draw desired velocity
            DrawGizmos.DrawRay(context.m_position, m_velocityDesired, Color.blue);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public bool ArriveEnabled(BehaviorContext context)
        {
            return context.m_settings.m_slowingDistance > 0.0f;
        }

        public bool WithinArriveSlowingDistance(BehaviorContext context, Vector3 positionTarget)
        {
            return (positionTarget - context.m_position).sqrMagnitude < context.m_settings.m_slowingDistance * context.m_settings.m_slowingDistance;
        }

        public Vector3 CalculateArriveSteeringForce(BehaviorContext context, Vector3 positionTarget)
        {
            // make sure we have a legal slowing distance
            if (!ArriveEnabled(context))
                return Vector3.zero;

            // update target position 
            m_positionTarget     = positionTarget;
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

        public void OnDrawArriveGizmos(BehaviorContext context)
        {
            // draw a circle around the target so we can see where we start breaking and where we must stop
            DrawGizmos.DrawWireDisc(m_positionTarget, context.m_settings.m_arriveDistance, Color.yellow);
            DrawGizmos.DrawWireDisc(m_positionTarget, context.m_settings.m_slowingDistance, Color.yellow);
        }
    }
}
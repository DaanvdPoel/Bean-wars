using UnityEngine;

namespace Steering
{
    public class Pursue : Behavior 
    {
        public GameObject m_target;     // the target object
        private SimpleBehaviorTree.Examples.UnitBrain brain; // the unitBrain of this object
        private Vector3 m_previousTargetPosition; // previous target position in m
        private Vector3 m_currentTargetPosition;  // previous target position in m

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public Pursue(GameObject target, SimpleBehaviorTree.Examples.UnitBrain unitBrain)
        {
            brain = unitBrain;
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
            try
            {
                m_currentTargetPosition = m_target.transform.position;
            }
            catch
            {
                m_target = brain.target;
                try
                { 
                    m_currentTargetPosition = m_target.transform.position; 
                }
                catch
                {
                    return m_previousTargetPosition;
                }

            }
            // calculate target velocity 
            Vector3 targetVelocity = (m_currentTargetPosition - m_previousTargetPosition) / dt;

            // return the target position in the near future
            return m_currentTargetPosition + targetVelocity * context.m_settings.m_lookAheadTime;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public override Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position plus desired velocity, and return steering force          
            m_positionTarget  = GetFuturePosition(dt, context);
            if (ArriveEnabled(context) && WithinArriveSlowingDistance(context, m_positionTarget))
                m_velocityDesired = CalculateArriveSteeringForce(context, m_positionTarget);
            else
                m_velocityDesired = (m_positionTarget - context.m_position).normalized * context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            if (ArriveEnabled(context))
                OnDrawArriveGizmos(context);
        }
    }
}

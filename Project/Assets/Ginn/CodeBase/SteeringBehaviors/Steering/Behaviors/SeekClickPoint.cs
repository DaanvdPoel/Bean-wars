using UnityEngine;

namespace Steering
{
    public class SeekClickPoint : Behavior 
    {
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position
            // (used to be Input.GetMouseButtonDown(0) but clicks can be missed since we update via FixedUpdate())
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
                {
                    // update target position
                    m_positionTarget   = hit.point;
                    m_positionTarget.y = context.m_position.y; // make sure to stay at the same level
                }
            }

            // calcute desired velocity and return the steering force
            if (ArriveEnabled(context) && WithinArriveSlowingDistance(context, m_positionTarget))
                m_velocityDesired = CalculateArriveSteeringForce(context, m_positionTarget);
            else
                m_velocityDesired = (m_positionTarget - context.m_position).normalized * context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }

        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            DrawGizmos.DrawSolidDisc(m_positionTarget, 0.25f, Color.white);
            if (ArriveEnabled(context))
                OnDrawArriveGizmos(context);
        }
    }
}


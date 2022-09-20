using UnityEngine;

namespace Steering
{
    public class AvoidObstacle : Behavior 
    {
        private int     m_obstacleLayer; // the active obstacle layer mask

        // info used to draw gizmos (or not)
        private bool    m_doAvoidObject; // true if we are avoiding an obstacle
        private Vector3 m_hitPoint;      // raycast results: the current hit point

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            base.Start(context);

            // get layer mask using the avoid layer name
            m_obstacleLayer = LayerMask.GetMask(context.m_settings.m_obstacleLayer);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // construct ray from position and velocity and check for the nearest collider
            Ray ray         = new Ray(context.m_position, context.m_velocity);
            m_doAvoidObject = Physics.Raycast(ray, out RaycastHit hit, context.m_settings.m_avoidDistance, m_obstacleLayer, QueryTriggerInteraction.Ignore);

            // return zero steering force if no collider found
            if (!m_doAvoidObject)
                return Vector3.zero;

            // remember hit point for drawing gizmos
            m_hitPoint = hit.point;

            // calculate desired velocity: (hit point - collider position).normalized * avoidMaxForce
            m_velocityDesired = (hit.point - hit.collider.transform.position).normalized * context.m_settings.m_avoidMaxForce;

            // make sure desired velocity and velocity are not aligned
            float angle = Vector3.Angle(m_velocityDesired.With(y:0), context.m_velocity.With(y:0));
            if (angle > 179.0f)
                m_velocityDesired = Vector3.Cross(Vector3.up, context.m_velocity).normalized * context.m_settings.m_avoidMaxForce;

            // update target position and return steering force  
            m_positionTarget = context.m_position + m_velocityDesired; // fake target just used to draw gizmos!
            return m_velocityDesired - context.m_velocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void OnDrawGizmos(BehaviorContext context)
        {
            // do nothing if we are disabled
            if (context.m_settings.m_avoidMaxForce <= 0.0f)
                return;

            // draw ray from current position to sensor position
            DrawGizmos.DrawRay(context.m_position, 
                               context.m_velocity.normalized * context.m_settings.m_avoidDistance, 
                               m_doAvoidObject ? Color.black : Color.grey);

            // draw feedback on collision in own colors
            if (m_doAvoidObject)
            {
                DrawGizmos.DrawWireDisc(m_hitPoint, 0.25f            , Color.black);
                DrawGizmos.DrawRay     (m_hitPoint, m_velocityDesired, Color.green);
            }
        }
    }
}

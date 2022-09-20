using UnityEngine;

namespace Steering.Refactored
{
    public abstract class Avoid : Behavior 
    {
        // delegate used to calculate the desired velocity
        public delegate Vector3 CalculateDesiredVelocity(RaycastHit hit, float maxForce);

        // repositioning properties
        public float m_offset = 0.0f; // ray origin offset relative to current position on axis perpendicular to velocity
        public float m_angle  = 0.0f; // ray direction angle relative to current velocity
        public float m_scale  = 1.0f; // scale factor maximum ray distance 

        // runtime info
        private int     m_layerMask;                                 // the active layer mask
        private CalculateDesiredVelocity m_calculateDesiredVelocity; // the calculate method

        // info used to draw gizmos (or not)
        private Ray     m_ray;               // the ray used to look ahead
        private float   m_lookAheadDistance; // the look ahead distance
        private bool    m_doAvoidObject;     // true if we are avoiding an obstacle
        private Vector3 m_hitPoint;          // raycast results: the current hit point

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        protected void Start(string layerName, CalculateDesiredVelocity calculate, BehaviorContext context)
        {
            base.Start(context);

            // get layer mask 
            m_layerMask                = LayerMask.GetMask(layerName);
            m_calculateDesiredVelocity = calculate;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // construct ray from position and velocity and check for the nearest collider
            if (m_offset == 0.0f && m_angle == 0.0f)
                m_ray = new Ray(context.m_position, context.m_velocity);
            else
            {
                Vector3 position = context.m_position;

                // translate position if there is an offset
                if (m_offset != 0.0f)
                {
                    // calculate a vector perpendicular to velocity
                    Vector3 perpendicular = Vector3.Cross(Vector3.up, context.m_velocity).normalized;

                    // shift position to the left or the right
                    position += perpendicular * m_offset;
                }

                // calculate direction
                Vector3 direction = Quaternion.AngleAxis(m_angle, Vector3.up) * context.m_velocity;

                // create the new ray using shifted position and rotated direct
                m_ray = new Ray(position, direction);
            }
         
            // calculate active look ahead distance
            m_lookAheadDistance = context.m_settings.m_avoidDistance * m_scale;

            // check for the nearest collider
            m_doAvoidObject = Physics.Raycast(m_ray, out RaycastHit hit, m_lookAheadDistance, m_layerMask, QueryTriggerInteraction.Ignore);

            // return zero steering force if no collider found
            if (!m_doAvoidObject)
                return Vector3.zero;

            // remember hit point for drawing gizmos
            m_hitPoint = hit.point;

            // calculate desired velocity: (hit point - collider position).normalized * avoidMaxForce
            m_velocityDesired = m_calculateDesiredVelocity(hit, context.m_settings.m_avoidMaxForce);

            // make sure desired velocity and velocity are not aligned
#if true
            float angle = Vector3.SignedAngle(-m_velocityDesired.With(y:0), context.m_velocity.With(y:0), Vector3.up);
            if (angle >= 0.0 && angle < context.m_settings.m_avoidAngleThreshold)
                m_velocityDesired = Vector3.Cross(Vector3.up  , context.m_velocity) * context.m_settings.m_avoidMaxForce;
            else if (angle <= 0.0f && angle > -context.m_settings.m_avoidAngleThreshold)
                m_velocityDesired = Vector3.Cross(Vector3.down, context.m_velocity) * context.m_settings.m_avoidMaxForce;
#else
            float angle = Vector3.Angle(-m_velocityDesired.With(y:0), context.m_velocity.With(y:0));
            if (angle < context.m_settings.m_avoidAngleThreshold)
                m_velocityDesired = Vector3.Cross(Vector3.up, context.m_velocity).normalized * context.m_settings.m_avoidMaxForce;
#endif

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
            DrawGizmos.DrawRay(m_ray.origin,
                            m_ray.direction.normalized * m_lookAheadDistance,
                            m_doAvoidObject ? Color.black : Color.grey);

            // draw feedback on collision in own colors
            if (m_doAvoidObject)
            {
                DrawGizmos.DrawWireDisc(m_hitPoint, 0.25f, Color.black);
                DrawGizmos.DrawRay     (m_hitPoint, m_velocityDesired, Color.green);
            }
        }
    }
}

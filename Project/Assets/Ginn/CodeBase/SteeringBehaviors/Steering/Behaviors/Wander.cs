using UnityEngine;

namespace Steering
{
    public class Wander : Behavior 
    {
        private float m_wanderAngle;   // wander angle in radians
        private int   m_obstacleLayer; // the active obstacle layer mask

        public Wander(Transform transform)
        {
            // calculate the initial wander angle that matches the current transform orientation
            m_wanderAngle = Vector3.SignedAngle(transform.forward, Vector3.right, Vector3.up) * Mathf.Deg2Rad;
        }

        override public void Start(BehaviorContext context)
        {
            base.Start(context);

            // get layer mask using the avoid layer name
            m_obstacleLayer = LayerMask.GetMask(context.m_settings.m_obstacleLayer);
        }

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // improving readability
            float wanderCircleDistance = context.m_settings.m_wanderCircleDistance;
            float wanderCircleRadius   = context.m_settings.m_wanderCircleRadius;
            float wanderNoiseAngle     = context.m_settings.m_wanderNoiseAngle;

            // update the wander delta with random angle within the range defined by noise angle 
            m_wanderAngle += Random.Range(-0.5f * wanderNoiseAngle * Mathf.Deg2Rad,
                                           0.5f * wanderNoiseAngle * Mathf.Deg2Rad);

            // make sure not to run into obstacles by steering the other way if we are about to hit something
            Ray ray = new Ray(context.m_position, m_positionTarget - context.m_position);
            if (Physics.Raycast(ray, context.m_settings.m_avoidDistance, m_obstacleLayer, QueryTriggerInteraction.Ignore))
                m_wanderAngle = (m_wanderAngle + Mathf.PI) % 2.0f * Mathf.PI; // wander 180 degrees the other way to avoid hitting an obstacle

            // calculate the center of the circle
            Vector3 centerOfCircle = context.m_position + context.m_velocity.normalized * wanderCircleDistance;

            // calculate the offset on the circle
            Vector3 offset = new Vector3(wanderCircleRadius * Mathf.Cos(m_wanderAngle),
                                         0.0f,
                                         wanderCircleRadius * Mathf.Sin(m_wanderAngle));
            
            // update target position plus desired velocity and return steering force            
            m_positionTarget  = centerOfCircle + offset;
            m_velocityDesired = (m_positionTarget - context.m_position).normalized * context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }

        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);

            // improving readability
            float wanderCircleDistance = context.m_settings.m_wanderCircleDistance;
            float wanderCircleRadius   = context.m_settings.m_wanderCircleRadius;
            float wanderNoiseAngle     = context.m_settings.m_wanderNoiseAngle;

            // draw circle
            Vector3 centerOfCircle = context.m_position + context.m_velocity.normalized * wanderCircleDistance;
            DrawGizmos.DrawWireDisc(centerOfCircle, wanderCircleRadius, Color.black);

            // draw noise lines
            float a = wanderNoiseAngle * Mathf.Deg2Rad;
            Vector3 rangeMin = new Vector3(wanderCircleRadius * Mathf.Cos(m_wanderAngle - a),
                                           0.0f,
                                           wanderCircleRadius * Mathf.Sin(m_wanderAngle - a));

            Vector3 rangeMax = new Vector3(wanderCircleRadius * Mathf.Cos(m_wanderAngle + a),
                                           0.0f,
                                           wanderCircleRadius * Mathf.Sin(m_wanderAngle + a));

            Debug.DrawLine(centerOfCircle, centerOfCircle + rangeMin, Color.black);
            Debug.DrawLine(centerOfCircle, centerOfCircle + rangeMax, Color.black);
        }
    }
}

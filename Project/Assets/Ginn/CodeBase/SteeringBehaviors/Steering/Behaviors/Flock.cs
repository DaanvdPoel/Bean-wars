using UnityEngine;

namespace Steering
{
    public class Flock : Behavior 
    {
        private readonly Collider m_myCollider;

        private int               m_flockLayer;
        private float             m_largestRadius;

        public Flock(GameObject obj)
        {
            m_myCollider = obj.GetComponent<Collider>();
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            base.Start(context);

            // get layer mask using the flock layer name
            m_flockLayer = LayerMask.GetMask(context.m_settings.m_flockLayer);

            // determine largest radius
            m_largestRadius = 0.0f;
            if (context.m_settings.m_flockAlignmentWeight > 0.0f)
                m_largestRadius = Mathf.Max(m_largestRadius, context.m_settings.m_flockAlignmentRadius);
            if (context.m_settings.m_flockCohesionWeight > 0.0f)
                m_largestRadius = Mathf.Max(m_largestRadius, context.m_settings.m_flockCohesionRadius);
            if (context.m_settings.m_flockSeparationWeight > 0.0f)
                m_largestRadius = Mathf.Max(m_largestRadius, context.m_settings.m_flockSeparationRadius);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private Vector3 CalculateDesiredVelocity(BehaviorContext context)
        {
            // find all neighbors
            Collider[] neighbors = Physics.OverlapSphere(context.m_position, m_largestRadius, m_flockLayer, QueryTriggerInteraction.Ignore);
            if (neighbors.Length == 0)
                return Vector3.zero;

            // prepare to calculate the three flock calculation forces
            FlockAlignment  alignment  = new FlockAlignment (context.m_settings.m_flockAlignmentRadius );
            FlockCohesion   cohesion   = new FlockCohesion  (context.m_settings.m_flockCohesionRadius  );
            FlockSeparation separation = new FlockSeparation(context.m_settings.m_flockSeparationRadius);
            
            // process all neigbors
            foreach (Collider neighbor in neighbors)
            {
                // skip this agent
                if (neighbor == m_myCollider)
                    continue;

                // get steering component from neighbor (if any)
                Steering neighborSteering = neighbor.gameObject.GetComponent<Steering>();
                if (neighborSteering == null)
                {
                    Debug.LogError($"ERROR: Flock Behavior found neighbor in layer {context.m_settings.m_flockLayer} without Steering script!");
                    continue;
                }

                // calcute direction and squared distance to neighbor
                Vector3 neighborDirection = neighborSteering.m_position - context.m_position;
                float   sqrDistance       = neighborDirection.sqrMagnitude;

                // skip neigbor if not in sight
                if (Vector3.Angle(m_myCollider.transform.forward, neighborDirection) > context.m_settings.m_flockVisibilityAngle)
                    continue;

                // update calculations
                alignment .AddVelocity (sqrDistance, neighborSteering.m_velocity);
                cohesion  .AddPosition (sqrDistance, neighborSteering.m_position);
                separation.AddDirection(sqrDistance, neighborDirection);
            }

            // calculate desired velocity
            Vector3 desiredVelocity = alignment .DesiredVelocity()                   * context.m_settings.m_flockAlignmentWeight +
                                      cohesion  .DesiredVelocity(context.m_position) * context.m_settings.m_flockCohesionWeight  +
                                      separation.DesiredVelocity()                   * context.m_settings.m_flockSeparationWeight;
            return desiredVelocity.normalized * context.m_settings.m_maxDesiredVelocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public override Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position plus desired velocity, and return steering force 
            m_velocityDesired = CalculateDesiredVelocity(context);
            m_positionTarget  = context.m_position + m_velocityDesired * dt;
            return m_velocityDesired - context.m_velocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
#if false
            DrawGizmos.DrawWireDisc(context.m_position, m_largestRadius, Color.black);
#endif
#if false
            // calculate the world rotation (the angle the velocity makes in with the vector to the right)
            float worldRotation = Vector3.SignedAngle(Vector3.right, context.m_velocity, -Vector3.up) * Mathf.Deg2Rad;

            // draw view lines
            float a = context.m_settings.m_flockVisibilityAngle * Mathf.Deg2Rad;
            Vector3 rangeMin = new Vector3(m_largestRadius * Mathf.Cos(worldRotation - a),
                                           0.0f,
                                           m_largestRadius * Mathf.Sin(worldRotation - a));

            Vector3 rangeMax = new Vector3(m_largestRadius * Mathf.Cos(worldRotation + a),
                                           0.0f,
                                           m_largestRadius * Mathf.Sin(worldRotation + a));

            Debug.DrawLine(context.m_position, context.m_position + rangeMin, Color.black);
            Debug.DrawLine(context.m_position, context.m_position + rangeMax, Color.black);
#endif
        }
    }
}

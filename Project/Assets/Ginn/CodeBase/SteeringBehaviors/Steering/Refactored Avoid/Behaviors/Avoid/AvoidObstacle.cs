using UnityEngine;

namespace Steering.Refactored
{
    public class AvoidObstacle : Avoid
    {
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            Start(context.m_settings.m_obstacleLayer, CalculateDesiredAvoidObstacleVelocity, context);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private Vector3 CalculateDesiredAvoidObstacleVelocity(RaycastHit hit, float maxForce)
        {
            // calculate desired velocity: (hit point - collider position).normalized * avoidMaxForce
            return (hit.point - hit.collider.transform.position).normalized * maxForce;
        }
    }
}
using UnityEngine;

namespace Steering.Refactored
{
    public class AvoidWall : Avoid
    {
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            Start(context.m_settings.m_wallLayer, CalculateDesiredAvoidWallVelocity, context);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private Vector3 CalculateDesiredAvoidWallVelocity(RaycastHit hit, float maxForce)
        {
            // calculate desired velocity: hit normal * wallMaxForce
            return hit.normal * maxForce;
        }
    }
}
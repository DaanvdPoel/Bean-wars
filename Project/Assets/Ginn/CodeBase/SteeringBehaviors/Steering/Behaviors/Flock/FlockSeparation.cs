using UnityEngine;

namespace Steering
{
    /// <summary>
    /// Support class to calculate the desired velocity to move away from the neighbor agents.
    /// </summary>
    class FlockSeparation
    {
        private float   m_sqrRadius;
        private int     m_neighborCount = 0;
        private Vector3 m_total         = Vector3.zero;

        public FlockSeparation(float radius)
        {
            m_sqrRadius = radius * radius;
        }

        public void AddDirection(float sqrDistance, Vector3 neighborDirection)
        {
            if (sqrDistance <= m_sqrRadius)
            {
                m_total += neighborDirection.normalized;
                m_neighborCount++;
            }
        }

        public Vector3 DesiredVelocity()
        {
            if (m_neighborCount > 0)
                return -(m_total / (float)m_neighborCount).normalized;
            else
                return Vector3.zero;
        }
    }
}

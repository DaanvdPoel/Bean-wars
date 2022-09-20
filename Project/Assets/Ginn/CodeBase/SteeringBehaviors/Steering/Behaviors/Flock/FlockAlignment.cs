using UnityEngine;

namespace Steering
{
    /// <summary>
    /// Support class to calculate the desired velocity to match the average velocity of the neighbor agents.
    /// </summary>
    class FlockAlignment
    {
        private float   m_sqrRadius;
        private int     m_neighborCount = 0;
        private Vector3 m_total         = Vector3.zero;

        public FlockAlignment(float radius)
        {
            m_sqrRadius = radius * radius;
        }

        public void AddVelocity(float sqrDistance, Vector3 neighborVelocity)
        {
            if (sqrDistance <= m_sqrRadius)
            {
                m_total += neighborVelocity;
                m_neighborCount++;
            }
        }

        public Vector3 DesiredVelocity()
        {
            if (m_neighborCount > 0)
                return (m_total / (float)m_neighborCount).normalized;
            else
                return Vector3.zero;
        }
    }
}

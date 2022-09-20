using UnityEngine;

namespace Steering
{
    /// <summary>
    /// Support class to calculate the desired velocity to steer towards 'center of mass' aka the average position of the neighbor agents.
    /// </summary>
    class FlockCohesion
    {
        private float   m_sqrRadius;
        private int     m_neighborCount = 0;
        private Vector3 m_total         = Vector3.zero;

        public FlockCohesion(float radius)
        {
            m_sqrRadius = radius * radius;
        }

        public void AddPosition(float sqrDistance, Vector3 neighborPosition)
        {
            if (sqrDistance <= m_sqrRadius)
            {
                m_total += neighborPosition;
                m_neighborCount++;
            }
        }

        public Vector3 DesiredVelocity(Vector3 position)
        {
            if (m_neighborCount > 0)
            {
                Vector3 average = m_total / (float)m_neighborCount;
                return (average - position).normalized;
            }
            else
                return Vector3.zero;
        }
    }
}

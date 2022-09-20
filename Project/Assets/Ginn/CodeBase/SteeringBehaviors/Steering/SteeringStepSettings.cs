using System;
using UnityEngine;

namespace Steering
{
    [Serializable]
    public struct SteeringStepSettings
    {
        public enum UpdateMethod { Coroutine, Update, FixedUpdate };

        public UpdateMethod m_location;     // which method do we use to step the behaviors
        public float        m_stepInterval; // step interval in seconds in case of stepping in Coroutine

        // Coroutine support
        private float m_time;
        private float m_prevTime;

        public float DeltaTime()
        {
            m_prevTime = m_time;
            m_time     = Time.time;
            float dt   = m_time - m_prevTime;
            if (dt <= 0.0f || dt > m_stepInterval * 2.0f)
                dt = m_stepInterval;
            return dt;
        }
    }
}

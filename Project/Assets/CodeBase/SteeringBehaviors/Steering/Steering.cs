using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    using BehaviorList = List<IBehavior>;

    public class Steering : MonoBehaviour
    {
        [Header("Steering Settings")]
        public string           m_label;                          // label to show when running
        public SteeringSettings m_settings;                       // the steering settings for all behaviors

        [Header("Steering Runtime")]
        public Vector3          m_position  = Vector3.zero;       // current position
        public Vector3          m_velocity  = Vector3.zero;       // current velocity
        public Vector3          m_steering  = Vector3.zero;       // steering force
        public BehaviorList     m_behaviors = new BehaviorList(); // all behaviors for this steering object

        private SteeringStepSettings m_stepSettings;

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private void Start()
        {
            // get the current position
            m_position = transform.position;

            // copy the step settings and start coroutine if required
            m_stepSettings = SteeringSetup.GetInstance().m_stepSettings;
            if (m_stepSettings.m_location == SteeringStepSettings.UpdateMethod.Coroutine)
                StartCoroutine(StepCoroutine());
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private IEnumerator StepCoroutine()
        {
            while (true)
            {
                float dt = m_stepSettings.DeltaTime();
                Step(dt);
                yield return new WaitForSeconds(dt);
            }
        }

        private void Update()
        {
            if (m_stepSettings.m_location == SteeringStepSettings.UpdateMethod.Update)
                Step(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (m_stepSettings.m_location == SteeringStepSettings.UpdateMethod.FixedUpdate)
                Step(Time.fixedDeltaTime);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private void Step(float dt)
        {
            // calculate steering 
            BehaviorContext context = new BehaviorContext(m_position, m_velocity, m_settings);
            m_steering = Vector3.zero;
            foreach (IBehavior behavior in m_behaviors)
            {
                m_steering += behavior.CalculateSteeringForce(dt, context);
            }

            // make sure y is fixed...steering is only done in the xz plane
            m_steering.y = 0.0f;

            // clamp steering force to maximum steering force and apply mass
            m_steering  = Vector3.ClampMagnitude(m_steering, m_settings.m_maxSteeringForce);
            m_steering /= m_settings.m_mass;

            // update velocity with steering force, and update position
            m_velocity  = Vector3.ClampMagnitude(m_velocity + m_steering, m_settings.m_maxSpeed);
            float threshold = m_settings.m_minSpeed * m_settings.m_minSpeed;
            if (m_steering.sqrMagnitude > threshold || m_velocity.sqrMagnitude > threshold)
                m_position += m_velocity * dt;
            else
                m_velocity  = Vector3.zero;

            // update object with new position
            transform.position = m_position;
            transform.LookAt(m_position + dt * m_velocity);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private void OnDrawGizmos()
        {
            DrawGizmos.DrawRay  (transform.position, m_velocity, Color.red);
            DrawGizmos.DrawLabel(transform.position, m_label,    Color.white);

            // allow all behaviors to draw feedback as well
            BehaviorContext context = new BehaviorContext(m_position, m_velocity, m_settings);
            foreach (IBehavior behavior in m_behaviors)
            {
                behavior.OnDrawGizmos(context);
            }
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public void SetBehaviors(BehaviorList behaviors, string label = "", SteeringSettings settings = null)
        {
            // remember the new settings
            m_label     = label;
            m_behaviors = behaviors;
            if (settings)
                m_settings = settings;

            // start all behaviors
            BehaviorContext context = new BehaviorContext(m_position, m_velocity, m_settings);
            foreach (IBehavior behavior in m_behaviors)
            {
                behavior.Start(context);
            }
        }
    }
}

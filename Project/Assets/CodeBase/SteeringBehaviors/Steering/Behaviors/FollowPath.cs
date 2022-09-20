using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    using WPList = List<GameObject>;

    public class FollowPath : Behavior 
    {
        private readonly WPList m_waypoints; // the original waypoints 

        private WPList m_wps;   // a copy of the original waypoints prepared for forward/backward/pingpong
        private int    m_index; // index in list with path following waypoints

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public FollowPath(List<GameObject> waypoints)
        {
            m_waypoints = waypoints;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public override void Start(BehaviorContext context)
        {
            base.Start(context);

            // reset waypoints
            m_index = 0;
            m_wps   = null;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private void InitWPS(BehaviorContext context)
        {
            // copy waypoints from settings object and prepare for ping pong if needed
            switch (context.m_settings.m_followPathMode)
            {
                case SteeringSettings.FPM.Forwards:
                    m_wps = new WPList(m_waypoints);
                    break;

                case SteeringSettings.FPM.Backwards:
                    m_wps = new WPList(m_waypoints);
                    m_wps.Reverse(); 
                    break;

                case SteeringSettings.FPM.PingPong:
                    m_wps = new WPList(m_waypoints);
                    WPList wps = new WPList(m_waypoints);
                    wps.Reverse();
                    m_wps.AddRange(wps);
                    break;

                case SteeringSettings.FPM.Random:
                    m_wps = new WPList(m_waypoints);
                    Shuffler.Shuffle(m_wps, new System.Random());
                    break;
            }

            // set initial position target
            if (m_wps.Count > 0)
                m_positionTarget = m_wps[0].transform.position;
        }

        private Vector3 GetCurrentWaypoint(BehaviorContext context)
        {
            // switch to the next waypoint if we are close to the current waypoint
            float distanceToWaypoint = (m_wps[m_index].transform.position - context.m_position).magnitude;
            if (distanceToWaypoint < context.m_settings.m_followPathRadius)
            {
                if (m_index < m_wps.Count - 1)
                    ++m_index;
                else if (context.m_settings.m_followPathLooping)
                    m_index = 0;
            }

            // return current waypoint
            return m_wps[m_index].transform.position;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // do nothing without waypoints
            if (m_wps == null)
                InitWPS(context);
            if (m_wps.Count == 0)
                return Vector3.zero;

            // determine if we are approaching the final waypoint and we need to arrive
            bool doArrive = false;
            if (context.m_settings.m_followPathMode == SteeringSettings.FPM.Forwards ||
                context.m_settings.m_followPathMode == SteeringSettings.FPM.Backwards ||
                context.m_settings.m_followPathMode == SteeringSettings.FPM.PingPong)
            {
                if (context.m_settings.m_followPathLooping == false && m_index == m_wps.Count - 1)
                    doArrive = true;
            }

            // update target position plus desired velocity and return steering force          
            m_positionTarget = GetCurrentWaypoint(context);
            if (doArrive && ArriveEnabled(context) && WithinArriveSlowingDistance(context, m_positionTarget))
                m_velocityDesired = CalculateArriveSteeringForce(context, m_positionTarget);
            else
                m_velocityDesired = (m_positionTarget - context.m_position).normalized * context.m_settings.m_maxDesiredVelocity;
            return m_velocityDesired - context.m_velocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);

            // draw lines between waypoints
            if (m_waypoints != null && m_waypoints.Count > 2)
            {
                GameObject prev_wp = null;
                foreach (var wp in m_waypoints)
                {
                    // draw circle
                    DrawGizmos.DrawWireDisc(wp.transform.position, context.m_settings.m_followPathRadius, Color.black);

                    // draw line between waypoints
                    if (prev_wp != null)
                        Debug.DrawLine(prev_wp.transform.position, wp.transform.position, Color.black);

                    // update previous waypoint with the current one
                    prev_wp = wp;
                }
            }
        }
    }
}

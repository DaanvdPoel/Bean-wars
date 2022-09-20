using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    using ColliderList = List<Collider>;
    using HideList     = List<Vector3>;

    public class Hide : Behavior 
    {
        readonly private GameObject m_target; // the target object we are hiding from
                                 
        // info used to find a hiding place and draw gizmos 
        private ColliderList m_colliders;     // all the colliders in the scene that match the hide layer
        private HideList     m_hidingPlaces;  // the list with hiding places
        private Vector3      m_hidingPlace;   // the current hiding place

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public Hide(GameObject target) 
        {
            m_target = target;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            base.Start(context);

            // find all obstacles that match our hide layer name
            m_colliders = FindCollidersWithLayer(context.m_settings.m_hideLayer);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        static public List<Collider> FindCollidersWithLayer(string layerName)
        {
            // get layer mask using the hide layer name
            int colliderLayer = LayerMask.NameToLayer(layerName);

            // get all collider game objects in the scene and find the ones with our layer
            Collider[] allColliders = GameObject.FindObjectsOfType(typeof(Collider)) as Collider[];
            List<Collider> colliders = new List<Collider>();
            foreach (Collider gameObject in allColliders)
            {
                if (gameObject.gameObject.layer == colliderLayer)
                    colliders.Add(gameObject);
            }

            // return the colliders that match the layer name
            return colliders;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public Vector3 CalculateHidingPlace(BehaviorContext context, Collider collider, Vector3 enemy_position)
        {
            // calculate place for the current obstacle
            Vector3 obstacleDirection = (collider.transform.position - enemy_position).normalized;
            Vector3 pointOtherSide    =  collider.transform.position + obstacleDirection;
            Vector3 hidingPlace       =  collider.ClosestPoint(pointOtherSide) + (obstacleDirection * context.m_settings.m_hideOffset);

            // return hiding place
            return hidingPlace;
        }

        public Vector3 CalculateHidingPlace(BehaviorContext context, Vector3 enemy_position)
        {
            // loop over colliders, find all hiding places, and find the nearest hiding place
            float closestDistanceSqr = float.MaxValue;
            m_hidingPlace            = context.m_position;
            m_hidingPlaces           = new HideList();
            for (int i = 0; i < m_colliders.Count; i++)
            {
                // calculate hiding place for the current obstacle and remember it so we can draw gizmos
                Vector3 hidingPlace = CalculateHidingPlace(context, m_colliders[i], enemy_position);
                m_hidingPlaces.Add(hidingPlace);

                // update closest hiding place if this hiding place is closer than the previous
                float distanceToHidingPlaceSqr = (context.m_position - hidingPlace).sqrMagnitude;
                if (distanceToHidingPlaceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceToHidingPlaceSqr; // we have a new closest point
                    m_hidingPlace      = hidingPlace;              // remember it as the new hiding place
                }
            }

            // return hiding place 
            return m_hidingPlace;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position plus desired velocity and return steering force    
            m_positionTarget  = CalculateHidingPlace(context, m_target.transform.position);
            if (ArriveEnabled(context) && WithinArriveSlowingDistance(context, m_positionTarget))
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

            // draw solid discs at all possible hiding places and the selected one
            foreach (Vector3 hidingPlace in m_hidingPlaces)
                DrawGizmos.DrawSolidDisc(hidingPlace, 0.25f, Color.blue);
            DrawGizmos.DrawWireDisc(m_hidingPlace, 0.35f, Color.blue);

            if (ArriveEnabled(context))
                OnDrawArriveGizmos(context);
        }
    }
}

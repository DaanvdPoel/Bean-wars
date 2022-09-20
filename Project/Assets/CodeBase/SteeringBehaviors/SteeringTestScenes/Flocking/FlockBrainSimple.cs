using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    [RequireComponent(typeof(Steering))]
    public class FlockBrainSimple : MonoBehaviour
    {
        [Header("Private")]
        private Steering m_steering;

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private void Start()
        {
            // get steering
            m_steering = GetComponent<Steering>();

            // add behaviors
            List<IBehavior> behaviors = new List<IBehavior>
            {
                new AvoidObstacle(),
                new AvoidWall    (),
                new Flock        (gameObject)
            };
            m_steering.SetBehaviors(behaviors, "Flock");
        }
    }
}

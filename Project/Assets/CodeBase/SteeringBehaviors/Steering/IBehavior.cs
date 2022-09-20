using UnityEngine;

namespace Steering
{
    public interface IBehavior
    {
        /// <summary>
        /// Allow the behavior to initialize.
        /// </summary>
        /// <param name="context">All the context information needed to perform the task at hand.</param>
        void Start(BehaviorContext context);

        /// <summary>
        /// Calculate the steering force contributed by this behavior.
        /// </summary>
        /// <param name="dt">The delta time for this step.</param>
        /// <param name="context">All the context information needed to perform the task at hand.</param>
        /// <returns></returns>
        Vector3 CalculateSteeringForce(float dt, BehaviorContext context);

        /// <summary>
        /// Draw the gizmos for this behavior.
        /// </summary>
        /// <param name="context">All the context information needed to perform the task at hand.</param>
        void OnDrawGizmos(BehaviorContext context);
    }
}

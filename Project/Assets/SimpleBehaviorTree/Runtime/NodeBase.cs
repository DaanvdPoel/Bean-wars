using UnityEngine.Events;

namespace SimpleBehaviorTree
{
    public abstract class NodeBase 
    {   
        /// <summary>
        /// The path to the task icons.
        /// </summary>
        protected const string PACKAGE_ROOT = "ROOT/Editor/Icons/Tasks";

        /// <summary>
        /// The default icon for all tasks/
        /// </summary>
        public virtual string IconPath => $"{PACKAGE_ROOT}/Play.png";

        /// <summary>
        /// True if this task has been active after the last tree reset.
        /// </summary>
        public bool HasBeenActive { get; private set; }

        /// <summary>
        /// Subscribe to this event if you want to know when this task is activated.
        /// </summary>
        private UnityEvent m_taskActive;
        public UnityEvent TaskActive => m_taskActive ?? (m_taskActive = new UnityEvent());

        /// <summary>
        /// Call this method to signal the editor this task is active.
        /// </summary>
        public void SetTaskActive()
        {
            // invoke the task active event so anyone that is listening knows this this task is active.
#if UNITY_EDITOR
            TaskActive.Invoke();
#endif

            // remember we are active
            HasBeenActive = true;
        }
    }
}

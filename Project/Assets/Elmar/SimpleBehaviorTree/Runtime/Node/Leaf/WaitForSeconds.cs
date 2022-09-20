namespace SimpleBehaviorTree
{
    /// <summary>
    /// The WaitForSeconds node waits a specified number of second.
    /// </summary>
    public class WaitForSeconds : LeafNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/HourglassFill.png";

        private float m_waitTime;
        private float m_timePassed = 0.0f;

        public float WaitTime { get { return m_waitTime; } set { m_waitTime = value; } }

        // The constructor requires the child node that this node wraps
        public WaitForSeconds(float waitTime) 
        {
            m_waitTime = waitTime;
        }
        public WaitForSeconds() { }

        // Prepare the node for first usage
        public override void Start() 
        {
            base.Start();

            // reset timer
            m_timePassed = 0.0f;
        }

        // Returns RUNNING while waiting for a specified number of seconds.
        // Returns SUCCESS after waiting the specified number of seconds.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinishedSuccess())
                return NodeState;

            // wait for it...
            m_timePassed += bb.m_dt;
            if (m_timePassed < m_waitTime)
                return SetNodeState(NodeState.RUNNING);
            else
                return SetNodeState(NodeState.SUCCESS);
        }
    }
}

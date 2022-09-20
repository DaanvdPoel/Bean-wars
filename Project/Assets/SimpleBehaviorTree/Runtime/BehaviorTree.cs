namespace SimpleBehaviorTree
{
    /// <summary>
    /// Convenience class that manages your behavior tree.
    /// </summary>
    [System.Serializable]
    public class BehaviorTree : IBehaviorTree
    {
        private           string            m_name = "Tree";     // the name of the tree
        private readonly  RootNode          m_rootNode;          // the root node we manage
        private readonly  Blackboard        m_blackboard;        // the blackboard used to update the tree
        private readonly  BlackboardUpdater m_blackboardUpdater; // the delegate that is called before each update to update the blackboard
        private           NodeState         m_nodeState;         // the current state of the root node

        /// <summary>
        /// The delegate you need to provide to update your own blackboard with every update.
        /// </summary>
        /// <param name="bb">The blackboard that needs to be updated.</param>
        public delegate void BlackboardUpdater(Blackboard bb);

        /// <summary>
        /// The name of the tree.
        /// </summary>
        public string Name { get { return m_name; } set { m_name = value; } }

        /// <summary>
        /// The root node of the tree we manage.
        /// </summary>
        public RootNode RootNode { get { return m_rootNode; } }

        /// <summary>
        /// The current state of the root node.
        /// </summary>
        public NodeState NodeState { get { return m_nodeState; } }

        /// <summary>
        /// Constructor for the BTManager.
        /// </summary>
        /// <param name="rootNode">The root node of the behavior tree to manage.</param>
        /// <param name="blackboard">The blackboard to be updated and passed on to the behavior tree with every update.</param>
        /// <param name="blackboardUpdater">Your method that updates your blackboard. Will be called with every update.</param>
        public BehaviorTree(RootNode rootNode, Blackboard blackboard, BlackboardUpdater blackboardUpdater)
        {
            m_rootNode          = rootNode;
            m_blackboard        = blackboard;
            m_blackboardUpdater = blackboardUpdater;
            m_nodeState         = NodeState.READY;
        }

        public BehaviorTree(Blackboard blackboard, BlackboardUpdater blackboardUpdater)
        {
            m_rootNode          = new RootNode();
            m_blackboard        = blackboard;
            m_blackboardUpdater = blackboardUpdater;
            m_nodeState         = NodeState.READY;
        }

        /// <summary>
        /// Update the behavior tree. 
        /// </summary>
        /// <param name="dt">Delta time to previous update in seconds.</param>
        public void Update(float dt)
        {
            // update blackboard
            m_blackboard.m_dt = dt;
            m_blackboardUpdater(m_blackboard);

            // restart behavior tree if the tree stopped running (returned with success or failure)
            if (m_nodeState != NodeState.RUNNING)
                Restart();

            // update behavior tree
            m_nodeState = m_rootNode.Update(m_blackboard);
        }

        /// <summary>
        /// Force restart the behavior tree.
        /// </summary>
        public void Restart()
        {
            m_rootNode.RestartTree();
        }
    }
}

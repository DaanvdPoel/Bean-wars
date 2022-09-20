using System.Collections.Generic;

namespace SimpleBehaviorTree
{
    /// <summary>
    /// Similar to a Sequence but evaluates all child nodes and never reports a failure. The exact number of fail or succeed nodes can be specified.
    /// </summary>
    public class Parallel : CompositeNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/CompareArrows.png";

        private int m_numAllowedToFail     =  0; // zero is none, less than zero is all
        private int m_numRequiredToSucceed = -1; // zero is none, less than zero is all

        private int m_actualNumFailed;
        private int m_actualNumSucceeded;

        /// <summary>
        /// The name of the node.
        /// </summary>
        public int NumAllowedToFail { get { return m_numAllowedToFail; } set { m_numAllowedToFail = value; } }
        public int NumRequiredToSucceed { get { return m_numRequiredToSucceed; } set { m_numRequiredToSucceed = value; } }

        // The constructor requires a list of child nodes to work
        public Parallel() { }
        public Parallel(List<INode> nodes, int numAllowedToFail=0, int numRequiredToSucceed=-1) : base(nodes) 
        {
            m_numAllowedToFail     = numAllowedToFail;
            m_numRequiredToSucceed = numRequiredToSucceed;
        }

        /// <summary>
        /// Prepare for first usage.
        /// </summary>
        public override void Start()
        {
            base.Start();

            // reset counts
            m_actualNumFailed    = 0;
            m_actualNumSucceeded = 0;
        }

        // Returns SUCCESS if the required number of child nodes failed or succeded.
        // Never reports FAILURE.
        // Returns RUNNING if any of the child nodes is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinishedSuccess())
                return NodeState;

            // process node
            foreach (INode child in Children)
            {
                // process node
                switch (child.Update(bb))
                {
                    case NodeState.FAILURE:
                        m_actualNumFailed++;
                        if (m_numAllowedToFail < 0 || m_numAllowedToFail > Children.Count)
                        {
                            if (m_actualNumFailed >= Children.Count)
                                return SetNodeState(NodeState.FAILURE);
                        }
                        else if (m_actualNumFailed >= m_numAllowedToFail)
                            return SetNodeState(NodeState.FAILURE);
                        continue;
                    case NodeState.SUCCESS:
                        m_actualNumSucceeded++;
                        if (m_numRequiredToSucceed < 0 || m_numRequiredToSucceed > Children.Count)
                        {
                            if (m_actualNumSucceeded >= Children.Count)
                                return SetNodeState(NodeState.SUCCESS);
                        }
                        else if (m_actualNumSucceeded >= m_numRequiredToSucceed)
                            return SetNodeState(NodeState.SUCCESS);
                        continue; 
                    case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                    default: continue;
                }
            }
            return SetNodeState(NodeState.RUNNING);
        }
    }
}
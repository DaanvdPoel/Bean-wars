using System.Collections.Generic;

namespace SimpleBehaviorTree
{
    /// <summary>
    /// Similar to a Selector but the children are updated in random order.
    /// </summary>
    public class RandomSelector : CompositeNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/LinearScale.png";

        private readonly List<INode> m_shuffledChildren = new List<INode>();

        // The constructor requires a list of child nodes to work
        public RandomSelector() { }
        public RandomSelector(List<INode> children) : base(children) { }

        // Prepare the node for first usage
        public override void Start()
        {
            base.Start();

            // create a shuffled copy of the list with original nodes
            List<INode> shuffledChildren = new List<INode>(Children);
            Shuffler.Shuffle(shuffledChildren, new System.Random());
        }

        // Returns SUCCESS immediately once a child node reports succeeded.
        // Reports FAILURE if all of the child nodes fails.
        // Returns RUNNING if any of the child nodes is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinished())
                return NodeState;

            // process nodes
            foreach (INode child in m_shuffledChildren)
            {
                switch (child.Update(bb))
                {
                    case NodeState.SUCCESS: return SetNodeState(NodeState.SUCCESS);
                    case NodeState.FAILURE: continue;
                    case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                    default: continue;
                }
            }
            return SetNodeState(NodeState.FAILURE);
        }
    }
}
namespace SimpleBehaviorTree
{
    /// <summary>
    /// An Inverter will invert or negate the result of their child node. Success becomes failure, and failure becomes success. They are most often used in conditional tests.
    /// </summary>
    public class Inverter : DecoratorNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/Invert.png";

        // The constructor requires the child node that this node wraps
        public Inverter() { }
        public Inverter(Node node) : base(node) { }

        // Returns SUCCESS if the child succeeeds. 
        // Reports FAILURE if the child fails.
        // Returns RUNNING if the child is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinished())
                return NodeState;

            // process node
            switch (Child.Update(bb))
            {
                case NodeState.SUCCESS: return SetNodeState(NodeState.FAILURE);
                case NodeState.FAILURE: return SetNodeState(NodeState.SUCCESS);
                case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                default: return SetNodeState(NodeState.SUCCESS);
            }
        }
    }
}

namespace SimpleBehaviorTree
{
    /// <summary>
    /// The ReturnSuccess node returns Success as soon as the child node is finished (with Success or Failure), but returns Running as long as the child node is running.
    /// </summary>
    public class ReturnSuccess : DecoratorNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/Checkmark.png";

        // The constructor requires the child node that this node wraps
        public ReturnSuccess() { }
        public ReturnSuccess(Node node) : base(node) { }

        // Returns SUCCESS if the child succeeeds. 
        // Reports SUCCESS if the child fails.
        // Returns RUNNING if the child is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinishedSuccess())
                return NodeState;

            // process node
            switch (Child.Update(bb))
            {
                case NodeState.SUCCESS: return SetNodeState(NodeState.SUCCESS);
                case NodeState.FAILURE: return SetNodeState(NodeState.SUCCESS);
                case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                default: return SetNodeState(NodeState.SUCCESS);
            }
        }
    }
}
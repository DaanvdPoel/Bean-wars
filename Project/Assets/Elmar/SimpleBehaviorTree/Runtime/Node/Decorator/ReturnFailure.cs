namespace SimpleBehaviorTree
{
    /// <summary>
    /// The ReturnFailure node returns Failure as soon as the child node is finished (with Success or Failure), but returns Running as long as the child node is running.
    /// </summary>
    public class ReturnFailure : DecoratorNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/Cancel.png";

        // The constructor requires the child node that this node wraps
        public ReturnFailure() { }
        public ReturnFailure(Node node) : base(node) { }

        // Returns FAILURE if the child succeeeds. 
        // Reports FAILURE if the child fails.
        // Returns RUNNING if the child is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinishedFailure())
                return NodeState;

            // process node
            switch (Child.Update(bb))
            {
                case NodeState.SUCCESS: return SetNodeState(NodeState.FAILURE);
                case NodeState.FAILURE: return SetNodeState(NodeState.FAILURE);
                case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                default: return SetNodeState(NodeState.SUCCESS);
            }
        }
    }
}
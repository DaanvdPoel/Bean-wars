namespace SimpleBehaviorTree
{
    /// <summary>
    /// Like a RepeatForever node, the RepeatUntilFailure will continue to reprocess their child. That is until the child finally returns a success, at which point the RepeatUntilSuccess will return success to its parent.
    /// </summary>
    public class RepeatUntilSuccess : DecoratorNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/EventAvailable.png";

        // The constructor requires the child node that this node wraps
        public RepeatUntilSuccess() { }
        public RepeatUntilSuccess(Node node) : base(node) { }

        // Returns SUCCESS if the child succeeeds. 
        // Reports RUNNING if the child fails.
        // Returns RUNNING if the child is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinishedSuccess())
                return NodeState;

            // process node
            Child.RestartTree();
            switch (Child.Update(bb))
            {
                case NodeState.SUCCESS: return SetNodeState(NodeState.SUCCESS);
                case NodeState.FAILURE: return SetNodeState(NodeState.RUNNING);
                case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                default: return SetNodeState(NodeState.SUCCESS);
            }
        }
    }
}
namespace SimpleBehaviorTree
{
    /// <summary>
    /// Like a RepeatForever node, the RepeatUntilFailure will continue to reprocess their child. That is until the child finally returns a failure, at which point the RepeatUntilFailure will return success to its parent.
    /// </summary>
    public class RepeatUntilFailure : DecoratorNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/EventBusy.png";

        // The constructor requires the child node that this node wraps
        public RepeatUntilFailure() { }
        public RepeatUntilFailure(Node node) : base(node) { }

        // Returns RUNNING if the child succeeeds. 
        // Reports SUCCESS if the child fails.
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
                case NodeState.SUCCESS: return SetNodeState(NodeState.RUNNING);
                case NodeState.FAILURE: return SetNodeState(NodeState.SUCCESS);
                case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                default: return SetNodeState(NodeState.SUCCESS);
            }
        }
    }
}
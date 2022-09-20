namespace SimpleBehaviorTree
{
    /// <summary>
    /// A RepeatForever node will reprocess its child node each time its child returns a result. These are often used at the very base of the tree, to make the tree to run continuously. Repeaters may optionally run their children a set number of times before returning to their parent.
    /// </summary>
    public class RepeatForever : DecoratorNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/Repeat.png";

        // The constructor requires the child node that this node wraps
        public RepeatForever() { }
        public RepeatForever(Node node) : base(node) { }

        // Returns RUNNING if the child succeeeds. 
        // Reports RUNNING if the child fails.
        // Returns RUNNING if the child is running.
        public override NodeState Update(Blackboard bb)
        {
            // process node
            Child.RestartTree();
            switch (Child.Update(bb))
            {
                case NodeState.SUCCESS: return SetNodeState(NodeState.RUNNING);
                case NodeState.FAILURE: return SetNodeState(NodeState.RUNNING);
                case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                default: return SetNodeState(NodeState.SUCCESS);
            }
        }
    } 
}

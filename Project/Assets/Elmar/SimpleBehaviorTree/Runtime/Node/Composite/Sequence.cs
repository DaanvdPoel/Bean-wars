using System.Collections.Generic;

namespace SimpleBehaviorTree
{
    /// <summary>
    /// A sequence will visit each child in order, starting with the first, and when that succeeds will call the second, and so on down the list of children. If any child fails it will immediately return failure to the parent. If the last child in the sequence succeeds, then the sequence will return success to its parent.
    /// </summary>
    public class Sequence : CompositeNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/RightArrow.png";

        // The constructor requires a list of child nodes to work
        public Sequence() { }
        public Sequence(List<INode> nodes) : base(nodes) { }

        // Returns SUCCESS if none of the child nodes are running (hence all of the child nodes succeeded). 
        // Reports FAILURE if any of the child nodes fails.
        // Returns RUNNING if any of the child nodes is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinished())
                return NodeState;

            // process nodes
            foreach (Node child in Children)
            {
                switch (child.Update(bb))
                {
                    case NodeState.SUCCESS: continue;
                    case NodeState.FAILURE: return SetNodeState(NodeState.FAILURE);
                    case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                    default: return SetNodeState(NodeState.SUCCESS);
                }
            }
            return SetNodeState(NodeState.SUCCESS);
        }
    }
}

using System.Collections.Generic;

namespace SimpleBehaviorTree
{
    /// <summary>
    /// Selectors are the yin to the sequence's yang. Where a sequence is an AND, requiring all children to succeed to return a success, a selector will return a success if any of its children succeed and not process any further children. It will process the first child, and if it fails will process the second, and if that fails will process the third, until a success is reached, at which point it will instantly return success. It will fail if all children fail. This means a selector is analagous with an OR gate, and as a conditional statement can be used to check multiple conditions to see if any one of them is true.
    /// </summary>
    public class Selector : CompositeNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/LinearScale.png";

        // The constructor requires a list of child nodes to work
        public Selector() { }
        public Selector(List<INode> nodes) : base(nodes) { }

        // Returns SUCCESS immediately once a child node reports succeeded.
        // Reports FAILURE if all of the child nodes fails.
        // Returns RUNNING if any of the child nodes is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinished())
                return NodeState;

            // process nodes
            foreach (INode child in Children)
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

using System;

namespace SimpleBehaviorTree
{
    public class Condition : LeafNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/Question.png";

        // The delegate that is called to evaluate this node 
        protected Func<Blackboard, bool> m_condition;

        // Because this node contains no logic itself, the logic must be passed in in the form of a delegate. 
        // As the signature states, the action needs to return a bool
        public Condition(Func<Blackboard, bool> condition)
        {
            m_condition = condition;
        }

        // Evaluates the node using the passed in delegate and reports the resulting state as appropriate 
        // Returns SUCCESS if the action returns true.
        // Returns FAILURE if the action returns false. 
        // Does not return RUNNING.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinished())
                return NodeState;

            // process node
            if (m_condition(bb))
                return SetNodeState(NodeState.SUCCESS);
            else
                return SetNodeState(NodeState.FAILURE);
        }
    }
}

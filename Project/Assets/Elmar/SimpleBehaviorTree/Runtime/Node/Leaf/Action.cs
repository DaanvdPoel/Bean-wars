using System;

namespace SimpleBehaviorTree
{
    public class Action : LeafNode
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/DownArrow.png";

        // The delegate that is called to evaluate this node 
        protected Func<Blackboard, NodeState> m_action;

        // Because this node contains no logic itself, the logic must be passed in in the form of a delegate. 
        // As the signature states, the action needs to return a NodeStates enum 
        public Action(Func<Blackboard, NodeState> action)
        {
            m_action = action;
        }

        // Evaluates the node using the passed in delegate and reports the resulting state as appropriate 
        // Returns SUCCESS if the action succeeeds.
        // Returns FAILURE if the action fails. 
        // Returns RUNNING if the action is running.
        public override NodeState Update(Blackboard bb)
        {
            // skip finished nodes
            if (NodeFinished())
                return NodeState;

            // process node
            switch (m_action(bb)) 
            {
                case NodeState.SUCCESS: return SetNodeState(NodeState.SUCCESS);
                case NodeState.FAILURE: return SetNodeState(NodeState.FAILURE);
                case NodeState.RUNNING: return SetNodeState(NodeState.RUNNING);
                default: return SetNodeState(NodeState.SUCCESS);
            }
        }
    }
}

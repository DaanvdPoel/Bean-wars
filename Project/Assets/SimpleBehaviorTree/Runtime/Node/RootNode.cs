using System.Collections.Generic;

namespace SimpleBehaviorTree
{
    /// <summary>
    /// The RootNode is the one and only root node in the tree, and should always be the node used to update the tree by calling DoUpdate(), or restart the tree by calling Restart().
    /// </summary>
    public class RootNode : Node
    {
        public override string IconPath { get; } = $"{PACKAGE_ROOT}/DownArrow.png";

        /// <summary>
        /// The root node constructor requires the child node that this root node wraps.
        /// </summary>
        /// <param name="child">The child node of this root node.</param>
        public RootNode() { }
        public RootNode(INode child) : base(new List<INode>() { child }) 
        { 
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Update the tree managed by this root node.
        /// </summary>
        /// <param name="bb">The blackboard with all the info needed to update the behavior tree.</param>
        /// <returns>Returns SUCCESS if the child succeeded. Returns FAILURE if the child failed. Returns RUNNING if the child is running.</returns>
        public override NodeState Update(Blackboard bb)
        {
            return SetNodeState(Children[0].Update(bb));
        }
    }
}
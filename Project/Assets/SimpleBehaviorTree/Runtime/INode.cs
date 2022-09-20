using System.Collections.Generic;
using UnityEngine.Events;

namespace SimpleBehaviorTree
{
    public interface INode 
    {
        /// <summary>
        /// Get the path to the icon for this node. Used by the editor to visualize the behavior tree.
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// Get and set the name of this node.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The current state of this node.
        /// </summary>
        NodeState NodeState { get; }

        /// <summary>
        /// Access to all child nodes below this node.
        /// </summary>
        List<INode> Children { get; }

        /// <summary>
        /// Add a child to this node.
        /// </summary>
        /// <param name="child">The child to be added to this node.</param>
        void AddChild(INode child);

        /// <summary>
        /// Restart this node and all child nodes (recursively). After this the status of this node and all children is Ready.
        /// </summary>
        void RestartTree();

        /// <summary>
        /// Implementing classes use this method to valuate the desired set of conditions 
        /// </summary>
        /// <param name="bb">The blackboard with the info for this update.</param>
        /// <returns>The state of the node after the update is finished.</returns>
        NodeState Update(Blackboard bb);

        /// <summary>
        /// Subscribe to this event to get notified when this node is active. Used by the editor to visualize the behavior tree.
        /// </summary>
        UnityEvent TaskActive { get; }

        /// <summary>
        /// True if this node has been active. Used by the editor to visualize the behavior tree.
        /// </summary>
        bool HasBeenActive { get; }
    }
}

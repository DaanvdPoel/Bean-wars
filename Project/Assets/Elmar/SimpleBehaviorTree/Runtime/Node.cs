using System.Collections.Generic;
using UnityEngine;

namespace SimpleBehaviorTree
{
    [System.Serializable]
    public abstract class Node : NodeBase, INode
    {   
        // node stuff
        private string      m_name      = "";              // the name of the node
        private NodeState   m_nodeState = NodeState.READY; // the state of the node
        private List<INode> m_children  = null;            // the list with child nodes of this node

        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name { get { return m_name; } set { m_name = value; } }

        /// <summary>
        /// The state of the node (ready at startup (after reset), and success or failure or running after the first update).
        /// </summary>
        public NodeState NodeState { get { return m_nodeState; } }

        /// <summary>
        /// 
        /// </summary>
        public List<INode> Children { get { return m_children; } }

        public void AddChild(INode child)
        {
            if (m_children == null)
                m_children = new List<INode>();
            m_children.Add(child);
        }

        /// <summary>
        /// Node constructor. Initial name is retrieved from the object type.
        /// </summary>
        protected Node(List<INode> children) 
        {
            m_name     = this.GetType().Name;
            m_children = children;
        }
        protected Node()
        {
            m_name     = this.GetType().Name;
            m_children = null;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Restart this node and all child nodes (recursively). After this the status of this node and all children is Ready.
        /// </summary>
        public void RestartTree()
        {
            Start();
            if (Children != null)
            {
                foreach (INode child in Children)
                    child.RestartTree();
            }
        }

        /// <summary>
        /// Prepare the node for first usage. For all nodes the node state is set to ready.
        /// </summary>
        public virtual void Start() 
        { 
            m_nodeState = NodeState.READY;
        }

        /// <summary>
        /// Implementing classes use this method to valuate the desired set of conditions 
        /// </summary>
        /// <param name="bb">The blackboard with the info for this update.</param>
        /// <returns>The state of the node after the update is finished.</returns>
        public abstract NodeState Update(Blackboard bb);

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// See if the node is finished with a success or a failure.
        /// </summary>
        /// <returns>True if the node is finished with a failure or success, and false if not.</returns>
        public bool NodeFinished()
        {
            return NodeFinishedSuccess() || NodeFinishedFailure();
        }

        /// <summary>
        /// Convenience method to see if the node is finished with a success.
        /// </summary>
        /// <returns>True if the node is finished with a success, and false if not.</returns>
        public bool NodeFinishedSuccess()
        {
            return m_nodeState == NodeState.SUCCESS;
        }

        /// <summary>
        /// Convenience method to see if the node is finished with a failure.
        /// </summary>
        /// <returns>True if the node is finished with a failure, and false if not.</returns>
        public bool NodeFinishedFailure()
        {
            return m_nodeState == NodeState.FAILURE;
        }

        /// <summary>
        /// Set a new node state and return the provided value again so it can be used immediately to return the value from the calling method.
        /// </summary>
        /// <param name="newNodeState">The requested new node state.</param>
        /// <returns>The new node state.</returns>
        public NodeState SetNodeState(NodeState newNodeState)
        {
            // signal anyone who is listening that this node is active
            base.SetTaskActive();

            // check if state has changed
            if (newNodeState != m_nodeState)
            {
                // do it
                m_nodeState = newNodeState;
            }
            return m_nodeState;
        }
    }
}

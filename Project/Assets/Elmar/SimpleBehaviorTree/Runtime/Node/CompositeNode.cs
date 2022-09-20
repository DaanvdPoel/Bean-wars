using System.Collections.Generic;

namespace SimpleBehaviorTree
{
    /// <summary>
    /// A composite node is a node that can have one or more children. They will process one or more of these children in either a first to last sequence or random order depending on the particular composite node in question, and at some stage will consider their processing complete and pass either success or failure to their parent, often determined by the success or failure of the child nodes. During the time they are processing children, they will continue to return Running to the parent. 
    /// </summary>
    public abstract class CompositeNode : Node
    {
        /// <summary>
        /// A composite constructor requires a list of child nodes to be managed.
        /// </summary>
        /// <param name="children">The children managed by this composite.</param>
        public CompositeNode() { }

        public CompositeNode(List<INode> children) : base(children) 
        { 
        }
    }
}

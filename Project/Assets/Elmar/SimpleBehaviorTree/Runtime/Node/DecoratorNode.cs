using System.Collections.Generic;

namespace SimpleBehaviorTree
{
    /// <summary>
    /// A decorator node, like a composite node, can have a child node. Unlike a composite node, they can specifically only have a single child. Their function is either to transform the result they receive from their child node's status, to terminate the child, or repeat processing of the child, depending on the type of decorator node.
    /// </summary>
    public abstract class DecoratorNode : Node
    {
        /// <summary>
        /// A decorator node constructor requires the child node that this decorator node wraps.
        /// </summary>
        /// <param name="child">The child node of this decorator node.</param>
        public DecoratorNode() { }
        public DecoratorNode(INode child) : base(new List<INode>() { child })
        {
        }
        

        /// <summary>
        /// Get the single child node managed by this decorator.
        /// </summary>
        public INode Child { get { return Children[0]; } }
    }
}
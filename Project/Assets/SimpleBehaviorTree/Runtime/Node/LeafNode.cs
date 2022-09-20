using System;

namespace SimpleBehaviorTree
{
    /// <summary>
    /// Leaf nodes are the lowest level node type, and are incapable of having any children. Leafs are however the most powerful of node types, as these will be defined and implemented by your game to do the game specific or character specific tests or actions required to make your tree actually do useful stuff.
    /// </summary>
    public abstract class LeafNode : Node
    {
        public LeafNode()
        {
        }
    }
}
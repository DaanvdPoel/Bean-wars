using System.Collections.Generic;
using UnityEngine;

namespace SimpleBehaviorTree
{
    public interface IBehaviorTree 
    {
        /// <summary>
        /// The name of this behavior tree.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The rootnode of this behavior tree.
        /// </summary>
        RootNode RootNode { get; }
    }
}
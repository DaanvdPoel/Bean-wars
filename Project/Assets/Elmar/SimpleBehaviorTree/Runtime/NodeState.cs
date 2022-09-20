namespace SimpleBehaviorTree
{
    /// <summary>
    /// Enum containing the possible states a node can return with each update.
    /// </summary>
    public enum NodeState
    {
        READY,
        SUCCESS,
        FAILURE,
        RUNNING,
    }
}

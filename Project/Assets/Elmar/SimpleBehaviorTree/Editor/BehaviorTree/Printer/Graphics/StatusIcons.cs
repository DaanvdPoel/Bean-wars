using System;

namespace SimpleBehaviorTree.Editor
{
    public class StatusIcons
    {
        private const string ICON_STATUS_PATH = "ROOT/Editor/Icons/Status";

        private TextureLoader Ready { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Ready.png");
        private TextureLoader Success { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Success.png");
        private TextureLoader Failure { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Failure.png");
        private TextureLoader Continue { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Continue.png");

        public TextureLoader GetIcon(NodeState status)
        {
            switch (status)
            {
                case NodeState.READY:
                    return Ready;
                case NodeState.SUCCESS:
                    return Success;
                case NodeState.FAILURE:
                    return Failure;
                case NodeState.RUNNING:
                    return Continue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}

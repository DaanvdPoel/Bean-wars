using UnityEngine;

namespace SimpleBehaviorTree.Editor
{
    public class GuiStyleCollection
    {
        public NodeBoxStyle BoxActive   { get; } = new NodeBoxStyle(new Color(1.0f, 0.0f, 0.0f, 1.0f)   , new Color(0.5f, 0.5f, 0.5f, 1.0f));

        public NodeBoxStyle BoxInactive { get; } = new NodeBoxStyle(new Color(0.59f, 0.59f, 0.59f, 1.0f), new Color(0.5f, 0.5f, 0.5f, 1.0f));

        public GUIStyle Title { get; } = new GUIStyle(GUI.skin.label)
        {
            fontSize = 9,
            alignment = TextAnchor.LowerCenter,
            wordWrap = true,
            padding = new RectOffset(3, 3, 3, 3),
        };
    }
}

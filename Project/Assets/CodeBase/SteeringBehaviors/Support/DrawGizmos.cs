using UnityEngine;

public static class DrawGizmos 
{
    //------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------
    static public void DrawRay(Vector3 position, Vector3 direction, Color color)
    {
#if UNITY_EDITOR
        // draw a ray with a solid disc at the end of it
        if (direction.sqrMagnitude > 0.001f)
        {
            Debug.DrawRay(position, direction, color);
            DrawSolidDisc(position + direction, 0.25f, color);
        }
#endif
    }

    static public void DrawLine(Vector3 p1, Vector3 p2, Color color)
    {
#if UNITY_EDITOR
        // draw a ray with a solid disc at the end of it
        if ((p1-p2).sqrMagnitude > 0.001f)
        {
            Debug.DrawLine(p1, p2, color);
        }
#endif
    }

    static public void DrawLabel(Vector3 position, string label, Color color)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;

#if UNITY_EDITOR
        // draw a label at the provided position in the provided color
        if (label.Length > 0)
        {
            UnityEditor.Handles.BeginGUI();
            UnityEditor.Handles.Label(position, label, style);
            UnityEditor.Handles.EndGUI();
        }
#endif
    }

    static public void DrawSolidDisc(Vector3 position, float radius, Color color)
    {
#if UNITY_EDITOR
        // draw a solid disc at the provided position in the provided color
        if (radius > 0.0f)
        {
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawSolidDisc(position, Vector3.up, radius);
        }
#endif
    }

    static public void DrawWireDisc(Vector3 position, float radius, Color color)
    {
#if UNITY_EDITOR
        // draw a wire disc at the provided position in the provided color
        if (radius > 0.0f)
        {
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawWireDisc(position, Vector3.up, radius);
        }
#endif
    }
}

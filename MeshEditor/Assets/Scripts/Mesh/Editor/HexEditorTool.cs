using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("HexEditorTool")]
public class HexEditorTool : EditorTool
{
    public override void OnToolGUI(EditorWindow window)
    {
        // hook mouse input.
        int controlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
        if (Event.current.type == EventType.Layout)
            HandleUtility.AddDefaultControl(controlId);
        if (Event.current.type == EventType.DragPerform)
            HandleUtility.AddDefaultControl(controlId);
        HandleUtility.Repaint();
    }
}

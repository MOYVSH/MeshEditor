using System.Collections;
using System.Collections.Generic;

using System;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.EditorTools;

using HEX;

[EditorTool("HexEditorTool3")]
public class HexEditorTool3 : EditorTool
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
public class HexEditor3 : EditorWindow
{
    [MenuItem("MyTool/HexEditor3")]
    public static void Open()
    {
        var window =GetWindow<HexEditor3>("HexEditor3");
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }
    public enum DrowType
    {
        Range,
        Line,
        FindPath,
    }
    private DrawHex hexDraw
    {
        get
        {
            if (!hexDraw_)
                hexDraw_ = ((GameObject)obj).GetComponent<DrawHex>();
            return hexDraw_;
        }
    }
    private DrawHex hexDraw_;
    private UnityEngine.Object obj;
    public float OffsetHeight;
    public int RingCount;
    public DrowType drowType;
    public bool IsEditor = true;

    HexData hexData;
    private void OnEnable()
    {
        Handles.color = Color.green;
    }
    void OnFocus()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
        Repaint();
    }

    void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public void OnGUI()
    {
        if (UnityEditor.Tools.current != Tool.Custom)
            EditorGUILayout.HelpBox("Press 'G' to edit", MessageType.Warning);

        EditorGUILayout.BeginVertical();
        obj = EditorGUILayout.ObjectField("Object", obj, typeof(UnityEngine.Object));
        OffsetHeight = EditorGUILayout.DelayedFloatField("OffsetHeight", OffsetHeight);
        RingCount = EditorGUILayout.IntField("RingCount", RingCount);
        drowType = (DrowType)EditorGUILayout.EnumPopup("Drow:", drowType);
        IsEditor = EditorGUILayout.Toggle("IsEditor", IsEditor);
        EditorGUILayout.EndVertical();
    }

    private HashSet<HexAxial> cells = new HashSet<HexAxial>();
    private List<HexAxial> cellBuffer = new List<HexAxial>();
    private Vector2? lastMousePos;

    private HexAxial cell_;
    private List<HexAxial> cells_ = new List<HexAxial>();
    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode == KeyCode.G)
            {
                ToolManager.SetActiveTool(typeof(HexEditorTool3));
            }
        }
        if (hexData == null)
            hexData = new HexData(Vector3.zero);

        updateBrushCells();

        if (Event.current.shift || Event.current.control)
            handleEndMode("Hexmap Brush Change", updateCellElevation);
        else
            handleEachMode("Hexmap Brush Change", updateCellElevation);
    }
    protected void handleEndMode(string undoKey, Action<List<HexAxial>> callback)
    {
        var e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button != 0)
                    return;
                cells.Clear();
                foreach (var v in getHitCells(e.mousePosition))
                    cells.Add(v);
                break;
            case EventType.MouseDrag:
                if (e.button != 0)
                    return;
                if (lastMousePos.HasValue)
                {
                    var dist = Vector2.Distance(lastMousePos.Value, e.mousePosition);
                    var lerpCount = dist / 5;
                    for (int i = 0; i < lerpCount; i++)
                    {
                        var pos = Vector2.Lerp(lastMousePos.Value, e.mousePosition, 1f / lerpCount * i);
                        foreach (var v in getHitCells(pos))
                            cells.Add(v);
                    }
                }
                else
                {
                    foreach (var v in getHitCells(e.mousePosition))
                        cells.Add(v);
                }

                lastMousePos = e.mousePosition;
                break;
            case EventType.MouseUp:
            case EventType.MouseLeaveWindow:
                if (e.button != 0)
                    return;
                e.Use();
                callback(cells.ToList());
                cells.Clear();
                lastMousePos = null;
                break;
        }

        foreach (var cell in cells)
        {
            DrawHex(hexDraw.HexDic[cell.Axial2EvenQ()]);
        }
    }
    protected void handleEachMode(string undoKey, Action<List<HexAxial>> callback)
    {
        var e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button != 0)
                    return;
                cells.Clear();
                #region 打印点击的点 已注释
                /*
                foreach (var item in cells_)
                {
                    var a = item.Axial2EvenQ();
                    Debug.LogError(item.q + "," + item.r);

                }
                Debug.LogError("------------------------------------------");
                foreach (var item in cells_)
                {
                    var a = item.Axial2EvenQ();
                    Debug.LogError(a.col + "," + a.row);
                }
                */
                #endregion 
                callback(cells_);
                lastMousePos = e.mousePosition;
                break;
            case EventType.MouseDrag:
                if (e.button != 0)
                    return;
                if (lastMousePos.HasValue)
                {
                    var dist = Vector2.Distance(lastMousePos.Value, e.mousePosition);
                    var lerpCount = dist / 5;
                    for (int i = 0; i < lerpCount; i++)
                    {
                        var pos = Vector2.Lerp(lastMousePos.Value, e.mousePosition, 1f / lerpCount * i);
                        foreach (var v in getHitCells(pos))
                            cells.Add(v);
                    }
                }
                else
                {
                    foreach (var v in getHitCells(e.mousePosition))
                        cells.Add(v);
                }
                callback(cells.ToList());
                e.Use();
                lastMousePos = e.mousePosition;
                break;
            case EventType.MouseUp:
                cells.Clear();
                lastMousePos = null;
                break;
        }
    }
    protected List<HexAxial> getHitCells(Vector2 pos)
    {
        cellBuffer.Clear();
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(pos);
        RaycastHit hit;
        var hitted = Physics.Raycast(mouseRay, out hit);
        if (hitted)
        {
            var cell = hexDraw.HexTemplate.WorldPos2Axial(mouseRay.GetPoint(hit.distance),hexDraw.Offset);
            switch (drowType)
            {
                case DrowType.Range:
                    cellBuffer = hexData.HexAxialRingRange(cell, RingCount);
                    break;
                case DrowType.Line:
                    break;
                case DrowType.FindPath:
                    break;
                default:
                    break;
            }
        }

        return cellBuffer;
    }
    void updateCellElevation(List<HexAxial> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var cell = hexDraw.getCell(list[i]);
            if (cell == null)
            {
                continue;
            }
            if (Event.current.shift)
                cell.height += 1;
            else if (Event.current.control)
                cell.height -= 1;
            else
                cell.height = OffsetHeight;
        }
        list.Clear();
        hexDraw.ReDrawMeshInScent();
    }
    HexData ForDrawTemp = null;
    protected void updateBrushCells()
    {
        Handles.color = Color.green;
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        var hitted = Physics.Raycast(mouseRay, out hit, Mathf.Infinity);
        if (!hitted)
        {
            cells_.Clear();
        }
        else
        {
            if (hexDraw != null)
            {
                //画球
                Handles.SphereHandleCap(0, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity, 0.2f, EventType.Repaint);

                cell_ = hexDraw.HexTemplate.WorldPos2Axial(hit.point, hexDraw.Offset);

                switch (drowType)
                {
                    case DrowType.Range:
                        cells_ = hexData.HexAxialRingRange(cell_, RingCount);
                        break;
                    case DrowType.Line:
                        break;
                    case DrowType.FindPath:
                        break;
                    default:
                        break;
                }
                foreach (var cell in cells_)
                {
                    ForDrawTemp = null;
                    if (hexDraw.HexDic.TryGetValue(cell.Axial2EvenQ(), out ForDrawTemp))
                        DrawHex(ForDrawTemp);
                }
            }
        }
        HandleUtility.Repaint();
    }

    void DrawHex(HexData hex)
    {
        List<Vector3> Points = new List<Vector3>(hex.Points);
        Points.Add(Points[0]);
        Handles.DrawPolyLine(Points.ToArray());
    }
}


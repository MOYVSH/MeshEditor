using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using HEX;
using System;
using System.Linq;

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
public class HexEditor : EditorWindow
{
    [MenuItem("MyTool/HexEditor")]
    public static void Open()
    {
        GetWindow<HexEditor>("HexEditor");
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

    private bool isMoveDown = false;
    private bool isDirty;

    HexData hexData;
    HexAxial _ZeroAxial = new HexAxial(0, 0);

    private void OnEnable()
    {
        Handles.color = Color.green;
        isMoveDown = false;
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
            EditorGUILayout.HelpBox("Add GameObject for edit", MessageType.Warning);
        
        EditorGUILayout.BeginVertical();
        obj = EditorGUILayout.ObjectField("Object", obj, typeof(UnityEngine.Object));
        OffsetHeight = EditorGUILayout.DelayedFloatField("OffsetHeight", OffsetHeight);
        RingCount = EditorGUILayout.IntField("RingCount", RingCount);
        drowType = (DrowType)EditorGUILayout.EnumPopup("Drow:", drowType);
        IsEditor = EditorGUILayout.Toggle("IsEditor", IsEditor);
        EditorGUILayout.EndVertical();
    }
    List<HexAxial> HexList = new List<HexAxial>();
    private void OnSceneGUI(SceneView sceneView)
    {
        // 当前屏幕坐标，左上角是（0，0）右下角（camera.pixelWidth，camera.pixelHeight）
        Handles.color = Color.green;
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            //Debug.LogError("按下");
            isMoveDown = true;
        }
        if (e.type == EventType.MouseUp && e.button == 0)
        {
            Debug.LogError("抬起");
            isMoveDown = false;
        }


        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        var hitted = Physics.Raycast(ray, out hit, Mathf.Infinity);

        if (!hitted)
        {
            HexList.Clear();
        }
        else
        {
            if (hexDraw != null)
            {
                //绘制一个圆点在鼠标位置标记
                Handles.SphereHandleCap(0, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity, 0.2f, EventType.Repaint);
                hexData = null;
                if (hexDraw.HexDic.TryGetValue(hexDraw.HexTemplate.WorldPos2Axial(hit.point, hexDraw.Offset).Axial2EvenQ(), out hexData))
                {
                    switch (drowType)
                    {
                        case DrowType.Range:
                            var centerPoint2 = hexData.Axial;
                            //两种不同的生成方式
                            //HexList = hexData.HexAxialRingRange(RingCount,centerPoint2);
                            HexList = hexData.HexAxialRingRange(centerPoint2, RingCount);
                            break;
                        case DrowType.Line:
                            var startPoint = new HexAxial(0, 0);
                            HexList = hexData.HexLine(startPoint, hexData.Axial);
                            break;
                        case DrowType.FindPath:
                            break;
                        default:
                            break;
                    }
                    foreach (var hexAxial in HexList)
                    {
                        hexData = null;
                        if (hexDraw.HexDic.TryGetValue(hexAxial.Axial2EvenQ(), out hexData))
                        {
                            var evenQ = hexAxial.Axial2EvenQ();
                            DrawHex(hexData);
                            if (IsEditor)
                            {
                                if (e.type == EventType.MouseDown && e.button == 0)
                                {
                                    hexDraw.HexDic[evenQ].height += OffsetHeight;
                                    isDirty = true;
                                }
                                if (isMoveDown && e.isKey && e.keyCode == KeyCode.LeftControl)
                                {
                                    hexDraw.HexDic[evenQ].height = 0;
                                    isDirty = true;
                                }
                                if (isMoveDown && e.isKey && e.keyCode == KeyCode.S)
                                {
                                    hexDraw.HexDic[evenQ].canWalk = false;
                                    isDirty = true;
                                }
                            }
                        }
                    }
                    if (isDirty)
                    {
                        isDirty = !isDirty;
                        hexDraw.RefreshTerrain();
                        hexDraw.ReDrawMeshInScent();
                    }
                    
                }
            }
        }
        sceneView.Repaint();
        HandleUtility.Repaint();
    }

    protected void drawCellInfo(HexAxial pos)
    {
        var cell = hexDraw.getCell(pos);
        if (cell == null)
            return;
        var labelPos =new Vector3(1.5f,0,-1.5f);
        Handles.Label(labelPos, getCellInfoContent(pos), GUI.skin.GetStyle("Tooltip"));
    }
    string getCellInfoContent(HexAxial pos)
    {
        return $"offset:{pos.Axial2EvenQ()}\naxial:{pos}";
    }
    void DrawHex(HexData hex)
    {
        List<Vector3> Points = new List<Vector3>(hex.Points);
        Points.Add(Points[0]);
        Handles.DrawPolyLine(Points.ToArray());
    }
}

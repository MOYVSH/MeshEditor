using HEX;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HexEditorMain : EditorWindow
{
    public interface Editor
    {
        void Init(DrawHex drawHex);
        void Reset();
        void OnEnable();
        void OnDisable();
        void OnGUI();
        void OnGUIBottom();
    }

    [MenuItem("MyTool/HexEditorMain")]
    public static void Open()
    {
        var window = GetWindow<HexEditorMain>("HexEditorMain");
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }

    private Object obj;
    public DrawHex drawHex;
    public GUIContent[] brushGUIContents;
    public HexData hexData;

    HexEditor2 hexEditor2;
    private void OnEnable()
    {
        this.titleContent = new GUIContent("HexgaonMap Editor");

        if (hexEditor2 == null)
            hexEditor2 = new HexEditor2();
        hexEditor2.Init(drawHex);
        hexEditor2.OnEnable();
    }

    private Vector2 m_scrollPosition;
    private void OnGUI()
    {
        obj = EditorGUILayout.ObjectField("Object", obj, typeof(Object));
        if (!obj) return;
        drawHex = (obj as GameObject).GetComponent<DrawHex>();
        if (drawHex!=null)
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
            hexEditor2.OnGUI();
            GUILayout.EndScrollView();
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.EndVertical();
        GUILayout.Space(2);
    }
}

public class HexEditor2 : HexEditorMain.Editor
{
    private DrawHex drawHex;
    public GUIContent[] brushGUIContents;

    public float OffsetHeight;
    public int RingCount;
    public bool IsEditor = true;

    int selectedBrush;
    public void Init(DrawHex drawHex)
    {
        this.drawHex = drawHex;

        brushGUIContents = new GUIContent[5];
        for (int i = 0; i < brushGUIContents.Length; i++)
        {
            brushGUIContents[i] = new GUIContent("123");
        }
            
    }

    public void OnDisable()
    {
    }

    public void OnEnable()
    {
    }

    public void OnGUI()
    {
        selectedBrush = GUILayout.Toolbar(selectedBrush, brushGUIContents);
        OffsetHeight = EditorGUILayout.DelayedFloatField("OffsetHeight", OffsetHeight);
        RingCount = EditorGUILayout.IntField("RingCount", RingCount);
        IsEditor = EditorGUILayout.Toggle("IsEditor", IsEditor);
    }

    public void OnGUIBottom()
    {
    }

    public void Reset()
    {
    }
}

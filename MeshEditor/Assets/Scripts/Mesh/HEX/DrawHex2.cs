using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HEX;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DrawHex2 : MeshDrawBase
{
    public MeshCollider mc;
    public int SidesCountX = 1;
    public int SidesCountY = 1;

    HexMap _Map;

    private List<Vector3> vts = new List<Vector3>();
    private List<Color> colors = new List<Color>();

    [ContextMenu("InitDrawMeshInScent")]
    public void DrawMeshInScent()
    {
        _Map = new HexMap(SidesCountX, SidesCountY);
        CreateTerrain();
        DrawMesh();
    }
    public void ReDrawMeshInScent()
    {
        if (_Map == null)
            _Map = new HexMap(SidesCountX, SidesCountY);
        CreateTerrain();
        DrawMesh();
    }
    protected override void DrawMesh()
    {
        Mesh mesh = targetFilter.sharedMesh;
        if (mesh == null)
            mesh = new Mesh();
        mesh.Clear();
        //顶点
        mesh.SetVertices(vts);
        //三角形
        mesh.SetTriangles(tris,0);
        //UV
        //mesh.uv = uvs.ToArray();
        //颜色
        mesh.SetColors(colors);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        //mesh.RecalculateTangents();//重新计算切线性能影响较大 功能我不清楚

        targetFilter.mesh = mesh;
        mc.sharedMesh = mesh;
    }

    # region //Gizmos GUI
    private void OnDrawGizmos()
    {
        //if (vts.Count == 0) return;
        //Gizmos.color = Color.cyan;
        //for (int i = 0; i < vts.Count; i++)
        //{
        //    Vector3 worldHitPoint = transform.TransformPoint(vts[i]);
        //    Gizmos.DrawWireSphere(worldHitPoint, .1f);
        //}
    }
    private void OnGUI()
    {
        //if (vts.Count == 0) return;
        //Gizmos.color = Color.red;
        //for (int i = 0; i < vts.Count; i++)
        //{
        //    Vector3 worldPoint = transform.TransformPoint(vts[i]);
        //    Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        //    Vector3 uiPoint = new Vector3(screenPoint.x, Camera.main.pixelHeight - screenPoint.y, screenPoint.z);
        //    GUI.Label(new Rect(uiPoint, new Vector2(100, 80)), i.ToString());
        //}
    }
    #endregion

    protected override void CreateTerrain()
    {
        vts.Clear();
        tris.Clear();
        uvs.Clear();
        colors.Clear();
        normals = null;

        refreshTerrain(0, 0, SidesCountX, SidesCountY);
    }

    void refreshTerrain(int cellXFrom, int cellYFrom, int cellXTo, int cellYTo)
    {
        for (int cy = cellYFrom; cy < cellYTo; cy++)
        {
            for (int cx = cellXFrom; cx < cellXTo; cx++)
            {
                triangulateTerrain(new HexEvenQ(cx,-cy));
            }
        }
    }
    void triangulateTerrain(HexEvenQ c)
    {
        CreateHex(c);
        for (int dir = 2; dir <= 4; dir++)
        {
            // 2 | 3 | 4
            CreateBrage(c, dir);
        }
        for (int dir = 3; dir <= 5; dir++)
        {
            // 2，3 | 3，4 | 4，5
            CreateTriangle(c, dir);
        }
    }

    void CreateHex(HexEvenQ c)
    {
        int cid = _Map.getCellArrayId(c);
        HexData hexData = _Map.getCell(cid);

        int hexIndex = vts.Count;

        vts.AddRange(hexData.Points);
        for (int i = 0; i < 6; i++)
        {
            colors.Add(Color.gray);
        }
        PolygonTriangles(hexIndex, 6);
    }
    void CreateBrage(HexEvenQ c0, int dir)
    {
        HexEvenQ c1 = c0.GetNeighbor(dir);
        if (!_Map.isValidCell(c1))
            return;

        var cid0 = _Map.getCellArrayId(c0);
        var cid1 = _Map.getCellArrayId(c1);
        var cell0 = _Map.getCell(cid0);
        var cell1 = _Map.getCell(cid1);

        int BrageIndex = vts.Count;

        vts.Add(cell0.Points[dir]);
        vts.Add(cell1.Points[(dir + 3 + 1) % 6]);
        vts.Add(cell1.Points[(dir + 3) % 6]);
        vts.Add(cell0.Points[dir + 1]);
        for (int i = 0; i < 4; i++)
        {
            colors.Add(Color.white);
        }
        PolygonTriangles(BrageIndex, 4);
    }
    void CreateTriangle(HexEvenQ c0, int dir)
    {
        HexEvenQ c1 = c0.GetNeighbor(dir - 1);
        if (!_Map.isValidCell(c1))
            return;
        HexEvenQ c2 = c0.GetNeighbor(dir);
        if (!_Map.isValidCell(c2))
            return;

        var cid0 = _Map.getCellArrayId(c0);
        var cid1 = _Map.getCellArrayId(c1);
        var cid2 = _Map.getCellArrayId(c2);
        var cell0 = _Map.getCell(cid0);
        var cell1 = _Map.getCell(cid1);
        var cell2 = _Map.getCell(cid2);

        int TriangleIndex = vts.Count;

        vts.Add(cell0.Points[dir]);
        vts.Add(cell1.Points[(dir + 2) % 6]);
        vts.Add(cell2.Points[(dir + 2 + 2) % 6]);
        for (int i = 0; i < 3; i++)
        {
            colors.Add(Color.white);
        }
        PolygonTriangles(TriangleIndex, 3);
    }
    void PolygonTriangles(int PolygonIndex, int PolygonSideCount)
    {
        //逆时针添加点需要顺时针读取 反之
        int trisLength = PolygonSideCount - 2;//三角形个数 变数-2
        for (int i = 0; i < trisLength; i++)
        {
            tris.Add(PolygonIndex);
            tris.Add(PolygonIndex + 2 + i);
            tris.Add(PolygonIndex + 1 + i);
        }
    }

    public HexMap getMap()
    {
        return _Map;
    }
}

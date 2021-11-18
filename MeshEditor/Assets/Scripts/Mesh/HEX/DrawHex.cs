using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HEX;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DrawHex : MeshDrawBase
{
    public MeshCollider mc;
    public int SidesCountX = 1;
    public int SidesCountY = 1;
    public float Offset = 0;
    private int hexIndex = 0;

    public HexData HexTemplate => _hexTemplate;
    private HexData _hexTemplate = new HexData(Vector3.zero, new HexEvenQ(0, 0));


    public Dictionary<HexEvenQ, HexData> HexDic => _hexDic;
    private Dictionary<HexEvenQ, HexData> _hexDic = new Dictionary<HexEvenQ, HexData>();
    private List<Vector3> vts = new List<Vector3>();
    private List<Color> colors = new List<Color>();

    [ContextMenu("InitDrawMeshInScent")]
    public void DrawMeshInScent()
    {
        CreateTerrain();
        DrawMesh();
    }
    public void ReDrawMeshInScent()
    {
        RefreshTerrain();
        DrawMesh();
    }
    protected override void OnDestroy()
    {
    }
    protected override void DrawMesh()
    {
        mesh = targetFilter.sharedMesh;
        if (mesh == null)
            mesh = new Mesh();

        //顶点  通过函数形式进行复制性能会更差一点
        mesh.vertices = vts.ToArray();
        //mesh.SetVertices(vts);
        //三角形
        mesh.triangles = tris.ToArray();
        //mesh.SetTriangles(tris,0);
        //UV
        //mesh.uv = uvs.ToArray();
        //颜色
        mesh.colors = colors.ToArray();

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
        _hexDic.Clear();
        vts.Clear();
        tris.Clear();
        uvs.Clear();
        colors.Clear();
        normals = null;

        CreateHex();
        CreateBrage();
        CreateTriangle();
    }
    public void RefreshTerrain()
    {
        vts.Clear();
        tris.Clear();
        uvs.Clear();
        colors.Clear();
        normals = null;

        CreateHex();
        CreateBrage();
        CreateTriangle();
    }
    void CreateHex()
    {
        for (int row = 0; row < SidesCountY; row++)
        {
            for (int col = 0; col < SidesCountX; col++)
            {
                this.hexIndex = vts.Count;
                var evenQ = new HexEvenQ(col, -row);
                #region 带偏移 带高度 从六边形左边的一个边开始绘制
                Vector3 startPos;
                startPos = HexTemplate.EvenQ2WorldSpace(evenQ, Offset) + Vector3.left;
                HexData hexData;
                if (_hexDic.TryGetValue(evenQ, out hexData) )
                {
                    hexData.RefreshPoints();
                }
                else
                {
                    hexData = new HexData(startPos, evenQ);
                    _hexDic.Add(evenQ, hexData);
                }

                vts.AddRange(hexData.Points);
                for (int i = 0; i < 6; i++)
                {
                    colors.Add(Color.gray);
                }
                PolygonTriangles(hexIndex, 6);
                #endregion
            }
        }
        
    }
    void CreateBrage()
    {
        foreach (var hexDataEle in _hexDic)
        {
            var hexData = hexDataEle.Value;
            for (int dir = 2; dir <= 4; dir++)
            {
                int BrageIndex = vts.Count;
                var nighexEvenQ = hexData.EvenQ.GetNeighbor(dir);
                HexData hexDataNig = null;
                _hexDic.TryGetValue(nighexEvenQ, out hexDataNig);
                #region //打印边 2，3，4 所对应的邻居测试用
                //if (hexDataNig != null)
                //{
                //    Debug.LogError(dir +" : "+ hexDataNig.EvenQ.col + "," + hexDataNig.EvenQ.row);
                //}
                //else
                //{
                //    Debug.LogError(dir);
                //}
                #endregion
                if (hexDataNig != null)
                {
                    vts.Add(hexData.Points[dir]);
                    vts.Add(hexDataNig.Points[(dir + 3 + 1) % 6]);
                    vts.Add(hexDataNig.Points[(dir + 3) % 6]);
                    vts.Add(hexData.Points[dir + 1]);

                    for (int i = 0; i < 4; i++)
                    {
                        colors.Add(Color.white);
                    }

                    PolygonTriangles(BrageIndex, 4);
                }
            }
        }
    }
    void CreateTriangle()
    {
        foreach (var hexDataEle in _hexDic)
        {
            var hexData = hexDataEle.Value;
            for (int dir = 3; dir <= 5; dir++)
            {
                // 2，3 | 3，4 | 4，5
                int BrageIndex = vts.Count;
                var nighexEvenQ1 = hexData.EvenQ.GetNeighbor(dir-1);
                var nighexEvenQ2 = hexData.EvenQ.GetNeighbor(dir);
                HexData hexDataNig1 = null;
                HexData hexDataNig2 = null;
                _hexDic.TryGetValue(nighexEvenQ1, out hexDataNig1);
                _hexDic.TryGetValue(nighexEvenQ2, out hexDataNig2);

                if (hexDataNig1 != null && hexDataNig2 != null)
                {
                    vts.Add(hexData.Points[dir]);
                    vts.Add(hexDataNig1.Points[(dir + 2) % 6]);
                    vts.Add(hexDataNig2.Points[(dir + 2 + 2) % 6]);

                    for (int i = 0; i < 3; i++)
                    {
                        colors.Add(Color.white);
                    }

                    PolygonTriangles(BrageIndex, 3);

                }
            }
        }
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

    public HexData getCell(HexAxial axial)
    {
        HexData temp = null;
        HexDic.TryGetValue(axial.Axial2EvenQ(), out temp);
        return temp;
    }
}

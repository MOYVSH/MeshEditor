using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TriangleDraw : MeshDrawBase
{
    public List<Vector3> vts = new List<Vector3>();
    private void Start()
    {
        mesh = new Mesh();
        //获取顶点

        //for (int i = 0; i < vts.Count; i++) //有点问题
        //{
        //    vts[i] = transform.InverseTransformPoint(vts[i]);
        //}
        mesh.vertices = vts.ToArray();
        //三角形
        //Mesh默认只渲染一面 所以一般默认按照顺时针画
        //顺时针  
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;
        //逆时针
        //tris[0] = 2;
        //tris[1] = 1;
        //tris[2] = 0;

        mesh.triangles = tris.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        targetFilter.mesh = mesh;
    }
    protected override void DrawMesh()
    {
        
    }

    protected override void CreateTerrain()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class QuadDraw : MeshDrawBase
{
    public List<Vector3> vts = new List<Vector3>();

    private void Start()
    {
        mesh = new Mesh();
        //获取顶点
        mesh.vertices = vts.ToArray();
        //三角形1
        tris[0] = 0;
        tris[1] = tris[4] = 1;
        tris[2] = tris[3] = 3;
        tris[5] = 2;

        mesh.triangles = tris.ToArray();

        //UV
        uvs = new List<Vector2>(vts.Count);
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(1, 0);

        mesh.uv = uvs.ToArray();

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

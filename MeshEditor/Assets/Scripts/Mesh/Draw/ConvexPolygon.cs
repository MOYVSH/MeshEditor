using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ConvexPolygon : MeshDrawBase
{
    public List<Vector3> vts = new List<Vector3>();
    protected override void DrawMesh()
    {
        if (Input.GetKeyDown(KeyCode.S))
            DrawPolygon();
    }
    protected override void CreateTerrain()
    {

    }
    private void DrawPolygon()
    {
        mesh = new Mesh();

        //顶点
        mesh.vertices = vts.ToArray();
        //三角形
        int trisLength = (vts.Count - 2) * 3;
        for (int i = 0, n = 1; i < trisLength; i += 3, n++)
        {
            tris.Add(n + 1);
            tris.Add(n);
            tris.Add(0);
        }
        mesh.triangles = tris.ToArray();

        //法线
        normals = new Vector3[vts.Count];
        for (int i = 0; i < vts.Count; i++)
        {
            normals[i] = new Vector3(-1, 0, 0);
        }
        mesh.normals = normals;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        targetFilter.mesh = mesh;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit,Mathf.Infinity))
            {
                Vector3 worldHitPoint = hit.point;
                Vector3 localHitoint = transform.InverseTransformPoint(worldHitPoint);
                vts.Add(localHitoint);
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
            Reset();
        if (Input.GetKeyDown(KeyCode.S))
            DrawPolygon();
    }

    private void Reset()
    {
        vts.Clear();

        targetFilter.mesh = null;
        Destroy(mesh);
    }
    private void OnGUI()
    {
        if (vts.Count == 0) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < vts.Count; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(vts[i]);
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
            Vector3 uiPoint = new Vector3(screenPoint.x, Camera.main.pixelHeight - screenPoint.y, screenPoint.z);
            GUI.Label(new Rect(uiPoint, new Vector2(100, 80)), i.ToString());
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < vts.Count; i++)
        {
            Vector3 worldHitPoint = transform.TransformPoint(vts[i]);
            Gizmos.DrawWireSphere(worldHitPoint, 0.15f);
        }
    }


}

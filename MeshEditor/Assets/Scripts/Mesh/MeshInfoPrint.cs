using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInfoPrint : MeshDrawBase
{
    protected override void CreateTerrain()
    {

    }

    protected override void DrawMesh()
    {

    }

    private void OnDrawGizmos()
    {
        targetFilter = GetComponent<MeshFilter>();
        mesh = targetFilter.mesh;
        Gizmos.color = Color.red;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(mesh.vertices[i]);
            Gizmos.DrawSphere(worldPoint,.1f);
        }
    }
}

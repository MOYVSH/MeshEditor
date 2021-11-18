using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshDrawBase : MonoBehaviour
{
    public MeshFilter targetFilter;
    protected Mesh mesh;
    protected List<int> tris = new List<int>();
    protected List<Vector2> uvs = new List<Vector2>();
    protected Vector3[] normals;

    private void Awake()
    {
        targetFilter = GetComponent<MeshFilter>();
    }
    protected virtual void Start()
    {
        CreateTerrain();
        DrawMesh();
    }
    protected virtual void Update()
    {
        
    }

    protected virtual void OnDestroy()
    {
        
    }

    protected abstract void DrawMesh();

    protected abstract void CreateTerrain();
}

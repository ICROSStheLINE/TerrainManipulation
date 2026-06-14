using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(MeshCollider))]
public class Ball : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    // MeshCollider meshCollider;

    [SerializeField] Material mat;
    [SerializeField] float radius = 1f;

    [HideInInspector] public Mesh mesh;
    [HideInInspector] public List<Vector3> vertices;
    [HideInInspector] public List<int> triangles;
    public List<Vector2> UVs;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;
        // meshCollider = GetComponent<MeshCollider>();

        vertices = new List<Vector3>();
        triangles = new List<int>();
        UVs = new List<Vector2>();
        mesh = new Mesh();

        ClearData();
        GenerateMesh();
        UploadMesh();
    }

    public void ClearData()
    {
        vertices.Clear();
        triangles.Clear();
        UVs.Clear();
        mesh.Clear();
    }

    public void GenerateMesh()
    {
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertices.AddRange(new Vector3[]
        {
            new Vector3(-1,  t,  0),
            new Vector3( 1,  t,  0),
            new Vector3(-1, -t,  0),
            new Vector3( 1, -t,  0),

            new Vector3( 0, -1,  t),
            new Vector3( 0,  1,  t),
            new Vector3( 0, -1, -t),
            new Vector3( 0,  1, -t),

            new Vector3( t,  0, -1),
            new Vector3( t,  0,  1),
            new Vector3(-t,  0, -1),
            new Vector3(-t,  0,  1)
        });

        // Project vertices onto sphere
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i].normalized * radius;
        }

        triangles.AddRange(new int[]
        {
            0,11,5,
            0,5,1,
            0,1,7,
            0,7,10,
            0,10,11,

            1,5,9,
            5,11,4,
            11,10,2,
            10,7,6,
            7,1,8,

            3,9,4,
            3,4,2,
            3,2,6,
            3,6,8,
            3,8,9,

            4,9,5,
            2,4,11,
            6,2,10,
            8,6,7,
            9,8,1
        });
    }

    public void UploadMesh()
    {
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        // meshCollider.sharedMesh = mesh;
    }
}
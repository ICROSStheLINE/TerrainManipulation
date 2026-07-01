using System.Collections;
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
        meshRenderer.sharedMaterial = GetComponent<Renderer>().material;
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
        for (int face = 0; face < 20; face++)
        {
            int startIndex = vertices.Count;

            for (int i = 0; i < 3; i++)
            {
                vertices.Add(
                    voxelVertices[
                        voxelVertexIndex[face, i]
                    ].normalized * radius
                );

                UVs.Add(voxelUVs[i]);
            }

            triangles.Add(startIndex + voxelTris[0]);
            triangles.Add(startIndex + voxelTris[1]);
            triangles.Add(startIndex + voxelTris[2]);
        }
    }

    public void UploadMesh()
    {
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0, false);
        // mesh.SetUVs(0, UVs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        mesh.UploadMeshData(false);

        meshFilter.mesh = mesh;
        // meshCollider.sharedMesh = mesh;
    }

    #region Static Variables

    static readonly float t = (1f + 1.61803398875f) / 2f;

    // 12 vertices of an icosahedron
    static readonly Vector3[] voxelVertices = new Vector3[12]
    {
        new Vector3(-1,  t,  0), // 0
        new Vector3( 1,  t,  0), // 1
        new Vector3(-1, -t,  0), // 2
        new Vector3( 1, -t,  0), // 3

        new Vector3( 0, -1,  t), // 4
        new Vector3( 0,  1,  t), // 5
        new Vector3( 0, -1, -t), // 6
        new Vector3( 0,  1, -t), // 7

        new Vector3( t,  0, -1), // 8
        new Vector3( t,  0,  1), // 9
        new Vector3(-t,  0, -1), // 10
        new Vector3(-t,  0,  1)  // 11
    };

    // 20 triangular faces
    static readonly int[,] voxelVertexIndex = new int[20, 3]
    {
        {0,11,5},
        {0,5,1},
        {0,1,7},
        {0,7,10},
        {0,10,11},

        {1,5,9},
        {5,11,4},
        {11,10,2},
        {10,7,6},
        {7,1,8},

        {3,9,4},
        {3,4,2},
        {3,2,6},
        {3,6,8},
        {3,8,9},

        {4,9,5},
        {2,4,11},
        {6,2,10},
        {8,6,7},
        {9,8,1}
    };

    // Every face is already a triangle
    static readonly int[] voxelTris = new int[3]
    {
        0, 1, 2
    };

    static readonly Vector2[] voxelUVs = new Vector2[3]
    {
        new Vector2(0f, 0f),
        new Vector2(0.5f, 1f),
        new Vector2(1f, 0f)
    };

    #endregion
}
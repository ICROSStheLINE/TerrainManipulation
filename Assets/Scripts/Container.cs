using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Container : MonoBehaviour
{
  MeshFilter meshFilter;
  MeshRenderer meshRenderer;
  MeshCollider meshCollider;

  [SerializeField] Material mat;
  [HideInInspector] public Mesh mesh;
  [HideInInspector] public List<Vector3> vertices;
  [HideInInspector] public List<int> triangles;
  public List<Vector2> UVs;

  static int chunkLength = 6; // x  (I had to make these static so I could use them to make the cube map)
  static int chunkHeight = 6; // y
  static int chunkWidth = 6; // z
  
  // 3D array docs: https://www.w3schools.com/cs/cs_arrays_multi.php
  bool[,,] cubeMap = new bool[chunkLength,chunkHeight,chunkWidth];

  void Start()
  {
    meshFilter = GetComponent<MeshFilter>();
    meshRenderer = GetComponent<MeshRenderer>();
    meshRenderer.sharedMaterial = mat;
    meshCollider = GetComponent<MeshCollider>();

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
    // Cube Map
    for (int x = 0; x < chunkLength; x++)
    {
      for (int y = 0; y < chunkHeight; y++)
      {
        for (int z = 0; z < chunkWidth; z++)
        {
          cubeMap[x,y,z] = true;
        }
      }
    }

    // Vertices
    for (int x = 0; x < chunkLength; x++)
    {
      for (int y = 0; y < chunkHeight; y++)
      {
        for (int z = 0; z < chunkWidth; z++)
        {
          for (int vertexIndex = 0; vertexIndex < 8; vertexIndex++)
          {
            // Implement that check I made before to cull out useless vertices
            vertices.Add(voxelVertices[vertexIndex] + new Vector3(x,y,z));
          }
        }
      }
    }

    // Face Triangles
    // I need to be able to identify which direction each face is, and which face each vertex is associated with.
    for (int blockIndex = 0; blockIndex < chunkLength * chunkHeight * chunkWidth; blockIndex++)
    {
      for (int faceIndex = 0; faceIndex < 6; faceIndex++)
      {
        // Do a facecheck to see if there is a cube in the direction of this face before drawing
        for (int vertexIndex = 0; vertexIndex < 6; vertexIndex++)
        {
          triangles.Add(voxelVertexIndex[faceIndex, voxelTris[faceIndex,vertexIndex]] + blockIndex * 8);
        }
      }
    }
  }

  public void UploadMesh()
  {
    mesh.SetVertices(vertices);
    mesh.SetTriangles(triangles, 0, false);

    mesh.RecalculateNormals();

    mesh.RecalculateBounds();

    mesh.UploadMeshData(false);
    meshFilter.mesh = mesh;
    meshCollider.sharedMesh = mesh;
  }

  #region Static Variables

  // The 8 corner positions of a unit cube (from (0,0,0) to (1,1,1)).
  static readonly Vector3[] voxelVertices = new Vector3[8]
  {
    new Vector3(0,0,0),//0
    new Vector3(1,0,0),//1
    new Vector3(0,1,0),//2
    new Vector3(1,1,0),//3
    new Vector3(0,0,1),//4
    new Vector3(1,0,1),//5
    new Vector3(0,1,1),//6
    new Vector3(1,1,1),//7
  };

  // For each of the 6 faces, this gives the indices into voxelVertices
  // that make up that face's 4 corner vertices.
  static readonly int[,] voxelVertexIndex = new int[6, 4]
  {
    {0,1,2,3},
    {4,5,6,7},
    {4,0,6,2},
    {5,1,7,3},
    {0,1,4,5},
    {2,3,6,7},
  };

  // For each of the 6 faces, this defines which of the 4 face vertices
  // are used to build the 2 triangles (6 indices) that make up that face.
  static readonly int[,] voxelTris = new int[6, 6]
  {
    {0,2,3,0,3,1},
    {0,1,2,1,3,2},
    {0,2,3,0,3,1},
    {0,1,2,1,3,2},
    {0,1,2,1,3,2},
    {0,2,3,0,3,1},
  };

  // Default UV coordinates for a single quad (face) ranging from (0,0) to (1,1).
  static readonly Vector2[] voxelUVs = new Vector2[4]
  {
    new Vector2(0,0),
    new Vector2(0,1),
    new Vector2(1,0),
    new Vector2(1,1)
  };
  #endregion
}

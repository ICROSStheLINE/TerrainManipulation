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

  static int chunkLength = 15; // x  (I had to make these static so I could use them to make the cube map)
  static int chunkHeight = 15; // y
  static int chunkWidth = 15; // z
  
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

    FillCubeMap();

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

  void FillCubeMap()
  {
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
  }

  bool CheckForNeighbouringCube(int x, int y, int z, int faceIndex) // Returns true if neighbour exists
  {
    if (faceIndex == 0) // Vector3.backward
    {
      if (Mathf.Sign(z - 1) == -1)
        return false;
      return cubeMap[x,y,z - 1];
    }
    if (faceIndex == 1) // Vector3.forward
    {
      if (z + 1 >= chunkWidth)
        return false;
      return cubeMap[x,y,z + 1];
    }
    if (faceIndex == 2) // Vector3.left
    {
      if (Mathf.Sign(x - 1) == -1)
        return false;
      return cubeMap[x - 1,y,z];
    }
    if (faceIndex == 3) // Vector3.right
    {
      if (x + 1 >= chunkLength)
        return false;
      return cubeMap[x + 1,y,z];
    }
    if (faceIndex == 4) // Vector3.down
    {
      if (Mathf.Sign(y - 1) == -1)
        return false;
      return cubeMap[x,y - 1,z];
    }
    if (faceIndex == 5) // Vector3.up
    {
      if (y + 1 >= chunkHeight)
        return false;
      return cubeMap[x,y + 1,z];
    }
    return false;
  }

  public void GenerateMesh()
  {
    for (int x = 0; x < chunkLength; x++)
    {
      for (int y = 0; y < chunkHeight; y++)
      {
        for (int z = 0; z < chunkWidth; z++)
        {
          for (int faceIndex = 0; faceIndex < 6; faceIndex++)
          {
            if (!CheckForNeighbouringCube(x, y, z, faceIndex))
            {
              int[] lastFourVertexIndicesAdded = new int[4];
              for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
              {
                Vector3 vertexPosition = voxelVertices[voxelVertexIndex[faceIndex, vertexIndex]] + new Vector3(x,y,z);
                vertices.Add(vertexPosition);
                lastFourVertexIndicesAdded[vertexIndex] = vertices.Count - 1;
              }
              for (int vertexIndex = 0; vertexIndex < 6; vertexIndex++)
              {
                triangles.Add(lastFourVertexIndicesAdded[voxelTris[faceIndex,vertexIndex]]);
              }
            }
          }
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
    {0,1,2,3}, // Vector3.backward
    {4,5,6,7}, // Vector3.forward
    {4,0,6,2}, // Vector3.left
    {5,1,7,3}, // Vector3.right
    {0,1,4,5}, // Vector3.down
    {2,3,6,7}, // Vector3.up
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

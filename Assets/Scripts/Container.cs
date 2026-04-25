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
    int chunkWidth = 6;
    int chunkHeight = 6;
    int chunkLength = 6;

    for (int chunkLength_ = 0; chunkLength_ < chunkLength; chunkLength_++)
    {
      for (int chunkWidth_ = 0; chunkWidth_ < chunkWidth; chunkWidth_++)
      {
        for (int chunkHeight_ = 0; chunkHeight_ < chunkHeight; chunkHeight_++)
        {
          for (int vertexIndex = 0; vertexIndex < 8; vertexIndex++)
          {
            vertices.Add(voxelVertices[vertexIndex] + new Vector3(chunkWidth_,chunkHeight_,chunkLength_));
          }
        }
      }
    }
    

    for (int blockIndex = 0; blockIndex < chunkWidth * chunkHeight * chunkLength; blockIndex++)
    {
      for (int faceIndex = 0; faceIndex < 6; faceIndex++)
      {
        for (int vertexIndex = 0; vertexIndex < 6; vertexIndex++)
        {
          // UVs.Add(voxelUVs[voxelTris[faceIndex,vertexIndex]] + new Vector2(0,cubeIndex));
          triangles.Add(voxelVertexIndex[faceIndex, voxelTris[faceIndex,vertexIndex]] + blockIndex * 8);
        }
      }
    }

    // // Front
    // vertices.Add(voxelVertices[0]);
    // vertices.Add(voxelVertices[2]);
    // vertices.Add(voxelVertices[3]);
    // UVs.Add(new Vector2(0, 0));
    // UVs.Add(new Vector2(0, 1));
    // UVs.Add(new Vector2(1, 0));
    // triangles.Add(0);
    // triangles.Add(1);
    // triangles.Add(2);
    // vertices.Add(voxelVertices[0]);
    // vertices.Add(voxelVertices[3]);
    // vertices.Add(voxelVertices[1]);
    // UVs.Add(new Vector2(0, 1));
    // UVs.Add(new Vector2(1, 1));
    // UVs.Add(new Vector2(1, 0));
    // triangles.Add(3);
    // triangles.Add(4);
    // triangles.Add(5);
    // // Up
    // vertices.Add(new Vector3(0, 1, 0));
    // vertices.Add(new Vector3(0, 1, 1));
    // vertices.Add(new Vector3(1, 1, 1));
    // UVs.Add(new Vector2(0, 0));
    // UVs.Add(new Vector2(0, 1));
    // UVs.Add(new Vector2(1, 1));
    // triangles.Add(6);
    // triangles.Add(7);
    // triangles.Add(8);
    // vertices.Add(new Vector3(1, 1, 1));
    // vertices.Add(new Vector3(1, 1, 0));
    // vertices.Add(new Vector3(0, 1, 0));
    // UVs.Add(new Vector2(1, 1));
    // UVs.Add(new Vector2(1, 0));
    // UVs.Add(new Vector2(0, 0));
    // triangles.Add(9);
    // triangles.Add(10);
    // triangles.Add(11);
    // // Back
    // vertices.Add(new Vector3(1, 0, 1));
    // vertices.Add(new Vector3(1, 1, 1));
    // vertices.Add(new Vector3(0, 0, 1));
    // UVs.Add(new Vector2(0, 0));
    // UVs.Add(new Vector2(0, 1));
    // UVs.Add(new Vector2(1, 0));
    // triangles.Add(12);
    // triangles.Add(13);
    // triangles.Add(14);
    // vertices.Add(new Vector3(0, 0, 1));
    // vertices.Add(new Vector3(1, 1, 1));
    // vertices.Add(new Vector3(0, 1, 1));
    // UVs.Add(new Vector2(1, 0));
    // UVs.Add(new Vector2(0, 1));
    // UVs.Add(new Vector2(1, 1));
    // triangles.Add(15);
    // triangles.Add(16);
    // triangles.Add(17);
    // // Down
    // vertices.Add(new Vector3(0, 0, 0));
    // vertices.Add(new Vector3(1, 0, 0));
    // vertices.Add(new Vector3(1, 0, 1));
    // UVs.Add(new Vector2(0, 0));
    // UVs.Add(new Vector2(1, 0));
    // UVs.Add(new Vector2(1, 1));
    // triangles.Add(18);
    // triangles.Add(19);
    // triangles.Add(20);
    // vertices.Add(new Vector3(0, 0, 0));
    // vertices.Add(new Vector3(1, 0, 1));
    // vertices.Add(new Vector3(0, 0, 1));
    // UVs.Add(new Vector2(0, 0));
    // UVs.Add(new Vector2(1, 1));
    // UVs.Add(new Vector2(0, 1));
    // triangles.Add(21);
    // triangles.Add(22);
    // triangles.Add(23);
    // // Left
    // vertices.Add(new Vector3(0, 0, 1));
    // vertices.Add(new Vector3(0, 1, 1));
    // vertices.Add(new Vector3(0, 0, 0));
    // UVs.Add(new Vector2(0, 0));
    // UVs.Add(new Vector2(0, 1));
    // UVs.Add(new Vector2(1, 0));
    // triangles.Add(24);
    // triangles.Add(25);
    // triangles.Add(26);
    // vertices.Add(new Vector3(0, 0, 0));
    // vertices.Add(new Vector3(0, 1, 1));
    // vertices.Add(new Vector3(0, 1, 0));
    // UVs.Add(new Vector2(1, 0));
    // UVs.Add(new Vector2(0, 1));
    // UVs.Add(new Vector2(1, 1));
    // triangles.Add(27);
    // triangles.Add(28);
    // triangles.Add(29);
    // // Right
    // vertices.Add(new Vector3(1, 0, 0));
    // vertices.Add(new Vector3(1, 1, 0));
    // vertices.Add(new Vector3(1, 1, 1));
    // UVs.Add(new Vector2(0, 0));
    // UVs.Add(new Vector2(0, 1));
    // UVs.Add(new Vector2(1, 1));
    // triangles.Add(30);
    // triangles.Add(31);
    // triangles.Add(32);
    // vertices.Add(new Vector3(1, 0, 0));
    // vertices.Add(new Vector3(1, 1, 1));
    // vertices.Add(new Vector3(1, 0, 1));
    // UVs.Add(new Vector2(0, 0));
    // UVs.Add(new Vector2(1, 1));
    // UVs.Add(new Vector2(1, 0));
    // triangles.Add(33);
    // triangles.Add(34);
    // triangles.Add(35);

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

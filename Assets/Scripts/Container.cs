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

  static int chunkLength = 2; // x  (I had to make these static so I could use them to make the cube map)
  static int chunkHeight = 2; // y
  static int chunkWidth = 2; // z
  
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


    
    for (int x = 0; x < chunkLength; x++)
    {
      for (int y = 0; y < chunkHeight; y++)
      {
        for (int z = 0; z < chunkWidth; z++)
        {
          for (int faceIndex = 0; faceIndex < 6; faceIndex++)
          {
            int[] lastFourVertexIndicesAdded = new int[4];
            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
            {
              vertices.Add(voxelVertices[voxelVertexIndex[faceIndex, vertexIndex]] + new Vector3(x,y,z));
              lastFourVertexIndicesAdded[vertexIndex] = vertices.Count - 1;
            }
            for (int vertexIndex = 0; vertexIndex < 6; vertexIndex++)
            {
              triangles.Add(lastFourVertexIndicesAdded[voxelTris[faceIndex,vertexIndex]]);
            }
            // EXPLANATION:
            // Before:
            // We were making exactly 8 vertices for each cube. No overlapping vertices... EFFICIENT!
            // We were carefully organizing triangles around each of the 8 vertices to draw faces...
            // And whenever we wanted to draw triangles for the next cube in the list, we used a "cubeIndex" variable to count what cube we were on...
            // then multiplied that count by 8 (since it was assumed that each cube has 8 vertices and the next set of vertices we needed to draw...
            // triangels between was also a set of 8 vertices)...
            // THIS WAS A PROBLEM because if we wanted to move to the next step where we were deciding which vertices to remove and which to keep,
            // we would no longer be able to reliably tell how much we had to multiply blockIndex by (since we no longer knew how many vertices were
            // made in the last set AND we didn't know what order they were in if some vertices were removed!).
            // SOLUTION:
            // Once I learned that this was the problem, i changed it to NO LONGER ADD EXACTLY 8 VERTICES FOR EACH CUBE...
            // and to instead add 4 VERTICES FOR EACH FACE... INEFFICIENT!!!
            // This, however, solves the problem where we don't know what order the list of vertices is in anymore.
            // All we need to do now is draw triangles between the LAST 4 CREATED VERTICES instead of picking out vertices in the total list to draw between.
            // NOTE that despite INEFFICIENTLY creating 4 vertices per cube face (which could potentially lead to a total of 24 vertices if the cube is alone)...
            // after doing the next step of culling out the faces that won't be used it will actually end up being MORE efficient than if we didn't do any culling
            // and just kept our old solution of 8 shared vertices per cube.
            // MAYBE in the future if I become a god-tier programmer I can figure out how to share vertices between faces WHILE ALSO culling faces that are hidden.
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

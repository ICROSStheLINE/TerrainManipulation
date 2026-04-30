using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk
{
  MeshFilter meshFilter;
  MeshRenderer meshRenderer;
  MeshCollider meshCollider;

 
  Mesh mesh;
  List<Vector3> vertices;
  List<int> triangles;
  List<Color> colors; // refer to https://docs.unity3d.com/ScriptReference/Mesh-colors.html

  public static int chunkLength = 15; // x  (I had to make these static so I could use them to make the cube map)
  public static int chunkHeight = 40; // y
  public static int chunkWidth = chunkLength; // z
  
  // 3D array docs: https://www.w3schools.com/cs/cs_arrays_multi.php
  public Block[,,] cubeMap = new Block[chunkLength,chunkHeight,chunkWidth];

  public static int groundHeight = 20;

  int chunkX;
  int chunkY;
  World worldScript;

  public Chunk(int chunkX_, int chunkY_, World worldScript_)
  // This constructor == void Start()
  // We can't use void Start cause this script will NOT be present in the gameObject
  // Since we are creating the gameobject with the class we cant just add it to the chunk (chicken vs egg scenario)
	{
    chunkX = chunkX_;
    chunkY = chunkY_;
    worldScript = worldScript_;
		GameObject chunkObject = new GameObject();
    chunkObject.transform.position = new Vector3(chunkX, 0, chunkY) * chunkLength;
    chunkObject.transform.SetParent(worldScript_.transform);
    chunkObject.name = "Chunk [" + chunkX + "," + chunkY + "]";
		meshFilter = chunkObject.AddComponent<MeshFilter>();
		meshRenderer = chunkObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = worldScript_.blockMaterial;
    meshCollider = chunkObject.AddComponent<MeshCollider>();
		
		vertices = new List<Vector3>();
    triangles = new List<int>();
    colors = new List<Color>();
    mesh = new Mesh();

    ClearData();

    FillCubeMap();
	}


  void FillCubeMap()
  {
    for (int x = 0; x < chunkLength; x++)
    {
      for (int y = 0; y < chunkHeight; y++)
      {
        for (int z = 0; z < chunkWidth; z++)
        {
          cubeMap[x,y,z] = new Block();
          if (y > groundHeight)
          {
            cubeMap[x,y,z].isSolid = false;
            continue;
          }
          cubeMap[x,y,z].isSolid = true;
        }
      }
    }
  }

  public bool CheckForCube(int x, int y, int z)
  {
    int chunkX = this.chunkX; 
    int chunkY = this.chunkY;
    if (z < 0)
    {
      chunkY -= 1;
      z += chunkWidth;
      return worldScript.CheckCubeInChunk(x,y,z,chunkX,chunkY);
    }
    if (z >= chunkWidth)
    {
      chunkY += 1;
      z -= chunkWidth;
      return worldScript.CheckCubeInChunk(x,y,z,chunkX,chunkY);
    }
    if (x < 0)
    {
      chunkX -= 1;
      x += chunkLength;
      return worldScript.CheckCubeInChunk(x,y,z,chunkX,chunkY);
    }
    if (x >= chunkLength)
    {
      chunkX += 1;
      x -= chunkLength;
      return worldScript.CheckCubeInChunk(x,y,z,chunkX,chunkY);
    }
    if (y < 0)
    {
      return false;
    }
    if (y >= chunkHeight)
    {
      return false;
    }
    
    return cubeMap[x,y,z].isSolid;
  }

  bool CheckForNeighbouringCube(int x, int y, int z, int faceIndex) // Returns true if neighbour exists
  {
    if (faceIndex == 0) // Vector3.backward
    {
      return CheckForCube(x,y,z - 1);
    }
    if (faceIndex == 1) // Vector3.forward
    {
      return CheckForCube(x,y,z + 1);
    }
    if (faceIndex == 2) // Vector3.left
    {
      return CheckForCube(x - 1,y,z);
    }
    if (faceIndex == 3) // Vector3.right
    {
      return CheckForCube(x + 1,y,z);
    }
    if (faceIndex == 4) // Vector3.down
    {
      return CheckForCube(x,y - 1,z);
    }
    if (faceIndex == 5) // Vector3.up
    {
      return CheckForCube(x,y + 1,z);
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
          if (!cubeMap[x,y,z].isSolid) // If this cube position is supposed to be empty
          {
            continue;
          }
          for (int faceIndex = 0; faceIndex < 6; faceIndex++)
          {
            if (CheckForNeighbouringCube(x, y, z, faceIndex)) // Only draw if there is no adjacent cube
            { continue; }

            int[] lastFourVertexIndicesAdded = new int[4];
            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
            {
              Vector3 vertexPosition = voxelVertices[voxelVertexIndex[faceIndex, vertexIndex]] + new Vector3(x,y,z);
              vertices.Add(vertexPosition);
              lastFourVertexIndicesAdded[vertexIndex] = vertices.Count - 1;

              // Check to see what block type cubeMap[x,y,z] is and then apply its required colours
              // ...Also add block types

              if (y > groundHeight)
                colors.Add(Color.green);
              else
                colors.Add(Color.red);
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

  public void ClearData()
  {
    vertices.Clear();
    triangles.Clear();
    colors.Clear();
    mesh.Clear();
  }

  public void UploadMesh()
  {
    mesh.SetVertices(vertices);
    mesh.SetTriangles(triangles, 0, false);
    mesh.SetColors(colors);

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

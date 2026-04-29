using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    static int worldLength = 10;
    static int worldWidth = 10;
    Chunk[,] chunkMap = new Chunk[worldLength, worldWidth];
    public Material blockMaterial;

    void Start()
    {
        FillChunkMap();
        GenerateAllChunkMeshes();
    }

    void FillChunkMap()
    {
        for (int x = 0; x < worldLength; x++)
        {
            for (int y = 0; y < worldWidth; y++)
            {
                chunkMap[x,y] = new Chunk(x,y, this); // "this" passes in this script (not the gameObject)
            }
        }
    }

    void GenerateAllChunkMeshes()
    {
        for (int x = 0; x < worldLength; x++)
        {
            for (int y = 0; y < worldWidth; y++)
            {
                chunkMap[x,y].GenerateMesh();
                chunkMap[x,y].UploadMesh();
            }
        }
    }

    public bool CheckCubeInChunk(int chunkX, int chunkY, int cubeX, int cubeY, int cubeZ)
    {
        if (chunkX < 0 || chunkX >= worldLength || chunkY < 0 || chunkY >= worldWidth)
        {
            return false;
        }

        return chunkMap[chunkX,chunkY].CheckForCube(cubeX,cubeY,cubeZ);
    }
}

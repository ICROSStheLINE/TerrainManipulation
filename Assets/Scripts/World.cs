using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    static int worldLength = 5;
    static int worldWidth = 5;
    int groundHeight = 20;

    Chunk[,] chunkMap = new Chunk[worldLength, worldWidth];
    public Material blockMaterial;

    void Start()
    {
        FillChunkMap();
        
        int houseChunkX = Mathf.FloorToInt(worldLength/2);
        int houseChunkY = Mathf.FloorToInt(worldWidth/2);
        DrawHouse(1, groundHeight+1, 1, houseChunkX, houseChunkY);

        GenerateAllChunkMeshes();
    }

    void DrawHouse(int x, int y, int z, 
                   int chunkX, int chunkY)
    {
        DrawBlocks(x,y,z, x+7,y+7,z+7, true, chunkX, chunkY); // Giant cube
        DrawBlocks(x+1,y+0,z+1, x+6,y+6,z+6, false, chunkX, chunkY); // Hollow out cube
        DrawBlocks(x+7,y+0,z+4, x+7,y+1,z+4, false, chunkX, chunkY); // Door
        DrawBlocks(x+7,y+1,z+2, x+7,y+1,z+2, false, chunkX, chunkY); // Window next to door
        for (int i = 0; i < 3; i++)
        DrawBlocks(x+(-1+i),y+(7+i),z+(-1+i), x+(8-i),y+(7+i),z+(8-i), true, chunkX, chunkY); // Roof Layers
    }

    void DrawBlocks(int startX, int startY, int startZ, 
                    int endX, int endY, int endZ, 
                    bool draw,
                    int chunkX, int chunkY) 
    {
        for (int x = startX; x <= endX; x++) {
        for (int y = startY; y <= endY; y++) {
            for (int z = startZ; z <= endZ; z++) {
            chunkMap[chunkX,chunkY].cubeMap[x,y,z] = draw;
            }
        }
        }
    }

    void DrawBlock()
    {
        // First check to see if the target block is in the specified chunk's bounds
        // If not, draw the block in the correct spot in the next chunk over
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
                chunkMap[x,y].ClearData();
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

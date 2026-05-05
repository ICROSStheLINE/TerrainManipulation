using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    static int worldLength = 1;
    static int worldWidth = 1;

    Chunk[,] chunkMap = new Chunk[worldLength, worldWidth];
    public Material blockMaterial;

    void Start()
    {
        FillChunkMap();

        // int houseChunkX = Mathf.FloorToInt(worldLength/2);
        // int houseChunkY = Mathf.FloorToInt(worldWidth/2);
        // DrawHouse(1, Chunk.groundHeight+1, 1, houseChunkX, houseChunkY);

        GenerateAllChunkMeshes();
    }
    void Update()
    {
        GenerateAllChunkMeshes();
    }

    void DrawHouse(int x, int y, int z, 
                   int chunkX, int chunkY)
    {
        DrawBlocks(x,y,z, x+7,y+7,z+7, chunkX, chunkY, Block.BlockType.Stone); // Giant cube
        DrawBlocks(x+1,y+0,z+1, x+6,y+6,z+6, chunkX, chunkY, Block.BlockType.Air); // Hollow out cube
        DrawBlocks(x+7,y+0,z+4, x+7,y+1,z+4, chunkX, chunkY, Block.BlockType.Air); // Door
        DrawBlocks(x+7,y+1,z+2, x+7,y+1,z+2, chunkX, chunkY, Block.BlockType.Air); // Window next to door
        for (int i = 0; i < 3; i++)
        DrawBlocks(x+(-1+i),y+(7+i),z+(-1+i), x+(8-i),y+(7+i),z+(8-i), chunkX, chunkY, Block.BlockType.Stone); // Roof Layers
    }

    void DrawBlocks(int startX, int startY, int startZ, 
                    int endX, int endY, int endZ,
                    int chunkX, int chunkY,
                    Block.BlockType blockType) 
    {
        for (int x = startX; x <= endX; x++) {
        for (int y = startY; y <= endY; y++) {
        for (int z = startZ; z <= endZ; z++) {
            DrawBlock(x,y,z,chunkX,chunkY,blockType);
        }}}
    }

    public void DrawBlock(int x, int y, int z,
                   int chunkX, int chunkY,
                   Block.BlockType blockType)
    {
        if (chunkX < 0 || chunkX >= worldLength || chunkY < 0 || chunkY >= worldWidth)
        {
            return;
        }
        if (z < 0)
        {
            chunkY -= 1;
            z += Chunk.chunkWidth;
            DrawBlock(x,y,z,chunkX,chunkY,blockType);
        }
        if (z >= Chunk.chunkWidth)
        {
            chunkY += 1;
            z -= Chunk.chunkWidth;
            DrawBlock(x,y,z,chunkX,chunkY,blockType);
        }
        if (x < 0)
        {
            chunkX -= 1;
            x += Chunk.chunkLength;
            DrawBlock(x,y,z,chunkX,chunkY,blockType);
        }
        if (x >= Chunk.chunkLength) // If it's out of bounds
        {
            chunkX += 1;
            x -= Chunk.chunkLength;
            DrawBlock(x,y,z,chunkX,chunkY,blockType);
        }
        if (y < 0)
        {
            return;
        }
        if (y >= Chunk.chunkHeight)
        {
            return;
        }
        chunkMap[chunkX,chunkY].cubeMap[x,y,z] = blockType;
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

    public bool CheckCubeInChunk(int cubeX, int cubeY, int cubeZ, int chunkX, int chunkY)
    {
        if (chunkX < 0 || chunkX >= worldLength || chunkY < 0 || chunkY >= worldWidth)
        {
            return false;
        }

        return chunkMap[chunkX,chunkY].CheckForCube(cubeX,cubeY,cubeZ);
    }

    static public (int,int,int,int,int) ConvertWorldPositionToCubeInChunk(Vector3 worldPosition)
    {
        int blockX;
        int blockY;
        int blockZ;
        int chunkX;
        int chunkY;

        blockX = Mathf.FloorToInt(worldPosition.x);
        blockY = Mathf.FloorToInt(worldPosition.y);
        blockZ = Mathf.FloorToInt(worldPosition.z);

        chunkX = Mathf.FloorToInt(blockX / Chunk.chunkLength);
        chunkY = Mathf.FloorToInt(blockZ / Chunk.chunkWidth);

        blockX = blockX - (chunkX * Chunk.chunkLength);
        blockZ = blockZ - (chunkY * Chunk.chunkWidth);

        return (blockX, blockY, blockZ, chunkX, chunkY);
    }
}

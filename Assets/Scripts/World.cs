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
    }

    void FillChunkMap()
    {
        for (int x = 0; x < worldLength; x++)
        {
            for (int z = 0; z < worldWidth; z++)
            {
                chunkMap[x,z] = new Chunk(new Vector3(x,0,z), this); // "this" passes in this script (not the gameObject)
            }
        }
    }
}

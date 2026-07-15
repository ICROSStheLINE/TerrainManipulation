using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheCube : MonoBehaviour
{
    Material material;
    World world;
    PhysicalProperties physicalProperties;
    float previousManaCharge = 0;
    bool materialized = false;
    List<Transform> detectionPoints = new List<Transform>();
    Vector3 latchAnchor;
    bool latched = false;
    List<BasicBlockInfo> latchedBlocks = new List<BasicBlockInfo>();
    Color defaultColor;
    

    void Start()
    {
        GameObject worldGameObject = GameObject.FindWithTag("World");
        world = worldGameObject.transform.GetComponent<World>();
        material = GetComponent<Renderer>().material;
        defaultColor = material.GetColor("_Color");
        physicalProperties = GetComponent<PhysicalProperties>();
        previousManaCharge = physicalProperties.manaCharge;

        foreach (Transform child in transform)
        {
            detectionPoints.Add(child);
        }

        // Invoke("Materialize", 5f);
    }


    void Update()
    {
        float manaChargeDelta = physicalProperties.manaCharge - previousManaCharge;

        if (manaChargeDelta != 0 && !materialized)
        {
            Materialize();
        }
        else if (manaChargeDelta == 0 && materialized)
        {
            Dematerialize();
        }

        if (latched)
        {
            DragAroundLatchedObjects();
        }

        previousManaCharge = physicalProperties.manaCharge;
    }

    void Materialize()
    {
        material.SetColor("_Color", Color.black);

        List<BasicBlockInfo> overlappedBlocks = OverlappingBlocks();

        if (overlappedBlocks.Count > 0)
        {
            Latch(overlappedBlocks);
        }
    }

    void Dematerialize()
    {
        material.SetColor("_Color", defaultColor);

        if (latched)
        {
            Unlatch();
        }
    }

    void Unlatch()
    {
        latched = false;
    }

    void Latch(List<BasicBlockInfo> blocksToLatchOnto)
    {
        latched = true;
        latchAnchor = transform.position;
        latchedBlocks = blocksToLatchOnto;

        // I have no idea what to do here.
        // I guess take in the cube position and information as arguments?
        // Then move them accordingly and have them only be able to move in a spot that an air block was?
        // Maybe find a way to store information on where the overlapping cube was relative to this object, then constantly keep it moving towards that relative position
    }

    void DragAroundLatchedObjects()
    {
        int latchDeltaX = 0;
        int latchDeltaY = 0;
        int latchDeltaZ = 0;
        if (transform.position.x - latchAnchor.x > 1f) latchDeltaX++;
        if (transform.position.x - latchAnchor.x < -1f) latchDeltaX--;
        if (transform.position.y - latchAnchor.y > 1f) latchDeltaY++;
        if (transform.position.y - latchAnchor.y < -1f) latchDeltaY--;
        if (transform.position.z - latchAnchor.z > 1f) latchDeltaZ++;
        if (transform.position.z - latchAnchor.z < -1f) latchDeltaZ--;
        if (latchDeltaX != 0 || latchDeltaY != 0 || latchDeltaZ != 0)
        {
            // For testing purposes, I am testing to see if moving TheCube one unit up the x axis would bring the latched block with it
            for (int i = 0; i < latchedBlocks.Count; i++)
            {
                world.DrawBlock(latchedBlocks[i].cubePos.x,
                    latchedBlocks[i].cubePos.y,
                    latchedBlocks[i].cubePos.z,
                    latchedBlocks[i].chunkPos.x,
                    latchedBlocks[i].chunkPos.y,
                    Block.BlockType.Air,
                    false);
            }
        }
        
        if (latchDeltaX != 0 || latchDeltaY != 0 || latchDeltaZ != 0)
        {
            // For testing purposes, I am testing to see if moving TheCube one unit up the x axis would bring the latched block with it
            for (int i = 0; i < latchedBlocks.Count; i++)
            {
                BasicBlockInfo thisCube = new BasicBlockInfo();
                thisCube.blockType = latchedBlocks[i].blockType;
                thisCube.cubePos = latchedBlocks[i].cubePos + new Vector3Int(latchDeltaX,latchDeltaY,latchDeltaZ);
                thisCube.chunkPos = latchedBlocks[i].chunkPos;
                latchedBlocks[i] = thisCube;
                // Somehow apply this positional change to the actual block lol
                world.DrawBlock(thisCube.cubePos.x,
                    thisCube.cubePos.y,
                    thisCube.cubePos.z,
                    thisCube.chunkPos.x,
                    thisCube.chunkPos.y,
                    thisCube.blockType,
                    true);
            }
            latchAnchor += new Vector3(latchDeltaX,latchDeltaY,latchDeltaZ);
            // latchAnchor = transform.position;
        }
    }

    List<BasicBlockInfo> OverlappingBlocks()
    {
        List<BasicBlockInfo> overlappingBlocks = new List<BasicBlockInfo>();
        // Find all the the cubes overlapping with the detection points using world.ConvertWorldPositionToCubeInChunk(...)
        foreach (Transform detectionPoint in detectionPoints)
        {
            // Find the info on what blocktype that cube and chunk position refers to
            int x;
            int y;
            int z;
            int chunkX;
            int chunkY;
            (x,y,z,chunkX,chunkY) = World.ConvertWorldPositionToCubeInChunk(detectionPoint.position);
            if (world.CheckCubeInChunk(x,y,z,chunkX,chunkY)) // Check if this cube is a solid block
            {
                BasicBlockInfo thisCube = new BasicBlockInfo();
                thisCube.blockType = world.CheckCubeTypeInChunk(x,y,z,chunkX,chunkY);
                thisCube.cubePos = new Vector3Int(x,y,z);
                thisCube.chunkPos = new Vector2Int(chunkX,chunkY);
                if (!overlappingBlocks.Contains(thisCube))
                {
                    overlappingBlocks.Add(thisCube); // I can't tell if this is spaghetti code or not
                }
            }
        }

        // Return an array of blocktypes that aren't air blocks. maybe also return their position too so I can move them around
        // Profit
        foreach (BasicBlockInfo overlappingCube in overlappingBlocks)
            Debug.Log(overlappingCube.blockType + "\n" + overlappingCube.cubePos + "\n" + overlappingCube.chunkPos);
        return overlappingBlocks;
    }

    struct BasicBlockInfo
    {
        public Block.BlockType blockType;
        public Vector3Int cubePos;
        public Vector2Int chunkPos;
    }
}

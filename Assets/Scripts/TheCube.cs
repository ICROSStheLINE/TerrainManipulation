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
    BoxCollider boxCollider;
    List<Transform> detectionPoints = new List<Transform>();
    

    void Start()
    {
        GameObject worldGameObject = GameObject.FindWithTag("World");
        world = worldGameObject.transform.GetComponent<World>();
        material = GetComponent<Renderer>().material;
        physicalProperties = GetComponent<PhysicalProperties>();
        previousManaCharge = physicalProperties.manaCharge;
        boxCollider = GetComponent<BoxCollider>();

        foreach (Transform child in transform)
        {
            detectionPoints.Add(child);
        }

        Invoke("OverlappingCubes", 5f);
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


        previousManaCharge = physicalProperties.manaCharge;
    }

    void Materialize()
    {
        // if OverlappingCubes > 0 then call Latch()
    }

    void Dematerialize()
    {
        
    }

    void Latch()
    {
        // I have no idea what to do here.
        // I guess take in the cube position and information as arguments?
        // Then move them accordingly and have them only be able to move in a spot that an air block was?
        // Maybe find a way to store information on where the overlapping cube was relative to this object, then constantly keep it moving towards that relative position
    }

    List<OverlappingCube> OverlappingCubes()
    {
        List<OverlappingCube> overlappingCubes = new List<OverlappingCube>();
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
                OverlappingCube thisCube = new OverlappingCube();
                thisCube.blockType = world.CheckCubeTypeInChunk(x,y,z,chunkX,chunkY);
                thisCube.cubePos = new Vector3Int(x,y,z);
                thisCube.chunkPos = new Vector2Int(chunkX,chunkY);
                if (!overlappingCubes.Contains(thisCube))
                {
                    overlappingCubes.Add(thisCube); // I can't tell if this is spaghetti code or not
                }
            }
        }

        // Return an array of blocktypes that aren't air blocks. maybe also return their position too so I can move them around
        // Profit
        foreach (OverlappingCube overlappingCube in overlappingCubes)
            Debug.Log(overlappingCube.blockType + "\n" + overlappingCube.cubePos + "\n" + overlappingCube.chunkPos);
        return overlappingCubes;
    }

    struct OverlappingCube
    {
        public Block.BlockType blockType;
        public Vector3Int cubePos;
        public Vector2Int chunkPos;
    }
}

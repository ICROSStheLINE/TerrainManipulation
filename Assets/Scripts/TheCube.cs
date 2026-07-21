using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    Coroutine manaFlowCoroutine;
    float manaChargeDelta;
    Vector3 originalScale;
    HashSet<BasicBlockInfo> placedBlocks = new HashSet<BasicBlockInfo>();
    HashSet<BasicBlockInfo> randomizedBlocks = new HashSet<BasicBlockInfo>();
    HashSet<Vector2Int> affectedChunks = new HashSet<Vector2Int>();
    

    void Start()
    {
        GameObject worldGameObject = GameObject.FindWithTag("World");
        world = worldGameObject.transform.GetComponent<World>();
        material = GetComponent<Renderer>().material;
        defaultColor = material.GetColor("_Color");
        physicalProperties = GetComponent<PhysicalProperties>();
        previousManaCharge = physicalProperties.manaCharge;
        foreach (Transform child in transform)
        { detectionPoints.Add(child); }
        originalScale = transform.localScale;
        StartCoroutine("Shrink");
        Destroy(gameObject, 3f);

        // Invoke("Materialize", 5f);
    }


    void Update()
    {
        manaChargeDelta = CheckManaChargeDelta();

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

    IEnumerator Shrink()
    {
        float duration = 2;
        float shrinkAmount;
        float elapsedTime = 0;
        int durationDivision = 0;

        while ( elapsedTime < duration )
        {
            elapsedTime += Time.deltaTime;

            shrinkAmount = Mathf.Lerp(1f, 0.1f, elapsedTime / duration);
            transform.localScale = originalScale * shrinkAmount;

            int currentDivision = Mathf.Min(Mathf.FloorToInt((elapsedTime / duration) * 20), 20);

            if (currentDivision > durationDivision)
            {
                durationDivision = currentDivision;

                int latchedBlocksCount = latchedBlocks.Count;
                if (latchedBlocksCount > 1)
                {
                    int groundBlockCount = 0;
                    foreach (BasicBlockInfo latchedBlock in latchedBlocks)
                    { if (Block.IsGround(latchedBlock.blockType)) {groundBlockCount++;} }

                    for (int i = latchedBlocksCount - 1; i > 1; i--)
                    {
                        if (Block.IsGround(latchedBlocks[i].blockType) && groundBlockCount > 1)
                        {
                            latchedBlocks.RemoveAt(i);
                            groundBlockCount--;
                            break;
                        }
                    }
                }
            }

            yield return null;
        }
    }

    float CheckManaChargeDelta()
    {
        float delta = physicalProperties.manaCharge - previousManaCharge;

        if (delta != 0)
        {
            manaChargeDelta = delta;

            if (manaFlowCoroutine != null)
                StopCoroutine(manaFlowCoroutine);

            manaFlowCoroutine = StartCoroutine(ManaFlowCooldown());
        }

        return manaChargeDelta;
    }

    IEnumerator ManaFlowCooldown()
    {
        yield return new WaitForSeconds(0.251f);

        manaChargeDelta = 0;
        manaFlowCoroutine = null;
    }

    void Materialize()
    {
        materialized = true;
        material.SetColor("_Color", Color.black);

        List<BasicBlockInfo> overlappedBlocks = OverlappingBlocks();

        if (overlappedBlocks.Count > 0)
        {
            Latch(overlappedBlocks);
        }
    }

    void Dematerialize()
    {
        materialized = false;
        material.SetColor("_Color", defaultColor);

        if (latched)
        {
            Unlatch();
        }
    }

    void Unlatch()
    {
        placedBlocks.Clear();
        affectedChunks.Clear();
        latched = false;
    }

    void Latch(List<BasicBlockInfo> overlappedBlocks)
    {
        latched = true;
        latchAnchor = transform.position;
        latchedBlocks = overlappedBlocks;

        latchedBlocks.Sort((a, b) =>
        {
            float aDist = (World.BlockToWorldPosition(a.cubePos, a.chunkPos) - latchAnchor).sqrMagnitude;
            float bDist = (World.BlockToWorldPosition(b.cubePos, b.chunkPos) - latchAnchor).sqrMagnitude;

            return aDist.CompareTo(bDist);
        });
    }

    void DragAroundLatchedObjects()
    {
        int latchDeltaX = 0;
        int latchDeltaY = 0;
        int latchDeltaZ = 0;
        if (transform.position.x - latchAnchor.x > Chunk.voxelSize) latchDeltaX++;
        if (transform.position.x - latchAnchor.x < -Chunk.voxelSize) latchDeltaX--;
        if (transform.position.y - latchAnchor.y > Chunk.voxelSize) latchDeltaY++;
        if (transform.position.y - latchAnchor.y < -Chunk.voxelSize) latchDeltaY--;
        if (transform.position.z - latchAnchor.z > Chunk.voxelSize) latchDeltaZ++;
        if (transform.position.z - latchAnchor.z < -Chunk.voxelSize) latchDeltaZ--;
        if (latchDeltaX != 0 || latchDeltaY != 0 || latchDeltaZ != 0)
        {
            // Cool block shaking effect {
            foreach (BasicBlockInfo randomizedBlock in randomizedBlocks)
            {
                world.DrawBlock(randomizedBlock.cubePos.x,
                    randomizedBlock.cubePos.y,
                    randomizedBlock.cubePos.z,
                    randomizedBlock.chunkPos.x,
                    randomizedBlock.chunkPos.y,
                    Block.BlockType.Air,
                    false);
            }
            randomizedBlocks.Clear();
            foreach (BasicBlockInfo placedBlock in placedBlocks)
            {
                world.DrawBlock(placedBlock.cubePos.x,
                    placedBlock.cubePos.y,
                    placedBlock.cubePos.z,
                    placedBlock.chunkPos.x,
                    placedBlock.chunkPos.y,
                    Block.BlockType.Air,
                    false);
            }
            for (int i = 0; i < 3; i++)
            {
                foreach (BasicBlockInfo placedBlock in placedBlocks)
                {
                    BasicBlockInfo randomizedBlock;
                    randomizedBlock.blockType = placedBlock.blockType;
                    randomizedBlock.cubePos = placedBlock.cubePos + new Vector3Int(Random.Range(-1,2),Random.Range(-1,2),Random.Range(-1,2));
                    randomizedBlock.chunkPos = placedBlock.chunkPos;

                    if (world.CheckCubeTypeInChunk(randomizedBlock.cubePos,randomizedBlock.chunkPos) != Block.BlockType.Air)
                    { continue; }
                    world.DrawBlock(randomizedBlock.cubePos.x,
                        randomizedBlock.cubePos.y,
                        randomizedBlock.cubePos.z,
                        randomizedBlock.chunkPos.x,
                        randomizedBlock.chunkPos.y,
                        randomizedBlock.blockType,
                        false);
                    randomizedBlocks.Add(randomizedBlock);
                }
            }  // } Cool block shaking effect

            // For testing purposes, I am testing to see if moving TheCube one unit up the x axis would bring the latched block with it
            for (int i = 0; i < latchedBlocks.Count; i++)
            {
                affectedChunks.Add(latchedBlocks[i].chunkPos);

                if (Block.IsGround(latchedBlocks[i].blockType))
                {
                    // placedBlocks.Add(latchedBlocks[i]);
                    StartCoroutine(AddToPlacedBlocksList(latchedBlocks[i]));
                    continue;
                }

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
                Vector3Int cubePosDestination = latchedBlocks[i].cubePos + new Vector3Int(latchDeltaX,latchDeltaY,latchDeltaZ);
                Vector2Int cubeChunkDestination = latchedBlocks[i].chunkPos;
                (cubePosDestination,cubeChunkDestination) = World.FindRealBlockChunkAndPos(cubePosDestination,cubeChunkDestination);
                if (!Block.IsGround(latchedBlocks[i].blockType) && Block.IsSolid(latchedBlocks[i].blockType) && world.CheckCubeTypeInChunk(cubePosDestination,cubeChunkDestination) != Block.BlockType.Air)
                { continue; }

                BasicBlockInfo thisCube = new BasicBlockInfo();
                thisCube.blockType = latchedBlocks[i].blockType;
                thisCube.cubePos = cubePosDestination;
                thisCube.chunkPos = cubeChunkDestination;
                latchedBlocks[i] = thisCube;
                // Somehow apply this positional change to the actual block lol
                world.DrawBlock(thisCube.cubePos.x,
                    thisCube.cubePos.y,
                    thisCube.cubePos.z,
                    thisCube.chunkPos.x,
                    thisCube.chunkPos.y,
                    thisCube.blockType,
                    false);

                affectedChunks.Add(thisCube.chunkPos);
            }

            foreach (Vector2Int chunk in affectedChunks)
            {
                world.GenerateSpecificChunkMesh(chunk);
            }

            latchAnchor += new Vector3(latchDeltaX,latchDeltaY,latchDeltaZ) * Chunk.voxelSize;
            // latchAnchor = transform.position;
        }
    }

    IEnumerator AddToPlacedBlocksList(BasicBlockInfo placedBlock)
    {
        yield return new WaitForSeconds(0.1f);
        placedBlocks.Add(placedBlock);
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
                thisCube.blockType = world.CheckCubeTypeInChunk(new Vector3Int(x,y,z),new Vector2Int(chunkX,chunkY));
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
        return overlappingBlocks;
    }

    struct BasicBlockInfo
    {
        public Block.BlockType blockType;
        public Vector3Int cubePos;
        public Vector2Int chunkPos;
    }
}

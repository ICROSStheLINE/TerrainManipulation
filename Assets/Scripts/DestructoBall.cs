using UnityEngine;

public class DestructoBall : MonoBehaviour
{
    World world;

    void Start()
    {
        GameObject worldGameObject = GameObject.FindWithTag("World");
        world = worldGameObject.transform.GetComponent<World>();
    }

    void Update()
    {
        DESTROYCUBES();
    }

    void DESTROYCUBES()
    {
        int x;
        int y;
        int z;
        int chunkX;
        int chunkY;
        (x,y,z,chunkX,chunkY) = World.ConvertWorldPositionToCubeInChunk(transform.position);
        world.DrawBlock(x,y,z,chunkX,chunkY,Block.BlockType.Air,true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public bool isSolid;
    public Color color;
    public int hardness;
    public enum BlockType { Air, Grass, Dirt, Stone }

    public Block(BlockType blockType)
    {
        SetBlock(blockType);
    }

    public void SetBlock(BlockType blockType)
    {
        if (blockType == BlockType.Air)
        {
            isSolid = false;
            color = Color.clear;
            hardness = 0;
        }
        if (blockType == BlockType.Grass)
        {
            isSolid = true;
            color = Color.green;
            hardness = 1;
        }
        if (blockType == BlockType.Dirt)
        {
            isSolid = true;
            color = new Color(0.6470588f, 0.1647059f, 0.1647059f);
            hardness = 1;
        }
        if (blockType == BlockType.Stone)
        {
            isSolid = true;
            color = Color.gray;
            hardness = 2;
        }
    }
}

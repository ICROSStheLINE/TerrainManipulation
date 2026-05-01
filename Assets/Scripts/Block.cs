using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Block 
// This is now a STATIC CLASS.
// This means there will NO LONGER BE INSTANCES OF IT!
// It will ONLY SERVE TO HOLD INFORMATION about block types. Sad :(
{
    public enum BlockType { Air, Grass, Dirt, Stone }

    public static bool IsSolid(BlockType blockType)
    {
        if (blockType == BlockType.Air) return false;
        return true;
    }

    public static Color GetColor(BlockType blockType)
    {
        if (blockType == BlockType.Air)
        {
            return Color.clear;
        }
        if (blockType == BlockType.Grass)
        {
            return Color.green;
        }
        if (blockType == BlockType.Dirt)
        {
            return new Color(0.6470588f, 0.1647059f, 0.1647059f);
        }
        if (blockType == BlockType.Stone)
        {
            return Color.gray;
        }

        return Color.magenta;
    }

    public static int GetHardness(BlockType blockType)
    {
        if (blockType == BlockType.Air) return 0;
        if (blockType == BlockType.Grass) return 1;
        if (blockType == BlockType.Dirt) return 1;
        if (blockType == BlockType.Stone) return 2;

        return 0;
    }
}
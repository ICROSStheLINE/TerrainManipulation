using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Block 
// This is now a STATIC CLASS.
// This means there will NO LONGER BE INSTANCES OF IT!
// It will ONLY SERVE TO HOLD INFORMATION about block types. Sad :(
{
    public enum BlockType { Air, Grass, Dirt, Stone, Wood }

    public static bool IsSolid(BlockType blockType)
    {
        if (blockType == BlockType.Air) return false;
        if (blockType == BlockType.Grass) return true;
        if (blockType == BlockType.Dirt) return true;
        if (blockType == BlockType.Stone) return true;
        if (blockType == BlockType.Wood) return true;
        
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
        if (blockType == BlockType.Wood)
        {
            return new Color(0.5f, 0.25f, 0.0f);
        }

        return Color.magenta;
    }

    public static int GetHardness(BlockType blockType)
    {
        if (blockType == BlockType.Air) return 0;
        if (blockType == BlockType.Grass) return 1;
        if (blockType == BlockType.Dirt) return 1;
        if (blockType == BlockType.Stone) return 3;
        if (blockType == BlockType.Wood) return 2;

        return 0;
    }
}
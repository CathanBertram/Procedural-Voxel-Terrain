using System;
using System.Collections;
using System.Collections.Generic;
using Blocks;
using UnityEngine;

public static class TreeGenerator
{
    public static EasingType heightEasing;
    public static EasingType leafStartEasing;
    public static int minTreeHeight;
    public static int maxTreeHeight;
    public static float minLeafStart;
    public static float maxLeafStart;

    public static TreeData GenerateTree(int chunkX = 0, int chunkZ = 0, int xPos = 0, int zPos = 0, int treeNum = 0)
    {
        TreeData treeData = new TreeData();

        System.Random random = new System.Random(Noise.Seed + chunkX + chunkZ + treeNum);

        int noiseX = chunkX + xPos, noiseZ = chunkZ + zPos;
        
        var fn = Noise.noiseSettings2D.CreateFastNoise();
        var treeHeight = Mathf.RoundToInt(Easing.Ease(heightEasing, minTreeHeight, maxTreeHeight, fn.GetNoise(noiseX, noiseZ)));

        int leafStart = Mathf.RoundToInt(treeHeight * Easing.Ease(leafStartEasing, minLeafStart, maxLeafStart,
            fn.GetNoise(noiseX + treeHeight, noiseZ + treeHeight)));
        
        int leafRadius = Mathf.FloorToInt(treeHeight * 0.75f);
        int additionalLeafHeight = random.Next(1, 2);
        treeData.radius = leafRadius;
        
        byte logId = BlockDatabase.Instance.GetBlockID("Log");
        byte leafId = BlockDatabase.Instance.GetBlockID("Leaves");

        
        
        for (int i = 0; i < treeHeight; i++)
        {
            treeData.treeData.Add(new Vector3Int(0, i, 0), logId);
        }

        for (int y = leafStart; y < treeHeight + 2; y++)
        {
            for (int x = -leafRadius; x < leafRadius; x++)
            {
                for (int z = -leafRadius; z < leafRadius; z++)
                {
                    if ((x * x) + ((y - leafStart - 1) * (y - leafStart - 1)) + (z * z) < (leafRadius * leafRadius))
                    //if (Math.Abs(x) + Math.Abs(y) + Math.Abs(z) < treeHeight)
                    {
                        var pos = new Vector3Int(x, y, z);

                        if (treeData.treeData.ContainsKey(pos))
                            continue;
                        treeData.treeData.Add(pos, leafId);
                    }
                }
            }
        }

        return treeData;
    }
}

public class TreeData
{
    public Dictionary<Vector3Int, byte> treeData;
    public int radius;

    public TreeData()
    {
        treeData = new Dictionary<Vector3Int, byte>();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using Blocks;
using UnityEngine;

public static class FeatureGenerator
{
    public static EasingType heightEasing;
    public static EasingType leafStartEasing;
    public static int minTreeHeight;
    public static int maxTreeHeight;
    public static float minLeafStart;
    public static float maxLeafStart;

    public static FeatureData GenerateFeature(Biome biome, int chunkX = 0, int chunkZ = 0, int xPos = 0, int zPos = 0,
        int featureNum = 0)
    {
        switch (biome.featureToGenerate)
        {
            case Biome.FeatureToGenerate.Cactus:
                return GenerateCactus(chunkX, chunkZ, xPos, zPos, featureNum);
            case Biome.FeatureToGenerate.Tree:
                return GenerateTree(chunkX, chunkZ, xPos, zPos, featureNum);
        }
        return GenerateTree(chunkX, chunkZ, xPos, zPos, featureNum);
    }
    
    public static FeatureData GenerateTree(int chunkX = 0, int chunkZ = 0, int xPos = 0, int zPos = 0, int treeNum = 0)
    {
        FeatureData featureData = new FeatureData();

        System.Random random = new System.Random(Noise.Seed + chunkX + chunkZ + treeNum);

        int noiseX = chunkX + xPos, noiseZ = chunkZ + zPos;
        
        var fn = Noise.noiseSettings2D.CreateFastNoise();
        var treeHeight = Mathf.RoundToInt(Easing.Ease(heightEasing, minTreeHeight, maxTreeHeight, fn.GetNoise(noiseX, noiseZ)));

        int leafStart = Mathf.RoundToInt(treeHeight * Easing.Ease(leafStartEasing, minLeafStart, maxLeafStart,
            fn.GetNoise(noiseX + treeHeight, noiseZ + treeHeight)));
        
        int leafRadius = Mathf.FloorToInt(treeHeight * 0.75f);
        int additionalLeafHeight = random.Next(1, 2);
        featureData.radius = leafRadius;
        
        byte logId = BlockDatabase.Instance.GetBlockID("Log");
        byte leafId = BlockDatabase.Instance.GetBlockID("Leaves");

        
        
        for (int i = 0; i < treeHeight; i++)
        {
            featureData.featureData.Add(new Vector3Int(0, i, 0), logId);
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

                        if (featureData.featureData.ContainsKey(pos))
                            continue;
                        featureData.featureData.Add(pos, leafId);
                    }
                }
            }
        }

        return featureData;
    }
    public static FeatureData GenerateCactus(int chunkX = 0, int chunkZ = 0, int xPos = 0, int zPos = 0, int cactusNum = 0)
    {
        FeatureData featureData = new FeatureData();

        System.Random random = new System.Random(Noise.Seed + chunkX + chunkZ + cactusNum);

        int noiseX = chunkX + xPos, noiseZ = chunkZ + zPos;
        
        var fn = Noise.noiseSettings2D.CreateFastNoise();
        var cactusHeight = Mathf.RoundToInt(Easing.Ease(heightEasing, minTreeHeight, maxTreeHeight, fn.GetNoise(noiseX, noiseZ)));
        
        featureData.radius = 1;
        
        byte blockID = BlockDatabase.Instance.GetBlockID("Cactus");



        for (int i = 0; i < cactusHeight; i++)
        {
            featureData.featureData.Add(new Vector3Int(0, i, 0), blockID);
        }
        

        return featureData;
    }
}

public class FeatureData
{
    public Dictionary<Vector3Int, byte> featureData;
    public int radius;

    public FeatureData()
    {
        featureData = new Dictionary<Vector3Int, byte>();
    }

}

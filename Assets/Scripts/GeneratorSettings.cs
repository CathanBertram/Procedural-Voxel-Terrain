using System;
using System.Collections;
using System.Collections.Generic;
using Generation;
using UnityEngine;

public class GeneratorSettings : MonoBehaviour
{
    public float noiseCaveThreshold;
    public EasingType easingType;
    public int lowerBound;
    public int upperBound;
    public int seed;
    public NoiseSettings noiseSettings2D;
    public NoiseSettings noiseSettings3D;
    
    public EasingType heightEasing;
    public EasingType leafStartEasing;
    public int minTreeHeight;
    public int maxTreeHeight;
    [Range(0,1)]
    public float minLeafStart;
    [Range(0,1)]
    public float maxLeafStart;

    public int maxTreesPerChunk;
    public int maxTreeAttempts;
    [Range(0,100)]
    public int treeSpawnChance;
    private void Awake()
    {
        Noise.Seed = seed;
        noiseSettings2D.seed = seed;
        noiseSettings3D.seed = seed;
        Noise.noiseSettings2D = noiseSettings2D;
        Noise.noiseSettings3D = noiseSettings3D;

        ChunkGenerator.easingType = easingType;
        ChunkGenerator.lowerBound = lowerBound;
        ChunkGenerator.upperBound = upperBound;
        ChunkGenerator.noiseCaveThreshold = noiseCaveThreshold;
        
        FeatureGenerator.minLeafStart = minLeafStart;
        FeatureGenerator.maxLeafStart = maxLeafStart;
        FeatureGenerator.maxTreeHeight = maxTreeHeight;
        FeatureGenerator.minTreeHeight = minTreeHeight;

        FeatureGenerator.heightEasing = heightEasing;
        FeatureGenerator.leafStartEasing = leafStartEasing;

        ChunkGenerator.maxTreeAttempts = maxTreeAttempts;
        ChunkGenerator.maxTreesPerChunk = maxTreesPerChunk;
        ChunkGenerator.treeSpawnChance = treeSpawnChance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Biome", fileName = "Biome", order = 0)]
public class Biome : ScriptableObject
{
    public string biomeName;
    
    public EasingType easingType;
    public int lowerBound;
    public int upperBound;
    
    public string primaryTopLayerBlock;
    public Vector2Int primaryTopLayerBlockRange;
    public string secondaryTopLayerBlock;
    public Vector2Int secondaryTopLayerBlockRange;

    public NoiseSettings biomeNoiseSettings;

    public FeatureToGenerate featureToGenerate;
    
    public int maxFeaturesPerChunk;
    public int maxFeatureAttempts;
    [Range(0,100)]
    public int featureSpawnChance;
    public enum FeatureToGenerate
    {
        Tree,
        Cactus
    }
}

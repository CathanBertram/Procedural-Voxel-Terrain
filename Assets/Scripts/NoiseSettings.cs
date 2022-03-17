using System;
using System.Collections;
using System.Collections.Generic;
using Generation;
using Unity.Mathematics;
using UnityEngine;

public class NoiseSettings : MonoBehaviour
{
    public EasingType easingType;
    public int lowerBound;
    public int upperBound;
    public FastNoiseUnity fastNoiseUnity;
    public bool updateVFX;
    public FastNoise.NoiseType noiseType;
    public int seed = 1337;
    public float frequency = 0.01f;
    public FastNoise.Interp interp = FastNoise.Interp.Quintic;
    public float cellularJitter = 0.45f;
    public float gain = 0.5f;
    public float lacunarity = 2.0f;
    public int octaves = 3;
    public FastNoise.FractalType fractalType = FastNoise.FractalType.FBM;
    public int cellularDistanceIndex0 = 0;
    public int cellularDistanceIndex1 = 1;
    public FastNoise.CellularDistanceFunction cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean;
    public float gradientPerturbAmp = 1.0f;
    public FastNoise.CellularReturnType cellularReturnType = FastNoise.CellularReturnType.CellValue;

    private void Awake()
    {
        Noise.SetSettings(noiseType, seed, interp, frequency, gain, lacunarity, octaves, fractalType, cellularJitter, 
            cellularDistanceIndex0, cellularDistanceIndex1, 
            cellularDistanceFunction,cellularReturnType,gradientPerturbAmp);

        ChunkGenerator.easingType = easingType;
        ChunkGenerator.lowerBound = lowerBound;
        ChunkGenerator.upperBound = upperBound;
    }

    private void OnValidate()
    {
        if (!updateVFX) return;

        fastNoiseUnity.noiseType = noiseType;
        fastNoiseUnity.seed = seed;
        fastNoiseUnity.frequency = frequency;
        fastNoiseUnity.interp = interp;
        fastNoiseUnity.cellularJitter = cellularJitter;
        fastNoiseUnity.gain = gain;
        fastNoiseUnity.lacunarity = lacunarity;
        fastNoiseUnity.octaves = octaves;
        fastNoiseUnity.fractalType = fractalType;
        fastNoiseUnity.cellularDistanceIndex0 = cellularDistanceIndex0;
        fastNoiseUnity.cellularDistanceIndex1 = cellularDistanceIndex1;
        fastNoiseUnity.cellularDistanceFunction = cellularDistanceFunction;
        fastNoiseUnity.cellularReturnType = cellularReturnType;
        fastNoiseUnity.gradientPerturbAmp = gradientPerturbAmp;
    }
}

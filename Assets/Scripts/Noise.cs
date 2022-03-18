using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Noise
{
    private static FastNoise.NoiseType noiseType;
    private static int seed = 1337;
    public static int Seed => seed;
    private static FastNoise.Interp interp = FastNoise.Interp.Quintic;
    private static float frequency = 0.01f;
    private static float gain = 0.5f;
    private static float lacunarity = 2.0f;
    private static int octaves = 3;
    private static FastNoise.FractalType fractalType = FastNoise.FractalType.FBM;
    //Cellular Settings
    private static float cellularJitter = 0.45f;
    private static int cellularDistanceIndex0 = 0;
    private static int cellularDistanceIndex1 = 1;
    private static FastNoise.CellularDistanceFunction cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean;
    private static FastNoise.CellularReturnType cellularReturnType = FastNoise.CellularReturnType.CellValue;
    private static float gradientPerturbAmp = 1.0f;

    public static void SetSettings(FastNoise.NoiseType _noiseType = FastNoise.NoiseType.Simplex, int _seed = 1337, FastNoise.Interp _interp = FastNoise.Interp.Quintic, float _frequency = 0.01f,
        float _gain = 0.5f, float _lacunarity = 2.0f, int _octaves = 3, FastNoise.FractalType _fractalType = FastNoise.FractalType.FBM, float _cellularJitter = 0.45f, 
        int _cellularDistanceIndex0 = 0, int _cellularDistanceIndex1 = 1, FastNoise.CellularDistanceFunction _cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean,
        FastNoise.CellularReturnType _cellularReturnType = FastNoise.CellularReturnType.CellValue, float _gradientPerturbAmp = 1.0f)
    {
        noiseType = _noiseType; seed = _seed; interp = _interp; frequency = _frequency; gain = _gain; lacunarity = _lacunarity; octaves = _octaves; fractalType = _fractalType;
        cellularJitter = _cellularJitter; cellularDistanceIndex0 = _cellularDistanceIndex0; cellularDistanceIndex1 = _cellularDistanceIndex1; cellularDistanceFunction = _cellularDistanceFunction;
        cellularReturnType = _cellularReturnType; gradientPerturbAmp = _gradientPerturbAmp;
    }

    
    // public static float UnityPerlinNoise2D(float x, float y, System.Random prng)
    // {
    //     Vector2[] octaveOffsets = new Vector2[perlinNoiseSettings.octaves];
    //
    //     //System.Random prng = new System.Random(seed);
    //     prng = new System.Random(seed);
    //
    //     for (int i = 0; i < perlinNoiseSettings.octaves; i++) {
    //         float offsetX = prng.Next (-100000, 100000);
    //         float offsetY = prng.Next (-100000, 100000);
    //         octaveOffsets [i] = new Vector2 (offsetX, offsetY);
    //     }
    //     
    //     if (perlinNoiseSettings.scale <= 0)
    //     {
    //         perlinNoiseSettings.scale = 0.0001f;
    //     }
    //
    //     float amplitude = 1;
    //     float frequency = 1;
    //     float noiseHeight = 0;
    //
    //     for (int i = 0; i < perlinNoiseSettings.octaves; i++)
    //     {
    //         float sampleX = x / perlinNoiseSettings.scale * frequency + octaveOffsets[i].x;
    //         float sampleY = y / perlinNoiseSettings.scale * frequency + octaveOffsets[i].y;
    //
    //         float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
    //         noiseHeight += perlinValue * amplitude;
    //
    //         amplitude *= perlinNoiseSettings.persistance;
    //         frequency *= perlinNoiseSettings.lacunarity;
    //     }
    //
    //     return noiseHeight;
    // }
    // public static float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight) {
    //     float[,] noiseMap = new float[mapWidth,mapHeight];
    //     
    //     System.Random prng = new System.Random(seed);
    //
    //     
    //     Vector2[] octaveOffsets = new Vector2[perlinNoiseSettings.octaves];
    //     for (int i = 0; i < perlinNoiseSettings.octaves; i++) {
    //         float offsetX = prng.Next (-100000, 100000);
    //         float offsetY = prng.Next (-100000, 100000);
    //         octaveOffsets [i] = new Vector2 (offsetX, offsetY);
    //     }
    //
    //     if (perlinNoiseSettings.scale <= 0) {
    //         perlinNoiseSettings.scale = 0.0001f;
    //     }
    //
    //     float maxNoiseHeight = float.MinValue;
    //     float minNoiseHeight = float.MaxValue;
    //
    //     float halfWidth = mapWidth * 0.5f;
    //     float halfHeight = mapHeight * 0.5f;
    //
    //     for (int y = 0; y < mapHeight; y++) {
    //         for (int x = 0; x < mapWidth; x++) {
		  //
    //             float amplitude = 1;
    //             float frequency = 1;
    //             float noiseHeight = 0;
    //
    //             for (int i = 0; i < perlinNoiseSettings.octaves; i++) {
    //                 float sampleX = (x - halfWidth) / perlinNoiseSettings.scale * frequency + octaveOffsets[i].x;
    //                 float sampleY = (y - halfHeight) / perlinNoiseSettings.scale * frequency + octaveOffsets[i].y;
    //
    //                 float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
    //                 noiseHeight += perlinValue * amplitude;
    //
    //                 amplitude *= perlinNoiseSettings.persistance;
    //                 frequency *= perlinNoiseSettings.lacunarity;
    //             }
    //
    //             if (noiseHeight > maxNoiseHeight) {
    //                 maxNoiseHeight = noiseHeight;
    //             } else if (noiseHeight < minNoiseHeight) {
    //                 minNoiseHeight = noiseHeight;
    //             }
    //             noiseMap [x, y] = noiseHeight;
    //         }
    //     }
    //
    //     for (int y = 0; y < mapHeight; y++) {
    //         for (int x = 0; x < mapWidth; x++) {
    //             noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
    //         }
    //     }
    //
    //     return noiseMap;
    // }
    public static float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight, int octaves, float scale, float persistence, float lacunarity) {
        float[,] noiseMap = new float[mapWidth,mapHeight];
        
        System.Random prng = new System.Random(Random.Range(0, 10000));

        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next (-100000, 100000);
            float offsetY = prng.Next (-100000, 100000);
            octaveOffsets [i] = new Vector2 (offsetX, offsetY);
        }

        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth * 0.5f;
        float halfHeight = mapHeight * 0.5f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
		
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                    //float perlinValue = PerlinNoise2D(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap [x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
            }
        }

        return noiseMap;
    }
    
    public static float[,] FNGenerateNoiseMap(int w, int h, int x, int y)
    {
        float[,] noiseMap = new float[w, h];
        
        FastNoise fn = new FastNoise();

        fn.SetNoiseType(noiseType);
        fn.SetSeed(seed);
        fn.SetFrequency(frequency);
        fn.SetInterp(interp);
        
        //Fractal Settings
        fn.SetFractalGain(gain);
        fn.SetFractalLacunarity(lacunarity);
        fn.SetFractalOctaves(octaves);
        fn.SetFractalType(fractalType);
        
        //Cellular Settings
        fn.SetCellularDistance2Indicies(cellularDistanceIndex0, cellularDistanceIndex1);
        fn.SetCellularDistanceFunction(cellularDistanceFunction);
        fn.SetCellularJitter(cellularJitter);
        fn.SetCellularReturnType(cellularReturnType);
        
        fn.SetGradientPerturbAmp(gradientPerturbAmp);

        for (int _x = 0; _x < w; _x++)
        {
            for (int _y = 0; _y < h; _y++)
            {
                noiseMap[_x, _y] = fn.GetNoise(_x + x, _y + y);
            }
        }

        return noiseMap;
    }
    
    public static float[,,] FNGenerateNoiseMap(int w, int h, int d, int x, int y, int z)
    {
        float[,,] noiseMap = new float[w, h, d];
        
        FastNoise fn = new FastNoise();

        fn.SetNoiseType(noiseType);
        fn.SetSeed(seed);
        fn.SetFrequency(frequency);
        fn.SetInterp(interp);
        
        //Fractal Settings
        fn.SetFractalGain(gain);
        fn.SetFractalLacunarity(lacunarity);
        fn.SetFractalOctaves(octaves);
        fn.SetFractalType(fractalType);
        
        //Cellular Settings
        fn.SetCellularDistance2Indicies(cellularDistanceIndex0, cellularDistanceIndex1);
        fn.SetCellularDistanceFunction(cellularDistanceFunction);
        fn.SetCellularJitter(cellularJitter);
        fn.SetCellularReturnType(cellularReturnType);
        
        fn.SetGradientPerturbAmp(gradientPerturbAmp);

        for (int _x = 0; _x < w; _x++)
        {
            for (int _y = 0; _y < h; _y++)
            {
                for (int _z = 0; _z < d; _z++)
                {
                    noiseMap[_x, _y, _z] = fn.GetNoise(_x + x, _y + y, _z + z);
                }
            }
        }
        
        return noiseMap;
    }
}

[System.Serializable]
public struct PerlinNoiseSettings
{
    public int octaves;
    public float frequency;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public FastNoise.Interp interp;
    public PerlinNoiseSettings(int _octaves, float _frequency, float _persistance, float _lacunarity, FastNoise.Interp _interp)
    {
        octaves = _octaves;
        frequency = _frequency;
        persistance = _persistance;
        lacunarity = _lacunarity;
        interp = _interp;
    }
}
public struct SimplexNoiseSettings
{
    public int octaves;
    public float frequency;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public SimplexNoiseSettings(int _octaves, float _frequency, float _persistance, float _lacunarity)
    {
        octaves = _octaves;
        frequency = _frequency;
        persistance = _persistance;
        lacunarity = _lacunarity;
    }
}
public struct CellularNoiseSettings
{
    public int octaves;
    public float frequency;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public CellularNoiseSettings(int _octaves, float _frequency, float _persistance, float _lacunarity)
    {
        octaves = _octaves;
        frequency = _frequency;
        persistance = _persistance;
        lacunarity = _lacunarity;
    }
}


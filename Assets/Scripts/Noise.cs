using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Noise
{
    public static PerlinNoiseSettings perlinNoiseSettings = new PerlinNoiseSettings(1, 80f, 0.5f, 1f);
    public static int seed;
    
    public static float UnityPerlinNoise2D(float x, float y, System.Random prng)
    {
        Vector2[] octaveOffsets = new Vector2[perlinNoiseSettings.octaves];

        //System.Random prng = new System.Random(seed);
        prng = new System.Random(seed);

        for (int i = 0; i < perlinNoiseSettings.octaves; i++) {
            float offsetX = prng.Next (-100000, 100000);
            float offsetY = prng.Next (-100000, 100000);
            octaveOffsets [i] = new Vector2 (offsetX, offsetY);
        }
        
        if (perlinNoiseSettings.scale <= 0)
        {
            perlinNoiseSettings.scale = 0.0001f;
        }

        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < perlinNoiseSettings.octaves; i++)
        {
            float sampleX = x / perlinNoiseSettings.scale * frequency + octaveOffsets[i].x;
            float sampleY = y / perlinNoiseSettings.scale * frequency + octaveOffsets[i].y;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= perlinNoiseSettings.persistance;
            frequency *= perlinNoiseSettings.lacunarity;
        }

        return noiseHeight;
    }
    public static float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight) {
        float[,] noiseMap = new float[mapWidth,mapHeight];
        
        System.Random prng = new System.Random(seed);

        
        Vector2[] octaveOffsets = new Vector2[perlinNoiseSettings.octaves];
        for (int i = 0; i < perlinNoiseSettings.octaves; i++) {
            float offsetX = prng.Next (-100000, 100000);
            float offsetY = prng.Next (-100000, 100000);
            octaveOffsets [i] = new Vector2 (offsetX, offsetY);
        }

        if (perlinNoiseSettings.scale <= 0) {
            perlinNoiseSettings.scale = 0.0001f;
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

                for (int i = 0; i < perlinNoiseSettings.octaves; i++) {
                    float sampleX = (x - halfWidth) / perlinNoiseSettings.scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / perlinNoiseSettings.scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= perlinNoiseSettings.persistance;
                    frequency *= perlinNoiseSettings.lacunarity;
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
}

[System.Serializable]
public struct PerlinNoiseSettings
{
    public int octaves;
    public float scale;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public PerlinNoiseSettings(int _octaves, float _scale, float _persistance, float _lacunarity)
    {
        octaves = _octaves;
        scale = _scale;
        persistance = _persistance;
        lacunarity = _lacunarity;
    }
    
    public void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
        
    }
}


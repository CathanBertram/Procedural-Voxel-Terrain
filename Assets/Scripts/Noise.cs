using System.Diagnostics;
using UnityEngine;

public class Noise : MonoBehaviour
{
    private static Noise instance;
    public static Noise Instance => instance;
    public int seed;

    public PerlinNoiseSettings perlinNoiseSettings;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    private void OnValidate()
    {
        perlinNoiseSettings.OnValidate();
    }

    public float PerlinNoise2D(float x, float y)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[perlinNoiseSettings.octaves];
        
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
    
    public float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight) {
        float[,] noiseMap = new float[mapWidth,mapHeight];

        System.Random prng = new System.Random (seed);
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
}

[System.Serializable]
public struct PerlinNoiseSettings
{
    public int octaves;
    public float scale;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
        
    }
}

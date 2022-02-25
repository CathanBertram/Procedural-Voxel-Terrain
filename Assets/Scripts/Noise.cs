using System;
using System.Linq;
using UnityEngine;
public static class Noise
{
    public static PerlinNoiseSettings perlinNoiseSettings = new PerlinNoiseSettings(1, 80f, 0.5f, 1f);
    public static int seed;
    static Noise()
    {
        CalculatePermutation(out _permutation);
        CalculateGradients(out _gradients);
    }
    

    public static float UnityPerlinNoise2D(float x, float y)
    {
        Vector2[] octaveOffsets = new Vector2[perlinNoiseSettings.octaves];

        System.Random prng = new System.Random(seed);

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
        
        System.Random prng = new System.Random(seed);

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

                    //float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                    float perlinValue = PerlinNoise2D(sampleX, sampleY) * 2 - 1;
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
    
    #region PerlinNoise
    private static int[] _permutation;

    private static Vector2[] _gradients;
    
    private static void CalculatePermutation(out int[] p)
    {
        System.Random prng = new System.Random(seed);

        p = Enumerable.Range(0, 256).ToArray();

        /// shuffle the array
        for (var i = 0; i < p.Length; i++)
        {
            var source = prng.Next(p.Length);

            var t = p[i];
            p[i] = p[source];
            p[source] = t;
        }
    }

    /// <summary>
    /// generate a new permutation.
    /// </summary>
    public static void Reseed()
    {
        CalculatePermutation(out _permutation);
    }

    private static void CalculateGradients(out Vector2[] grad)
    {
        System.Random prng = new System.Random(seed);
        grad = new Vector2[256];

        for (var i = 0; i < grad.Length; i++)
        {
            Vector2 gradient;

            do
            {
                gradient = new Vector2((float)(prng.Next() * 2 - 1), (float)(prng.Next() * 2 - 1));
            }
            while (gradient.SqrMagnitude() >= 1);

            gradient.Normalize();

            grad[i] = gradient;
        }

    }

    private static float Drop(float t)
    {
        t = Math.Abs(t);
        return 1f - t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Q(float u, float v)
    {
        return Drop(u) * Drop(v);
    }

    public static float PerlinNoise2D(float x, float y)
    {
        var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

        var total = 0f;

        var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

        foreach (var n in corners)
        {
            var ij = cell + n;
            var uv = new Vector2(x - ij.x, y - ij.y);

            var index = _permutation[(int)Mathf.Abs(ij.x) % _permutation.Length];
            index = _permutation[(index + (int)Mathf.Abs(ij.y)) % _permutation.Length];

            var grad = _gradients[index % _gradients.Length];

            total += Q(uv.x, uv.y) * Vector2.Dot(grad, uv);
        }

        return Math.Max(Math.Min(total, 1f), -1f);
    }
    public static float OctavePerlinNoise2D(float x, float y)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[perlinNoiseSettings.octaves];
        Reseed();
        //prng = new System.Random(1);

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

            float perlinValue = PerlinNoise2D(sampleX, sampleY) ;
            noiseHeight += perlinValue * amplitude;

            amplitude *= perlinNoiseSettings.persistance;
            frequency *= perlinNoiseSettings.lacunarity;
        }

        return noiseHeight;
    }
    #endregion
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


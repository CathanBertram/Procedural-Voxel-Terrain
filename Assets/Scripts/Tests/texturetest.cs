using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using UnityEngine;
using UnityEngine.UI;

public class texturetest : MonoBehaviour
{
    [SerializeField] private Renderer r;
    private Texture2D texture;
    [SerializeField] private int textureSize;
    public int width;
    public int height;
    public void DrawNoiseMap(float[,] noiseMap) {
        int width = noiseMap.GetLength (0);
        int height = noiseMap.GetLength (1);

        Texture2D texture = new Texture2D (width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, noiseMap [x, y]);
            }
        }
        texture.SetPixels (colourMap);
        texture.Apply ();

        r.sharedMaterial.mainTexture = texture;
        r.transform.localScale = new Vector3 (10, 1, 10);
    }

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
    public void Generate()
    {
        FastNoise fn = new FastNoise();

        fn.SetNoiseType(noiseType);
        fn.SetSeed(seed);
        fn.SetFrequency(frequency);
        fn.SetInterp(interp);
        fn.SetCellularJitter(cellularJitter);
        fn.SetFractalGain(gain);
        fn.SetFractalLacunarity(lacunarity);
        fn.SetFractalOctaves(octaves);
        fn.SetFractalType(fractalType);
        fn.SetCellularDistance2Indicies(cellularDistanceIndex0, cellularDistanceIndex1);
        fn.SetCellularDistanceFunction(cellularDistanceFunction);
        fn.SetGradientPerturbAmp(gradientPerturbAmp);
        fn.SetCellularReturnType(cellularReturnType);
        
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMap[x, y] = fn.GetNoise(x, y);
            }
        }

        DrawNoiseMap(noiseMap);
    }
    
    
}

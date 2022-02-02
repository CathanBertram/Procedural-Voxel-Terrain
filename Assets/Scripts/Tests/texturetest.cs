using System.Collections.Generic;
using UnityEngine;

public class texturetest : MonoBehaviour
{
    [SerializeField] private List<Texture2D> sprites;
    [SerializeField] private Renderer r;
    private Texture2D texture;
    [SerializeField] private int seed;
    [SerializeField] private int octaves;
    [SerializeField] private float scale;
    [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    private void Start()
    {
        texture = new Texture2D(128, 128);

        r.material.mainTexture = texture;
        
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                Noise.Instance.PerlinNoise2D(x, y);
                texture.SetPixel(x, y, new Color());
            }
        }
        texture.Apply();
    }

    private void Update()
    {
        DrawNoiseMap(Noise.Instance.GeneratePerlinNoiseMap(256,256));
    }
    
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
        r.transform.localScale = new Vector3 (width, 1, height);
    }
}

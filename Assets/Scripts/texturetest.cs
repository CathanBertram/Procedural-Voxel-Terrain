using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texturetest : MonoBehaviour
{
    [SerializeField] private List<Texture2D> sprites;
    [SerializeField] private Renderer renderer;
    private void Start()
    {
        int textureWidth = sprites[0].width;
        int textureHeight = sprites[0].height;
        Texture2D texture = new Texture2D(textureWidth * sprites.Count, textureHeight);
        renderer.material.mainTexture = texture;
        for (int i = 0; i < sprites.Count; i++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                for (int x = 0; x < textureWidth; x++)
                {
                    texture.SetPixel(x + (i * textureWidth), y,sprites[i].GetPixel(x, y));
                }
            }
        }

        texture.Apply();
    }
}

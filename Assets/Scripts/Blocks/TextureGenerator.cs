using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Voxel;

namespace Blocks
{
    public static class TextureGenerator
    {
        private static int textureWidth;
        private static int textureHeight;
        private static int index;
        public static Texture2D Generate(BlockData[] blockData)
        {
            textureWidth = blockData[1].backTexture.width;
            textureHeight = blockData[1].backTexture.height;
            index = 0;
            
            Dictionary<Texture2D, int> addedTextures = new Dictionary<Texture2D, int>();
            
            foreach (var data in blockData)
            {
                AddTexture(data.backTexture, addedTextures, 0, data);
                AddTexture(data.frontTexture, addedTextures, 1, data);
                AddTexture(data.topTexture, addedTextures, 2, data);
                AddTexture(data.bottomTexture, addedTextures, 3, data);
                AddTexture(data.leftTexture, addedTextures, 4, data);
                AddTexture(data.rightTexture, addedTextures, 5, data);
            }

            VoxelData.textureAtlasSize = index;
            
            Texture2D texture = new Texture2D(addedTextures.Count * textureWidth, textureHeight);
            int i = 0;
            foreach (var item in addedTextures)
            {
                //texture.SetPixels(i * textureWidth, 0, textureWidth, textureHeight, item.Key.GetPixels());

                for (int y = 0; y < textureHeight; y++)
                {
                    for (int x = 0; x < textureWidth; x++)
                    {
                        texture.SetPixel(x + (i * textureWidth), y,item.Key.GetPixel(x, y));
                    }
                }
                
                i++;
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            texture.Apply();
            return texture;
        }

        private static void AddTexture(Texture2D textureToAdd, Dictionary<Texture2D, int> addedTextures, int faceIndex, BlockData block)
        {
            if (textureToAdd == null)
                return;
            
            if (addedTextures.ContainsKey(textureToAdd))
                block.SetTextureID(faceIndex, addedTextures[textureToAdd]);
            else
            {
                addedTextures.Add(textureToAdd, index);
                block.SetTextureID(faceIndex, addedTextures[textureToAdd]);
                index++;
            }
        }
    }
}

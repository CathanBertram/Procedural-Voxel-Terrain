using System;
using UnityEngine;

namespace Blocks
{
    [System.Serializable]
    public class BlockData
    {
        [SerializeField] private string blockName;
        public string BlockName => blockName;
        [SerializeField] private bool isSolid;
        public bool IsSolid => isSolid;
        
        [Header("Texture Values")] 
        public Texture2D backTexture;
        public Texture2D frontTexture;
        public Texture2D topTexture;
        public Texture2D bottomTexture; 
        public Texture2D leftTexture;
        public Texture2D rightTexture;
        
        [Header("Texture Values")] 
        private int backFace;
        private int frontFace;
        private int topFace;
        private int bottomFace;
        private int leftFace;
        private int rightFace;
        // Back Front Top Bottom Left Right
        public int GetTextureID(int faceIndex)
        {
            switch (faceIndex)
            {
                case 0: return backFace;
                case 1: return frontFace;
                case 2: return topFace;
                case 3: return bottomFace;
                case 4: return leftFace;
                case 5: return rightFace;
                default:
                    throw new NotImplementedException();
            }
        }

        public void SetTextureID(int faceIndex, int textureIndex)
        {
            switch (faceIndex)
            {
                case 0: backFace = textureIndex; break;
                case 1: frontFace = textureIndex; break;
                case 2: topFace = textureIndex; break;
                case 3: bottomFace = textureIndex; break;
                case 4: leftFace = textureIndex; break;
                case 5: rightFace = textureIndex; break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

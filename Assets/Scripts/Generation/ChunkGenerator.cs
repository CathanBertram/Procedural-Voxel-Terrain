using System;
using UnityEngine;

namespace Generation
{
    public class ChunkGenerator : MonoBehaviour
    {
        [SerializeField] private Material chunkMaterial;
        [SerializeField] private BlockType[] blockTypes;
        public BlockType[] BlockTypes => blockTypes;
        private static ChunkGenerator instance;

        public static ChunkGenerator Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            } else {
                instance = this;
            }
        }
    }

    [System.Serializable]
    public class BlockType
    {
        [SerializeField] private string blockName;
        [SerializeField] private bool isSolid;
        public bool IsSolid => isSolid;

        [Header("Texture Values")] 
        [SerializeField] private int backFace;
        [SerializeField] private int frontFace;
        [SerializeField] private int topFace;
        [SerializeField] private int bottomFace;
        [SerializeField] private int leftFace;
        [SerializeField] private int rightFace;
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
                    return 0;
            }
        }
    }
}

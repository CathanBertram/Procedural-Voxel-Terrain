using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class BlockDatabase : MonoBehaviour
    {
        private static BlockDatabase instance;
        public static BlockDatabase Instance => instance;
        
        [SerializeField] private Material blockMaterial;

        [SerializeField] private BlockData[] blockData;
        private Dictionary<string, byte> blockDataDictionary = new Dictionary<string, byte>();
        public Dictionary<string, byte> BlockDataDictionary => blockDataDictionary;
        public BlockData[] BlockData => blockData;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
            
            for (byte i = 0; i < blockData.Length; i++)
            {
                blockDataDictionary.Add(blockData[i].BlockName, i);
            }

            blockMaterial.mainTexture = TextureGenerator.Generate(blockData);
        }

        public byte GetBlockID(string blockName)
        {
            return blockDataDictionary[blockName];
        }

        public bool GetIsSolid(byte blockID)
        {
            return blockData[blockID].IsSolid;
        }
    }
}
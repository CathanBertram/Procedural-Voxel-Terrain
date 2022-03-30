using System.Collections.Generic;
using UnityEngine;
using Voxel;

namespace Blocks
{
    public class BlockDatabase : MonoBehaviour
    {
        private static BlockDatabase instance;
        public static BlockDatabase Instance => instance;
        
        [SerializeField] private Material blockMaterial;

        [SerializeField] private BlockData[] blockData;
        [SerializeField] private List<Biome> biomes;
        public List<Biome> Biomes => biomes;
        
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

        public Biome GetBiome(int id)
        {
            if (id > biomes.Count) return biomes[0];

            return biomes[id];
        }
        
        public byte GetBlockID(string blockName)
        {
            if (!blockDataDictionary.ContainsKey(blockName))
            {
                Debug.unityLogger.LogError($"Block {blockName} does not exist", this);
                return 0;
            }
            
            return blockDataDictionary[blockName];
        }

        public bool GetIsSolid(byte blockID)
        {
            if (blockID == VoxelData.airID)
                return false;
            
            return blockData[blockID].IsSolid;
        }
    }
}

using Blocks;
using UnityEngine;
using Voxel;

namespace Generation
{
    public static class ChunkGenerator
    {
        public static byte[,,] GenerateVoxelMap(int xPos, int zPos)
        {
            // for (int y = 0; y < VoxelData.chunkHeight; y++)
            // {
            //     for (int x = 0; x < VoxelData.chunkWidth; x++)
            //     {
            //         for (int z = 0; z < VoxelData.chunkWidth; z++)
            //         {
            //             if (y == VoxelData.chunkHeight - 1)
            //                 voxelMap[x, y, z] = ChunkGenerator.Instance.BlockTypeDictionary["Grass"];
            //             else if (y >= VoxelData.chunkHeight - 3)
            //                 voxelMap[x, y, z] = ChunkGenerator.Instance.BlockTypeDictionary["Dirt"];
            //             else
            //                 voxelMap[x, y, z] = ChunkGenerator.Instance.BlockTypeDictionary["Stone"];
            //         }
            //     }
            // }
            
            byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];
            
            //Generate Data
            var yPos = VoxelData.seaLevel;
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    voxelMap[x, yPos, z] = BlockDatabase.Instance.GetBlockID("Grass");
                    var dirtLayerEnd = yPos - Random.Range(2,5);
                    for (int i = yPos - 1; i >= 0; i--)
                    {
                        if (i > dirtLayerEnd)
                        {
                            voxelMap[x, i, z] = BlockDatabase.Instance.GetBlockID("Dirt");
                        }
                        else
                        {
                            voxelMap[x, i, z] = BlockDatabase.Instance.GetBlockID("Stone");
                        }
                    }
                }
            }

            return voxelMap;
        }
    }
}

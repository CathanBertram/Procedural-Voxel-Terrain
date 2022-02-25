using Blocks;
using UnityEngine;
using Voxel;

namespace Generation
{
    public static class ChunkGenerator
    {
        public static byte[,,] GenerateVoxelMap(float xPos, float zPos)
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
            System.Random random = new System.Random(Noise.seed);
            
            byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];
            
            //Generate Data
            var yPos = VoxelData.seaLevel;
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    //yPos = VoxelData.seaLevel + Mathf.RoundToInt(Mathf.Lerp(0, 15, Noise.Instance.PerlinNoise2D(xPos + x, zPos + z)));
                    //yPos = VoxelData.seaLevel + Mathf.RoundToInt(Noise.Instance.PerlinNoise2D(xPos + x, zPos + z) * 4);
                    yPos = VoxelData.seaLevel + Mathf.RoundToInt(Easing.EaseInOutQuint(0, 30, 0.5f * (1 + Noise.UnityPerlinNoise2D(xPos + x, zPos + z))));
                    
                    voxelMap[x, yPos, z] = BlockDatabase.Instance.GetBlockID("Grass");
                    var dirtLayerEnd = yPos - random.Next(2, 5);
                    for (int i = yPos - 1; i >= 0; i--)
                    {
                        if (i > dirtLayerEnd)
                        {
                            voxelMap[x, i, z] = BlockDatabase.Instance.GetBlockID("Dirt");
                        }
                        else if (i == 0)
                        {
                            voxelMap[x, i, z] = BlockDatabase.Instance.GetBlockID("Bedrock");
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

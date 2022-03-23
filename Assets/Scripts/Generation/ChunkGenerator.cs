using System.Collections.Generic;
using Blocks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Voxel;

namespace Generation
{
    public static class ChunkGenerator
    {
        public static EasingType easingType;
        public static int lowerBound;
        public static int upperBound;
        public static float noiseCaveThreshold;
        public static byte[,,] GenerateVoxelMap(int xPos, int zPos)
        {
            //Seed Random for all random operations
            System.Random random = new System.Random(Noise.Seed);
#region Empty
            //Create new byte[,,] for voxel map
            byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];
#endregion

#region Surface and Fill
            //Generate a 2D noise map
            float[,] noiseMap = Noise.FNGenerateNoiseMap(VoxelData.chunkWidth, VoxelData.chunkWidth, Mathf.FloorToInt(xPos), Mathf.FloorToInt(zPos));

            //Loop through x column
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                //Loop through z column
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    //Get y position from noiseMap using an easing function
                    var yPos = VoxelData.seaLevel + Mathf.RoundToInt(Easing.Ease(easingType,lowerBound, upperBound, 0.5f * (1 + noiseMap[x,z])));
                    
                    //Set top layer to grass
                    //TODO replace with biome block
                    voxelMap[x, yPos, z] = BlockDatabase.Instance.GetBlockID("Grass");
                    
                    //Calculate how many layers of dirt there are
                    //TODO replace with biome specific data
                    var dirtLayerEnd = yPos - random.Next(2, 5);
                    //Loop through yPos and set block data
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
#endregion

#region Carve
            //Generate 3D noise map for caves
            float[,,] caveMap = Noise.FNGenerateNoiseMap(VoxelData.chunkWidth, VoxelData.chunkHeight,
                VoxelData.chunkWidth, Mathf.FloorToInt(xPos), 0, Mathf.FloorToInt(zPos));
            
            //Loop through xPos
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                //Loop through yPos
                for (int y = 0; y < VoxelData.chunkHeight; y++)
                {
                    //Loop through zPos
                    for (int z = 0; z < VoxelData.chunkWidth; z++)
                    {
                        //Get noise from caveMap and check if value is below threshold
                        //If it is, set block to air
                        if (0.5f * (caveMap[x,y,z] + 1) < noiseCaveThreshold)
                        {
                            voxelMap[x, y, z] = VoxelData.airID;
                        }
                    }
                }   
            }
#endregion

            Dictionary<Vector2Int, AdditionalChunkData> additionalChunkData =
                new Dictionary<Vector2Int, AdditionalChunkData>();
            Vector3Int testCenter = new Vector3Int(12, 100, 13);
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int z = 0; z < 8; z++)
                    {
                        var tempPos = testCenter;
                        tempPos.x += x;
                        tempPos.z += z;
                        if (tempPos.x < 0 || tempPos.x > VoxelData.chunkWidth - 1 || tempPos.y < 0 || tempPos.y > VoxelData.chunkHeight - 1 
                            || tempPos.z < 0 || tempPos.z > VoxelData.chunkWidth - 1)
                        {
                            var chunkX = Mathf.FloorToInt((float)(testCenter.x + x) / (float)VoxelData.chunkWidth);
                            var chunkZ = Mathf.FloorToInt((float)(testCenter.z + z) / (float)VoxelData.chunkWidth);

                            chunkX += xPos / VoxelData.chunkWidth;
                            chunkZ += zPos / VoxelData.chunkWidth;
             
                            Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);
                            
                            if(!additionalChunkData.ContainsKey(chunkPos))
                                additionalChunkData.Add(chunkPos, new AdditionalChunkData(chunkPos));

                            BlockData data = new BlockData()
                            {
                                x = (testCenter.x + x) % VoxelData.chunkWidth,
                                //x = ((temp.x % VoxelData.chunkWidth) + VoxelData.chunkWidth) % VoxelData.chunkWidth,
                                y = testCenter.y + y,
                                z = (testCenter.z + z) % VoxelData.chunkWidth,
                                //z = ((temp.z % VoxelData.chunkWidth) + VoxelData.chunkWidth) % VoxelData.chunkWidth,
                                blockID = 5
                            };
                            additionalChunkData[chunkPos].blockData.Add(data);
                        }
                        else
                        {
                            voxelMap[x + testCenter.x, y + testCenter.y, z + testCenter.z] = 4;
                        }
                    }
                }
            }
            
            
#region Bottom Layers

            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    voxelMap[x, 0, z] = BlockDatabase.Instance.GetBlockID("Bedrock");
                }
            }
#endregion

            foreach (var data in additionalChunkData)
            {
                WorldLoader.Instance.AddAdditionalData(data.Key, data.Value);
            }
            
            return voxelMap;
        }
    }
}

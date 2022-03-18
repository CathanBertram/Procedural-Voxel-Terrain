using Blocks;
using Unity.Mathematics;
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
        public static byte[,,] GenerateVoxelMap(float xPos, float zPos)
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

#region Bottom Layers

            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    voxelMap[x, 0, z] = BlockDatabase.Instance.GetBlockID("Bedrock");
                }
            }
#endregion
            return voxelMap;
        }
    }
}

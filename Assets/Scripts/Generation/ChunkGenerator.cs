using System;
using System.Collections.Generic;
using System.Threading;
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
        public static int maxTreesPerChunk;
        public static int maxTreeAttempts;
        public static int treeSpawnChance;
        
        public static byte[,,] GenerateVoxelMap(int xPos, int zPos)
        {
            //Seed Random for all random operations
            System.Random random = new System.Random(Noise.Seed + xPos + zPos);
#region PickBiome
            var biomeNoise = 0.5 * (1 + Noise.FNGenerateNoise(xPos, zPos));
            var biomeRange = 1f / BlockDatabase.Instance.Biomes.Count;
            float temp = 0;
            int count = 0;
            Biome biome = BlockDatabase.Instance.Biomes[0];
            do
            {
                temp += biomeRange;

                if (biomeNoise < temp)
                {
                    biome = BlockDatabase.Instance.Biomes[count];
                    break;
                }

                count++;
            } while (temp < 1);
#endregion
#region Empty
            //Create new byte[,,] for voxel map
            byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];
#endregion

#region Surface and Fill
            //Generate a 2D noise map
            float[,] noiseMap = Noise.FNGenerateNoiseMap(VoxelData.chunkWidth, VoxelData.chunkWidth, Mathf.FloorToInt(xPos), Mathf.FloorToInt(zPos), biome.biomeNoiseSettings);

            //Loop through x column
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                //Loop through z column
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    //Get y position from noiseMap using an easing function
                    var yPos = VoxelData.seaLevel + Mathf.RoundToInt(Easing.Ease(biome.easingType,biome.lowerBound, biome.upperBound, 0.5f * (1 + noiseMap[x,z])));
                    //Set top layer to grass
                    //TODO replace with biome block
                    
                    //Calculate how many layers of dirt there are
                    var primaryLayerEnd = yPos - random.Next(biome.primaryTopLayerBlockRange.x,
                        biome.primaryTopLayerBlockRange.y);
                    var secondaryLayerEnd = yPos - random.Next(biome.secondaryTopLayerBlockRange.x, biome.secondaryTopLayerBlockRange.y);
                    //Loop through yPos and set block data
                    for (int i = yPos; i >= 0; i--)
                    {
                        if (i > primaryLayerEnd)
                        {
                            voxelMap[x, yPos, z] = BlockDatabase.Instance.GetBlockID(biome.primaryTopLayerBlock);
                        }
                        else if (i > secondaryLayerEnd)
                        {
                            voxelMap[x, i, z] = BlockDatabase.Instance.GetBlockID(biome.secondaryTopLayerBlock);
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

#region GenerateTrees
            Dictionary<Vector2Int, AdditionalChunkData> additionalChunkData =
                new Dictionary<Vector2Int, AdditionalChunkData>();
            List<Point> points = new List<Point>();
            for (int i = 0; i < biome.maxFeaturesPerChunk; i++)
            {
                for (int j = 0; j < biome.maxFeatureAttempts; j++)
                {
                    //Get pos 1 block in from chunk borders
                    var randPos = GetRandomVec2Int(random, 1, VoxelData.chunkWidth - 2);
                    bool useablePoint = true;
                    foreach (var point in points)
                    {
                        if (Vector2Int.Distance(point.position, randPos) < point.radius)
                            useablePoint = false;
                    }

                    if (!useablePoint) continue;

                    if (random.Next(0, 100) > biome.featureSpawnChance) continue;

                    var treeData = FeatureGenerator.GenerateFeature(biome, xPos, zPos, randPos.x, randPos.y, i);
                    points.Add(new Point(treeData.radius, randPos));

                    int yPos = yPos = VoxelData.seaLevel + Mathf.RoundToInt(Easing.Ease(biome.easingType, biome.lowerBound,
                        biome.upperBound, 0.5f * (1 + noiseMap[randPos.x, randPos.y]))) + 1;
                    
                    if (voxelMap[randPos.x, yPos - 1, randPos.y] == VoxelData.airID) continue;
                    
                    var startPos = new Vector3Int(randPos.x, yPos, randPos.y);
                    foreach (var kvp in treeData.featureData)
                    {
                        var pos = kvp.Key + startPos;
                
                        if (pos.x < 0 || pos.x > VoxelData.chunkWidth - 1 || pos.y < 0 || pos.y > VoxelData.chunkHeight - 1 
                            || pos.z < 0 || pos.z > VoxelData.chunkWidth - 1)
                        {
                            var chunkX = Mathf.FloorToInt((float)(pos.x) / VoxelData.chunkWidth);
                            var chunkZ = Mathf.FloorToInt((float)(pos.z) / VoxelData.chunkWidth);
                
                            chunkX += xPos / VoxelData.chunkWidth;
                            chunkZ += zPos / VoxelData.chunkWidth;
                
                            Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);
                    
                            if(!additionalChunkData.ContainsKey(chunkPos))
                                additionalChunkData.Add(chunkPos, new AdditionalChunkData(chunkPos));
                            
                            BlockData data = new BlockData()
                            {
                                x = (pos.x + VoxelData.chunkWidth) % VoxelData.chunkWidth,
                                y = (pos.y + VoxelData.chunkHeight) % VoxelData.chunkHeight,
                                z = (pos.z + VoxelData.chunkWidth) % VoxelData.chunkWidth,
                                blockID = kvp.Value
                            };
                            additionalChunkData[chunkPos].blockData.Add(data);
                        }
                        else
                            voxelMap[pos.x, pos.y, pos.z] = kvp.Value;
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

            foreach (var data in additionalChunkData)
            {
                WorldLoader.Instance.AddAdditionalData(data.Key, data.Value);
            }
            
            return voxelMap;
        }

        private static Vector2Int GetRandomVec2Int(System.Random random, int min, int max)
        {
            var x = random.Next(min, max);
            var y = random.Next(min, max);

            return new Vector2Int(x, y);
        }

        private struct Point
        {
            public int radius;
            public Vector2Int position;

            public Point(int _radius, Vector2Int _position)
            {
                radius = _radius;
                position = _position;
            }
        }

        private struct HeightMapData
        {
            public float value;
            public Biome biome;
        }
    }
}

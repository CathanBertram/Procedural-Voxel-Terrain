using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class CaveGenerator
{
    public static HashSet<Vector3Int> GenerateCave(int chunkX, int chunkZ, int xPos, int yPos, int zPos)
    {
        System.Random random = new System.Random(Noise.Seed + chunkX * chunkX + chunkZ * chunkZ + xPos + yPos + zPos);
        var path = PathGenerator.PerlinWormPathGeneration(random, new Vector3Int(xPos + chunkX, yPos, zPos + chunkZ), 15);


        FastNoise fn = Noise.noiseSettings3D.CreateFastNoise();
        HashSet<Vector3Int> cave = new HashSet<Vector3Int>();
        for (int i = 0; i < path.Count; i++)
        {
            GetPointsInSphere(cave, path[i], Mathf.RoundToInt(Easing.Linear(0f, 10f, 0.5f * (1 + fn.GetNoise(path[i].x, path[i].y, path[i].z)))));
        }
        return cave;
    }
    public static HashSet<Vector3Int> GenerateCave(int seed, List<Vector3> path, NoiseSettings noiseSettings)
    {
        FastNoise fn = noiseSettings.CreateFastNoise();
        HashSet<Vector3Int> cave = new HashSet<Vector3Int>();
        for (int i = 0; i < path.Count; i++)
        {
            GetPointsInSphere(cave, path[i], Mathf.RoundToInt(Easing.Linear(0f, 10f, 0.5f * (1 + fn.GetNoise(path[i].x, path[i].y, path[i].z)))));
        }
        return cave;
    }
    public static void GetPointsInSphere(HashSet<Vector3Int> cave, Vector3 origin, int radius)
    {
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                for (int z = -radius; z < radius; z++)
                {
                    if ((x * x) + (y * y) + (z * z) > radius * radius) continue;
                    // var pos = new Vector3Int(Mathf.RoundToInt(origin.x) + x, Mathf.RoundToInt(origin.y) + y, Mathf.RoundToInt(origin.z) + z);
                    //if (!cave.Contains(pos))
                    cave.Add(new Vector3Int(Mathf.RoundToInt(origin.x) + x, Mathf.RoundToInt(origin.y) + y, Mathf.RoundToInt(origin.z) + z));
                }
            } 
        }
    }
}

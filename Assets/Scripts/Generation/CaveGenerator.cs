using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CaveGenerator
{
    public static List<Vector3Int> GenerateCave(int chunkX, int chunkZ, int xPos, int yPos, int zPos)
    {
        System.Random random = new System.Random(Noise.Seed + chunkX + chunkZ + xPos + yPos + zPos);
        var path = PathGenerator.DrunkZaxPathGeneration(random, new Vector3Int(xPos + chunkX, yPos, zPos + chunkZ), 1);

        List<Vector3Int> cave = new List<Vector3Int>();
        for (int i = 0; i < path.Count; i++)
        {
            GetPointsInSphere(cave, path[i], 3);
            // if (i == 0 || i == path.Count - 1)
            // {
            //     
            // }
        }
        
        return cave;
    }

    public static void GetPointsInSphere(List<Vector3Int> cave, Vector3Int origin, int radius)
    {
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                for (int z = -radius; z < radius; z++)
                {
                    if ((x * x) + (y * y) + (z * z) > radius * radius) continue;

                    Vector3Int pos = new Vector3Int(origin.x + x, origin.y + y, origin.z + z);
                    if (!cave.Contains(pos))
                        cave.Add(pos);
                    
                }
            } 
        }
    }
}

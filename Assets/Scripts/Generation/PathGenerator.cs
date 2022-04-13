using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum PathType
{
    DrunkRobot,
    DrunkZax,
    DrunkRobotEmu,
    DrunkLiberalEmu,
    DrunkModerateEmu,
    DrunkConservativeEmu,
    PerlinWorm
}
public static class PathGenerator
{
    public static int maxIterCount = 512;
    public static int maxDistance = 640;
    /// <summary>
    /// Picks a random direction with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3Int> DrunkRobotPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int curPos = Vector3Int.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;
        while (continuePath)
        {
            var dirInt = random.Next(0, 6);
            switch (dirInt)
            {
                case 0:
                    curPos += Vector3Int.up;
                    break;
                case 1:
                    curPos += Vector3Int.down;
                    break;
                case 2:
                    curPos += Vector3Int.back;
                    break;
                case 3:
                    curPos += Vector3Int.forward;
                    break;
                case 4:
                    curPos += Vector3Int.left;
                    break;
                case 5:
                    curPos += Vector3Int.right;
                    break;
                default:
                    curPos += Vector3Int.forward;
                    break;
            }
            
            path.Add(curPos);

            if (Vector3Int.Distance(curPos, Vector3Int.zero) > maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }

        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3Int>>(origin, path));
        
        return path;
    }
    /// <summary>
    /// Picks a random direction with each iteration, has a priority for a specific direction
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3Int> DrunkZaxPathGeneration(System.Random random, Vector3Int origin, int priority)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int curPos = Vector3Int.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        Vector3Int priorityDirection;
        switch (random.Next(0, 6))
        {
            case 0:
                priorityDirection = Vector3Int.up;
                break;
            case 1:
                priorityDirection = Vector3Int.down;
                break;
            case 2:
                priorityDirection = Vector3Int.back;
                break;
            case 3:
                priorityDirection = Vector3Int.forward;
                break;
            case 4:
                priorityDirection = Vector3Int.left;
                break;
            case 5:
                priorityDirection = Vector3Int.right;
                break;
            default:
                priorityDirection = Vector3Int.forward;
                break;
        }
        
        
        while (continuePath)
        {
            var dirInt = random.Next(0, 6 + priority);
            switch (dirInt)
            {
                case 0:
                    curPos += Vector3Int.up;
                    break;
                case 1:
                    curPos += Vector3Int.down;
                    break;
                case 2:
                    curPos += Vector3Int.back;
                    break;
                case 3:
                    curPos += Vector3Int.forward;
                    break;
                case 4:
                    curPos += Vector3Int.left;
                    break;
                case 5:
                    curPos += Vector3Int.right;
                    break;
                default:
                    curPos += priorityDirection;
                    break;
            }
            
            path.Add(curPos);

            if (Vector3Int.Distance(curPos, Vector3Int.zero) > maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }

        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3Int>>(origin, path));
        
        return path;
    }
    /// <summary>
    /// Picks a random direction with each iteration and will not go back on itself
    /// (if it went right in the previous iteration it will not go left in the current iteration)
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3Int> DrunkRobotEmuPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int curPos = Vector3Int.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        var prevDir = 0;
        
        while (continuePath)
        {
            var dirInt = random.Next(0, 6);
            while (dirInt == prevDir)
            {
                dirInt = random.Next(0, 6);
            }
            switch (dirInt)
            {
                case 0:
                    curPos += Vector3Int.up;
                    break;
                case 1:
                    curPos += Vector3Int.down;
                    break;
                case 2:
                    curPos += Vector3Int.back;
                    break;
                case 3:
                    curPos += Vector3Int.forward;
                    break;
                case 4:
                    curPos += Vector3Int.left;
                    break;
                case 5:
                    curPos += Vector3Int.right;
                    break;
                default:
                    curPos += Vector3Int.forward;
                    break;
            }

            prevDir = dirInt;
            path.Add(curPos);

            if (Vector3Int.Distance(curPos, Vector3Int.zero) > maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }

        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3Int>>(origin, path));
        
        return path;
    }
    
    /// <summary>
    /// Picks a random direction within 90 degrees with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3Int> DrunkLiberalEmuPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int curPos = Vector3Int.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        return path;
    }
    /// <summary>
    /// Picks a random direction within 60 degrees with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3Int> DrunkModerateEmuPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int curPos = Vector3Int.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        return path;
    }
    /// <summary>
    /// Picks a random direction within 30 degrees with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3Int> DrunkConservativeEmuPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int curPos = Vector3Int.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        return path;
    }
    /// <summary>
    /// Uses perlin noise to determine which direction to move in at each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3Int> PerlinWormPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int curPos = Vector3Int.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        return path;
    }
    /// <summary>
    /// Picks a random direction within angle degrees with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static List<Vector3Int> AngleConstrainedPathGeneration(System.Random random, Vector3Int origin, int angle)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int curPos = Vector3Int.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        return path;
    }
}

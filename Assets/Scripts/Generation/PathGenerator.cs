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
    PerlinWorm,
    AngleConstrained
}
public static class PathGenerator
{
    public static int maxIterCount = 512;
    public static int maxDistance = 144;
    /// <summary>
    /// Picks a random direction with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3> DrunkRobotPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;
        while (continuePath)
        {
            var dirInt = random.Next(0, 6);
            switch (dirInt)
            {
                case 0:
                    curPos += Vector3.up;
                    break;
                case 1:
                    curPos += Vector3.down;
                    break;
                case 2:
                    curPos += Vector3.back;
                    break;
                case 3:
                    curPos += Vector3.forward;
                    break;
                case 4:
                    curPos += Vector3.left;
                    break;
                case 5:
                    curPos += Vector3.right;
                    break;
                default:
                    curPos += Vector3.forward;
                    break;
            }
            
            path.Add(curPos);

            if (curPos.x * curPos.x + curPos.z + curPos.z > maxDistance * maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }

        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3>>(origin, path));
        
        return path;
    }
    /// <summary>
    /// Picks a random direction with each iteration, has a priority for a specific direction
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3> DrunkZaxPathGeneration(System.Random random, Vector3Int origin, int priority)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        Vector3 priorityDirection;
        switch (random.Next(0, 6))
        {
            case 0:
                priorityDirection = Vector3.up;
                break;
            case 1:
                priorityDirection = Vector3.down;
                break;
            case 2:
                priorityDirection = Vector3.back;
                break;
            case 3:
                priorityDirection = Vector3.forward;
                break;
            case 4:
                priorityDirection = Vector3.left;
                break;
            case 5:
                priorityDirection = Vector3.right;
                break;
            default:
                priorityDirection = Vector3.forward;
                break;
        }
        
        
        while (continuePath)
        {
            var dirInt = random.Next(0, 6 + priority);
            switch (dirInt)
            {
                case 0:
                    curPos += Vector3.up;
                    break;
                case 1:
                    curPos += Vector3.down;
                    break;
                case 2:
                    curPos += Vector3.back;
                    break;
                case 3:
                    curPos += Vector3.forward;
                    break;
                case 4:
                    curPos += Vector3.left;
                    break;
                case 5:
                    curPos += Vector3.right;
                    break;
                default:
                    curPos += priorityDirection;
                    break;
            }
            
            path.Add(curPos);

            if (curPos.x * curPos.x + curPos.z + curPos.z > maxDistance * maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }

        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3>>(origin, path));
        
        return path;
    }
    /// <summary>
    /// Picks a random direction with each iteration and will not go back on itself
    /// (if it went right in the previous iteration it will not go left in the current iteration)
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3> DrunkRobotEmuPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
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
                    curPos += Vector3.up;
                    break;
                case 1:
                    curPos += Vector3.down;
                    break;
                case 2:
                    curPos += Vector3.back;
                    break;
                case 3:
                    curPos += Vector3.forward;
                    break;
                case 4:
                    curPos += Vector3.left;
                    break;
                case 5:
                    curPos += Vector3.right;
                    break;
                default:
                    curPos += Vector3.forward;
                    break;
            }

            prevDir = dirInt;
            path.Add(curPos);

            if (curPos.x * curPos.x + curPos.z + curPos.z > maxDistance * maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }

        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3>>(origin, path));
        
        return path;
    }
    
    /// <summary>
    /// Picks a random direction within 90 degrees with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3> DrunkLiberalEmuPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;
        
        return AngleConstrainedPathGeneration(random, origin, 90);
    }
    /// <summary>
    /// Picks a random direction within 60 degrees with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3> DrunkModerateEmuPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        return AngleConstrainedPathGeneration(random, origin, 60);

    }
    /// <summary>
    /// Picks a random direction within 30 degrees with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3> DrunkConservativeEmuPathGeneration(System.Random random, Vector3Int origin)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        return AngleConstrainedPathGeneration(random, origin, 30);
    }
    /// <summary>
    /// Uses perlin noise to determine which direction to move in at each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Vector3> PerlinWormPathGeneration(System.Random random, Vector3Int origin, int angleConstraint)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        var fn = Noise.noiseSettings3D.CreateFastNoise();
        
        Vector3 curDir = GetRandomDir(random);

        while (continuePath)
        {
            curPos += curDir;
            path.Add(curPos);
            
            var randAng = random.NextDouble();
            var angle = Easing.Linear(-angleConstraint, angleConstraint, 0.5f * (fn.GetNoise(curPos.x + origin.x, curPos.y + origin.y, curPos.z + origin.z) + 1));
            curDir = Quaternion.Euler(angle, angle, angle) * curDir;
            
            if (curPos.x * curPos.x + curPos.z + curPos.z > maxDistance * maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }
        
        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3>>(origin, path));

        return path;
    }
    
    public static List<Vector3> PerlinWormPathGeneration(System.Random random, Vector3Int origin, int angleConstraint, NoiseSettings noiseSettings)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        var fn = noiseSettings.CreateFastNoise();
        
        Vector3 curDir = GetRandomDir(random);

        while (continuePath)
        {
            curPos += curDir;
            path.Add(curPos);
            
            var randAng = random.NextDouble();
            var angle = Easing.Linear(-angleConstraint, angleConstraint, 0.5f * (fn.GetNoise(curPos.x + origin.x, curPos.y + origin.y, curPos.z + origin.z) + 1));
            curDir = Quaternion.Euler(angle, angle, angle) * curDir;
            
            if (curPos.x * curPos.x + curPos.z + curPos.z > maxDistance * maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }
        
        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3>>(origin, path));

        return path;
    }
    
    /// <summary>
    /// Picks a random direction within angle degrees with each iteration
    /// </summary>
    /// <param name="random"></param>
    /// <param name="origin"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static List<Vector3> AngleConstrainedPathGeneration(System.Random random, Vector3Int origin, int angleConstraint)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 curPos = Vector3.zero;
        path.Add(curPos);
        bool continuePath = true;
        int iter = 0;

        Vector3 curDir = GetRandomDir(random);

        while (continuePath)
        {
            curPos += curDir;
            path.Add(curPos);
            
            var randAng = random.NextDouble();
            var angle = Easing.Linear(-angleConstraint, angleConstraint, (float)randAng);
            curDir = Quaternion.Euler(angle, angle, angle) * curDir;
            
            if (curPos.x * curPos.x + curPos.z + curPos.z > maxDistance * maxDistance)
            {
                continuePath = false;
            }

            iter++;
            if (iter >= maxIterCount)
            {
                continuePath = false;
            }
        }
        
        PathDrawer.kvpQueue.Add(new KeyValuePair<Vector3Int, List<Vector3>>(origin, path));

        return path;
    }

    public static Vector3 GetRandomDir(System.Random random)
    {
        var dirInt = random.Next(0, 6);
        switch (dirInt)
        {
            case 0:
                return Vector3.up;
            case 1:
                return Vector3.down;
            case 2:
                return Vector3.back;
            case 3:
                return Vector3.forward;
            case 4:
                return Vector3.left;
            case 5:
                return Vector3.right;
            default:
                return Vector3.forward;
        }
    }
}

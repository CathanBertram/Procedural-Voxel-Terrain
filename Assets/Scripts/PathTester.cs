using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathTester : MonoBehaviour
{
    public List<Vector3Int> path = new List<Vector3Int>();
    public int seed = 1337;
    public int sphereSize = 1;
    public int lineThickness = 2;
    public int zaxPriority;
    public PathType pathType;
    public void GeneratePath()
    {
        switch (pathType)
        {
            case PathType.DrunkRobot:
                path = PathGenerator.DrunkRobotPathGeneration(new System.Random(seed), Vector3Int.zero);
                break;
            case PathType.DrunkZax:
                path = PathGenerator.DrunkZaxPathGeneration(new System.Random(seed), Vector3Int.zero, zaxPriority);
                break;
            case PathType.DrunkRobotEmu:
                path = PathGenerator.DrunkRobotEmuPathGeneration(new System.Random(seed), Vector3Int.zero);
                break;
            case PathType.DrunkLiberalEmu:
                path = PathGenerator.DrunkLiberalEmuPathGeneration(new System.Random(seed), Vector3Int.zero);
                break;
            case PathType.DrunkModerateEmu:
                path = PathGenerator.DrunkModerateEmuPathGeneration(new System.Random(seed), Vector3Int.zero);
                break;
            case PathType.DrunkConservativeEmu:
                path = PathGenerator.DrunkConservativeEmuPathGeneration(new System.Random(seed), Vector3Int.zero);
                break;
            case PathType.PerlinWorm:
                path = PathGenerator.PerlinWormPathGeneration(new System.Random(seed), Vector3Int.zero);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
}

[CustomEditor(typeof(PathTester))]
public class PatherTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathTester myTarget = (PathTester) target;
        if (GUILayout.Button("Generate Path"))
        {
            myTarget.GeneratePath();
        }
    }
    private void OnSceneGUI()
    {
        PathTester myTarget = (PathTester) target;
        var path = myTarget.path;
        Handles.color = Color.green;
        Handles.SphereHandleCap(0,path[0], Quaternion.identity, myTarget.sphereSize, EventType.Repaint);
        Handles.color = Color.red;
        Handles.SphereHandleCap(1,path[path.Count-1], Quaternion.identity, myTarget.sphereSize, EventType.Repaint);
        Handles.color = Color.blue;
        for (int i = 1; i < path.Count ; i++)
        {
            Handles.DrawLine(path[i - 1], path[i], myTarget.lineThickness);
        }
    }
}

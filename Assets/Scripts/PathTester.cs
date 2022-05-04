using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class PathTester : MonoBehaviour
{
    public List<Vector3> path = new List<Vector3>();
    public int seed = 1337;
    public int sphereSize = 1;
    public int lineThickness = 2;
    public int zaxPriority;
    public PathType pathType;
    public int angleConstraint;
    public NoiseSettings noiseSettings;
    
    public int iterations;
    public TestHandler testHandler;
    public LineRenderer lineRenderer;
    public List<LineRenderer> lineRenderers;
    public GameObject startSphere;
    public GameObject endSphere;
    public void Test()
    {
        testHandler.Clear();
        
        Stopwatch totalST = new Stopwatch();
        totalST.Start();
        for (int i = 0; i < iterations; i++)
        {
            seed = i;
            Stopwatch st = new Stopwatch();
            st.Start();
            GeneratePath();
            CaveGenerator.GenerateCave(i, path, noiseSettings);
            st.Stop();
            testHandler.AddTestResult(seed, st.ElapsedMilliseconds, path.Count);
        }
        
        totalST.Stop();
        
        testHandler.finalResult = new TestHandler.TestResult(0, totalST.ElapsedMilliseconds, path.Count);
        
        testHandler.SaveResults($"{pathType}PathGenerationTest");
    }
    
    public void Test2()
    {
        pathType = PathType.DrunkRobot;
        Test();
        pathType = PathType.DrunkZax;
        Test();
        pathType = PathType.DrunkConservativeEmu;
        Test();
        pathType = PathType.DrunkLiberalEmu;
        Test();
        pathType = PathType.DrunkModerateEmu;
        Test();
        pathType = PathType.DrunkRobotEmu;
        Test();
        pathType = PathType.PerlinWorm;
        Test();
    }
    
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
                path = PathGenerator.PerlinWormPathGeneration(new System.Random(seed), Vector3Int.zero, angleConstraint, noiseSettings);
                break;
            case PathType.AngleConstrained:
                path = PathGenerator.AngleConstrainedPathGeneration(new System.Random(seed), Vector3Int.zero,
                    angleConstraint);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Centre();
        CreateLineRenderers();
    }

    private void CreateLineRenderers()
    {
        startSphere.transform.position = path[0];
        endSphere.transform.position = path[path.Count - 1];
        
        if (lineRenderers == null)
            lineRenderers = new List<LineRenderer>();
        
        foreach (var lr in lineRenderers)
        {
           DestroyImmediate(lr.gameObject); 
        }
        lineRenderers.Clear();
        for (int i = 1; i < path.Count; i++)
        {
            var lr = Instantiate(lineRenderer);
            lineRenderers.Add(lr);
            lr.positionCount = 2;
            lr.SetPosition(0, path[i-1]);
            lr.SetPosition(1, path[i]);
        }
    }
    
    private void Centre()
    {
        Vector3 centre = new Vector3();

        foreach (var v3 in path)
        {
            centre += v3;
        }

        centre /= path.Count;
        transform.position = centre;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(path[0], sphereSize);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(path[path.Count-1], sphereSize);
        Gizmos.color = Color.blue;
        for (int i = 1; i < path.Count ; i++)
        {
            Gizmos.DrawLine(path[i - 1], path[i]);
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
        if (GUILayout.Button("Test"))
        {
            myTarget.Test();
        }
        if (GUILayout.Button("Test2"))
        {
            myTarget.Test2();
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

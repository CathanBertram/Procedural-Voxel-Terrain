using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    public static List<KeyValuePair<Vector3Int, List<Vector3Int>>> kvpQueue =
        new List<KeyValuePair<Vector3Int, List<Vector3Int>>>();
    
    public Dictionary<Vector3Int, List<Vector3Int>> paths =
        new Dictionary<Vector3Int, List<Vector3Int>>();

    private int index = 0;

    private void Update()
    {
        foreach (var kvp in kvpQueue)
        {
            if (paths.ContainsKey(kvp.Key)) continue;
            paths.Add(kvp.Key, kvp.Value);
        }
        kvpQueue.Clear();
    }

    private void OnDrawGizmos()
    {
        // foreach (var kvp in kvpQueue)
        // {
        //     if (paths.ContainsKey(kvp.Key)) continue;
        //     paths.Add(kvp.Key, kvp.Value);
        // }
        // kvpQueue.Clear();
        //
        // if (paths.Count == 0) return;
        // // foreach (var pathKVP in paths)
        // // {
        // //     Gizmos.color = Color.green;
        // //     Gizmos.DrawSphere(pathKVP.Key + pathKVP.Value[0], 1);
        // //     Gizmos.DrawSphere(pathKVP.Key + pathKVP.Value[pathKVP.Value.Count-1], 1);
        // //     
        // //     Gizmos.color = Color.red;
        // //     for (int i = 1; i < pathKVP.Value.Count ; i++)
        // //     {
        // //         Gizmos.DrawLine(pathKVP.Value[i - 1] + pathKVP.Key, pathKVP.Value[i] + pathKVP.Key);
        // //     }
        // // }
        //
        // var key = paths.ElementAt(index).Key;
        // var value = paths.ElementAt(index).Value;
        // Gizmos.color = Color.green;
        // Gizmos.DrawSphere(key + value[0], 1);
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(key + value[value.Count-1], 1);
        // Gizmos.color = Color.blue;
        // for (int i = 1; i < value.Count ; i++)
        // {
        //     Gizmos.DrawLine(value[i - 1] + key, value[i] + key);
        // }
    }

    public void Cycle(bool up)
    {
        if (up)
            index++;
        else
            index--;

        if (index >= paths.Count)
            index = 0;
        else if (index < 0)
            index = paths.Count - 1;
    }
}

[CustomEditor(typeof(PathDrawer))]
public class PathDrawerEditor : Editor
{
    private int index = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathDrawer myTarget = (PathDrawer) target;
        if (GUILayout.Button("Cycle Up"))
        {
            myTarget.Cycle(true);
            Cycle(true);
        }
        if (GUILayout.Button("Cycle Down"))
        {
            myTarget.Cycle(false);
            Cycle(false);
        }
    }
    public void Cycle(bool up)
    {
        PathDrawer myTarget = (PathDrawer) target;

        if (up)
            index++;
        else
            index--;

        if (index >= myTarget.paths.Count)
            index = 0;
        else if (index < 0)
            index = myTarget.paths.Count - 1;
    }
    private void OnSceneGUI()
    {
        PathDrawer myTarget = (PathDrawer) target;
        
        if (myTarget.paths.Count == 0) return;
        // foreach (var pathKVP in paths)
        // {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawSphere(pathKVP.Key + pathKVP.Value[0], 1);
        //     Gizmos.DrawSphere(pathKVP.Key + pathKVP.Value[pathKVP.Value.Count-1], 1);
        //     
        //     Gizmos.color = Color.red;
        //     for (int i = 1; i < pathKVP.Value.Count ; i++)
        //     {
        //         Gizmos.DrawLine(pathKVP.Value[i - 1] + pathKVP.Key, pathKVP.Value[i] + pathKVP.Key);
        //     }
        // }

        var key = myTarget.paths.ElementAt(index).Key;
        var value = myTarget.paths.ElementAt(index).Value;
        Handles.color = Color.green;
        Handles.SphereHandleCap(0,key + value[0], Quaternion.identity, 1, EventType.Repaint);
        Handles.color = Color.red;
        Handles.SphereHandleCap(1,key + value[value.Count-1], Quaternion.identity, 1, EventType.Repaint);
        Handles.color = Color.blue;
        for (int i = 1; i < value.Count ; i++)
        {
            Handles.DrawLine(value[i - 1] + key, value[i] + key, 5);
        }
    }
}

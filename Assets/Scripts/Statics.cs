using System;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    public static event Action<GameObject> onFinishInitialGeneration;
    public static void OnFinishInitialGeneration(GameObject obj) { onFinishInitialGeneration?.Invoke(obj); }

    public static event Action onStartWorld;
    public static void OnStartWorld(){onStartWorld?.Invoke();}

    public static event Action<Vector2Int> onStartChunkGen;
    public static void OnStartChunkGen(Vector2Int pos) {onStartChunkGen?.Invoke(pos);}
    public static event Action<Vector2Int> onChunkGenerated;
    public static void OnChunkGenerated(Vector2Int pos) {onChunkGenerated?.Invoke(pos);}

    public static event Action<int> onBroadcastLoadingCount;

    public static void OnBroadcastLoadingCount(int count) { onBroadcastLoadingCount?.Invoke(count); }

    public static event Action onReset;
    public static void OnReset(){onReset?.Invoke();}
    public static event Action<Vector2Int, long, int> onAddTestResult;
    public static void OnAddTestResult(Vector2Int pos, long milliseconds, int length) {onAddTestResult?.Invoke(pos, milliseconds, length);}
}

public struct MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    public MeshData(Vector3[] _vertices, int[] _triangles, Vector2[] _uvs)
    {
        vertices = _vertices;
        triangles = _triangles;
        uvs = _uvs;
    }
}

public struct AdditionalChunkData
{
    public int x, z;
    public List<BlockData> blockData;

    public AdditionalChunkData(Vector2Int pos)
    {
        x = pos.x;
        z = pos.y;
        blockData = new List<BlockData>();
    }
}

public struct BlockData
{
    public int x, y, z;
    public byte blockID;

} 
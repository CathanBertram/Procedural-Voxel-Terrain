using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    
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
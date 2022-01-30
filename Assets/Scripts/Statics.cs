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

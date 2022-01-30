using System.Collections;
using System.Collections.Generic;
using Blocks;
using UnityEngine;
using Voxel;

public static class MeshGenerator
{
    public static Mesh GenerateMesh(byte[,,] voxelMap)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        GenerateMeshValues(voxelMap, vertices, triangles, uvs);
        return CreateMesh(voxelMap, vertices, triangles, uvs);
    }
    private static Mesh CreateMesh(byte[,,] voxelMap, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
    
    public static MeshData GenerateMeshData(byte[,,] voxelMap)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        GenerateMeshValues(voxelMap, vertices, triangles, uvs);
        return CreateMeshData(voxelMap, vertices, triangles, uvs);
    }
    private static MeshData CreateMeshData(byte[,,] voxelMap, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        return new MeshData(vertices.ToArray(), triangles.ToArray(), uvs.ToArray());
    }
    private static void GenerateMeshValues(byte[,,] voxelMap, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        PassableInt vertexIndex = new PassableInt(0);
        for (int y = 0; y < VoxelData.chunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    if (voxelMap[x, y, z] != VoxelData.airID)
                        AddVoxelToChunk(voxelMap, new Vector3(x, y, z), vertices, triangles, uvs, vertexIndex);
                }
            }
        }
    }

    private static void AddVoxelToChunk(byte[,,] voxelMap, Vector3 position, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, PassableInt vertexIndex)
    {
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            if(!CheckVoxel(voxelMap, position + VoxelData.faceChecks[faceIndex]))
            {
                vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 0]]);
                vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 1]]);
                vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 2]]);
                vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 3]]);
                
                AddTexture(BlockDatabase.Instance.BlockData[voxelMap[(int)position.x, (int)position.y, (int)position.z]].GetTextureID(faceIndex), uvs);

                triangles.Add(vertexIndex.value);
                triangles.Add(vertexIndex.value + 1);
                triangles.Add(vertexIndex.value + 2);
                triangles.Add(vertexIndex.value + 2);
                triangles.Add(vertexIndex.value + 1);
                triangles.Add(vertexIndex.value + 3);
                vertexIndex.value += 4;
            }
        }
    }
    
    private static bool CheckVoxel(byte[,,] voxelMap, Vector3 pos)
    {
        var x = Mathf.FloorToInt(pos.x);
        var y = Mathf.FloorToInt(pos.y);
        var z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.chunkWidth - 1 || y < 0 || y > VoxelData.chunkHeight - 1|  z < 0 || z > VoxelData.chunkWidth - 1 || voxelMap[x,y,z] == VoxelData.airID)
            return false;
        return BlockDatabase.Instance.GetIsSolid(voxelMap[x, y, z]);
    }

    private static void AddTexture(int textureID, List<Vector2> uvs)
    {
        // float y = textureID / VoxelData.textureAtlasSize;
        // var x = textureID - (y * VoxelData.textureAtlasSize);
        float y = 0;
        float x = textureID;
        
        var normalizedBlockTextureSize = VoxelData.normailizedBlockTextureSize;
        x *= normalizedBlockTextureSize;
        y *= normalizedBlockTextureSize;
        
        // uvs.Add(new Vector2(x, y));
        // uvs.Add(new Vector2(x, y + normalizedBlockTextureSize));
        // uvs.Add(new Vector2(x + normalizedBlockTextureSize, y));
        // uvs.Add(new Vector2(x + normalizedBlockTextureSize, y + normalizedBlockTextureSize));
        
        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + 1));
        uvs.Add(new Vector2(x + normalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + normalizedBlockTextureSize, y + 1));
    }
}

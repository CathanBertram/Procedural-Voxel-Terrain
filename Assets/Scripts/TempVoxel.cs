using System;
using System.Collections.Generic;
using UnityEngine;
using Voxel;

public class TempVoxel : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    
    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    Mesh mesh;

    private bool[,,] voxelMap = new bool[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];
    void Start()
    {
        mesh = new Mesh();
        
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    
    private void CreateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    private void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.chunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    AddVoxelToChunk(new Vector3(x,y,z));
                }
            }
        }
    }
    private void AddVoxelToChunk(Vector3 position)
    {
        for (int i = 0; i < 6; i++)
        {
            if(!CheckVoxel(position + VoxelData.faceChecks[i]))
            {
                vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[i, 0]]);
                vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[i, 1]]);
                vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[i, 2]]);
                vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[i, 3]]);
                uvs.Add(VoxelData.voxelUVs[0]);
                uvs.Add(VoxelData.voxelUVs[1]);
                uvs.Add(VoxelData.voxelUVs[2]);
                uvs.Add(VoxelData.voxelUVs[3]);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }

    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.chunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    voxelMap[x, y, z] = true;
                }
            }
        }
    }
    private bool CheckVoxel(Vector3 pos)
    {
        var x = Mathf.FloorToInt(pos.x);
        var y = Mathf.FloorToInt(pos.y);
        var z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.chunkWidth - 1 || y < 0 || y > VoxelData.chunkHeight - 1|  z < 0 || z > VoxelData.chunkWidth - 1)
            return false;
        
        return voxelMap[x, y, z];
    }
}

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
                for (int j = 0; j < 6; j++)
                {
                    var triangleIndex = VoxelData.voxelTriangles[i,j];
                    vertices.Add(VoxelData.voxelVertices[triangleIndex] + position);
                    triangles.Add(vertexIndex);
                    
                    uvs.Add(VoxelData.voxelUVs[j]);
                    
                    vertexIndex++;
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

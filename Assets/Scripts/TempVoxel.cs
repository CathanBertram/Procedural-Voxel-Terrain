using System.Collections.Generic;
using UnityEngine;
using Voxel;

public class TempVoxel : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    void Start()
    {
        var vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                var triangleIndex = VoxelData.voxelTriangles[i,j];
                vertices.Add(VoxelData.voxelVertices[triangleIndex]);
                triangles.Add(vertexIndex);
                
                uvs.Add(VoxelData.voxelUVs[j]);
                
                vertexIndex++;
            }
            
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    
}

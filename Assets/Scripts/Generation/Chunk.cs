using System.Collections.Generic;
using UnityEngine;
using Voxel;

namespace Generation
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;
    
        int vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        Mesh mesh;

        private byte[,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];
        void Start()
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Ground");
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
            meshCollider.sharedMesh = mesh;
        }
        private void CreateMeshData()
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int x = 0; x < VoxelData.chunkWidth; x++)
                {
                    for (int z = 0; z < VoxelData.chunkWidth; z++)
                    {
                        if (voxelMap[x,y,z] != VoxelData.airID)
                            AddVoxelToChunk(new Vector3(x,y,z));
                    }
                }
            }
        }
        private void AddVoxelToChunk(Vector3 position)
        {
            for (int faceIndex = 0; faceIndex < 6; faceIndex++)
            {
                if(!CheckVoxel(position + VoxelData.faceChecks[faceIndex]))
                {
                    vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 0]]);
                    vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 1]]);
                    vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 2]]);
                    vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 3]]);
                    
                    AddTexture(ChunkGenerator.Instance.BlockTypes[voxelMap[(int)position.x, (int)position.y, (int)position.z]].GetTextureID(faceIndex));
                    
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
            // for (int y = 0; y < VoxelData.chunkHeight; y++)
            // {
            //     for (int x = 0; x < VoxelData.chunkWidth; x++)
            //     {
            //         for (int z = 0; z < VoxelData.chunkWidth; z++)
            //         {
            //             if (y == VoxelData.chunkHeight - 1)
            //                 voxelMap[x, y, z] = ChunkGenerator.Instance.BlockTypeDictionary["Grass"];
            //             else if (y >= VoxelData.chunkHeight - 3)
            //                 voxelMap[x, y, z] = ChunkGenerator.Instance.BlockTypeDictionary["Dirt"];
            //             else
            //                 voxelMap[x, y, z] = ChunkGenerator.Instance.BlockTypeDictionary["Stone"];
            //         }
            //     }
            // }
            //Set All Voxels To Air
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int y = 0; y < VoxelData.chunkHeight; y++)
                {
                    for (int z = 0; z < VoxelData.chunkWidth; z++)
                    {
                        voxelMap[x, y, z] = VoxelData.airID;
                    }
                }
            }

            //Generate Data
            var yPos = VoxelData.seaLevel;
            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    voxelMap[x,yPos,z] = ChunkGenerator.Instance.BlockTypeDictionary["Grass"];
                    var dirtLayerEnd = yPos - Random.Range(2,5);
                    for (int i = yPos - 1; i >= 0; i--)
                    {
                        if (i > dirtLayerEnd)
                        {
                            voxelMap[x,i,z] = ChunkGenerator.Instance.BlockTypeDictionary["Dirt"];
                        }
                        else
                        {
                            voxelMap[x,i,z] = ChunkGenerator.Instance.BlockTypeDictionary["Stone"];
                        }
                    }
                }
            }
        }
        private bool CheckVoxel(Vector3 pos)
        {
            var x = Mathf.FloorToInt(pos.x);
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z);

            if (x < 0 || x > VoxelData.chunkWidth - 1 || y < 0 || y > VoxelData.chunkHeight - 1|  z < 0 || z > VoxelData.chunkWidth - 1 || voxelMap[x,y,z] == VoxelData.airID)
                return false;
            return ChunkGenerator.Instance.BlockTypes[voxelMap[x, y, z]].IsSolid;
        }

        private void AddTexture(int textureID)
        {
            float y = textureID / VoxelData.textureAtlasSizeInBlocks;
            var x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);
            
            var normalizedBlockTextureSize =VoxelData.normailizedBlockTextureSize;
            x *= normalizedBlockTextureSize;
            y *= normalizedBlockTextureSize;
            
            uvs.Add(new Vector2(x, y));
            uvs.Add(new Vector2(x, y + normalizedBlockTextureSize));
            uvs.Add(new Vector2(x + normalizedBlockTextureSize, y));
            uvs.Add(new Vector2(x + normalizedBlockTextureSize, y + normalizedBlockTextureSize));
        }
    }
}

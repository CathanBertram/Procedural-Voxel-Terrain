using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using static Voxel.VoxelData;

namespace Generation
{
    public class Chunk : MonoBehaviour, IChunkDestructability
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;
    
        int vertexIndex = 0;

        // byte 32768 short 65336 int 137000
        private byte[,,] voxelMap = new byte[chunkWidth, chunkHeight, chunkWidth];

        private Vector2Int chunkPos;
        public Vector2Int ChunkPos => chunkPos;

        private Queue<ChunkThreadInfo<byte[,,]>> voxelMapThreadInfoQueue = new Queue<ChunkThreadInfo<byte[,,]>>();
        private Queue<ChunkThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ChunkThreadInfo<MeshData>>();

        public Action<Vector2Int, Chunk> onFinishedGeneration;

        void Start()
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Ground");
            transform.SetParent(WorldLoader.Instance.transform);
        }
        
        public void BreakBlock(Vector3 pos, byte newBlock = 0)
        {
            var x = Mathf.FloorToInt(pos.x) - chunkPos.x * chunkWidth;
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z) - chunkPos.y * chunkWidth;

            voxelMap[x, y, z] = newBlock;
            RequestMeshData(OnMeshDataReceived);
        }

        public void PlaceBlock(Vector3 pos, byte newBlock)
        {
            var x = Mathf.FloorToInt(pos.x) - chunkPos.x * chunkWidth;
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z) - chunkPos.y * chunkWidth;

            voxelMap[x, y, z] = newBlock;
            RequestMeshData(OnMeshDataReceived);
        }

        #region default
        public void Generate(int xPos, int yPos)
        {
            Generate(new Vector2Int(xPos, yPos));
        }
        public void Generate(Vector2Int _chunkPos)
        {
            chunkPos = _chunkPos;
            
            voxelMap = ChunkGenerator.GenerateVoxelMap(chunkPos.x * chunkWidth, chunkPos.y * chunkWidth);
            var mesh = MeshGenerator.GenerateMesh(voxelMap);
            
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
#endregion

#region threaded

        public void GenerateThreaded(int xPos, int yPos)
        {
            GenerateThreaded(new Vector2Int(xPos, yPos));
        }
        public void GenerateThreaded(Vector2Int _chunkPos)
        {
            chunkPos = _chunkPos;
            RequestVoxelMap(OnVoxelMapReceived);
        }
        
        public void RequestVoxelMap(Action<byte[,,]> callback)
        {
            ThreadPool.QueueUserWorkItem(delegate { VoxelMapThread(callback); });
            
            //ThreadStart threadStart = delegate { VoxelMapThread(callback); };
            //new Thread(threadStart).Start();
        }
        private void VoxelMapThread(Action<byte[,,]> callback)
        {
            voxelMap = ChunkGenerator.GenerateVoxelMap(chunkPos.x * chunkWidth, chunkPos.y * chunkWidth);
            lock (voxelMapThreadInfoQueue)
            {
                voxelMapThreadInfoQueue.Enqueue(new ChunkThreadInfo<byte[,,]>(callback, voxelMap));
            }
        }
        
        public void RequestMeshData(Action<MeshData> callback)
        {
            ThreadPool.QueueUserWorkItem(delegate { MeshDataThread(callback); });
            
            // ThreadStart threadStart = delegate { MeshDataThread(callback); };
            // new Thread(threadStart).Start();
        }

        private void MeshDataThread(Action<MeshData> callback)
        {
            MeshData meshData = MeshGenerator.GenerateMeshData(voxelMap);
            lock (meshDataThreadInfoQueue)
            {
                meshDataThreadInfoQueue.Enqueue(new ChunkThreadInfo<MeshData>(callback, meshData));
            }
        }
        private void Update()
        {
            if (voxelMapThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < voxelMapThreadInfoQueue.Count; i++)
                {
                    ChunkThreadInfo<byte[,,]> chunkInfo = voxelMapThreadInfoQueue.Dequeue();
                    chunkInfo.callback(chunkInfo.parameter);
                }
            }

            if (meshDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
                {
                    ChunkThreadInfo<MeshData> chunkInfo = meshDataThreadInfoQueue.Dequeue();
                    chunkInfo.callback(chunkInfo.parameter);
                }
            }
        }

        private void OnVoxelMapReceived(byte[,,] _voxelMap)
        {
            voxelMap = _voxelMap;
            
            RequestMeshData(OnMeshDataReceived);
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = meshData.vertices;
            mesh.triangles = meshData.triangles;
            mesh.uv = meshData.uvs;
            
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
            
            onFinishedGeneration(chunkPos, this);
        }
        #endregion
        struct ChunkThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T parameter;

            public ChunkThreadInfo(Action<T> _callback, T _parameter)
            {
                callback = _callback;
                parameter = _parameter;
            }
        }
        
    }
}

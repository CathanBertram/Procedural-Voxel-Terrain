using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Voxel;
using static Voxel.VoxelData;

namespace Generation
{
    public class Chunk : MonoBehaviour, IChunkDestructability
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;
        private bool isRendering = false;
        int vertexIndex = 0;

        // byte 32768 short 65336 int 137000
        private byte[,,] voxelMap = new byte[chunkWidth, chunkHeight, chunkWidth];

        private Vector2Int chunkPos;
        public Vector2Int ChunkPos => chunkPos;

        private Queue<ChunkThreadInfo<byte[,,]>> voxelMapThreadInfoQueue = new Queue<ChunkThreadInfo<byte[,,]>>();
        private Queue<ChunkThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ChunkThreadInfo<MeshData>>();

        public Action<Vector2Int, Chunk> onFinishedGeneration;
        public Action<Vector2Int, Chunk> onMeshBuilt;

        void Start()
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Ground");
            transform.SetParent(WorldLoader.Instance.transform);
        }

        public void ReplaceBlock(Vector3 pos, byte newBlock = 0)
        {
            var x = Mathf.FloorToInt(pos.x) - chunkPos.x * chunkWidth;
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z) - chunkPos.y * chunkWidth;

            voxelMap[x, y, z] = newBlock;
            WorldLoader.Instance.RebuildAdjacentChunkMeshes(chunkPos);
            RequestMeshRebuild(OnRebuiltMeshDataReceived);
        }

        public void RebuildMesh()
        {            
            RequestMeshRebuild(OnRebuiltMeshDataReceived);
        }
        
        private void RequestMeshRebuild(Action<MeshData> callback)
        {
            ThreadPool.QueueUserWorkItem(delegate { MeshRebuildThread(callback); });
        }

        private void MeshRebuildThread(Action<MeshData> callback)
        {
            MeshData meshData = MeshGenerator.GenerateMeshData(voxelMap, chunkPos);
            lock (meshDataThreadInfoQueue)
            {
                meshDataThreadInfoQueue.Enqueue(new ChunkThreadInfo<MeshData>(callback, meshData));
            }
        }

        private void OnRebuiltMeshDataReceived(MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = meshData.vertices;
            mesh.triangles = meshData.triangles;
            mesh.uv = meshData.uvs;

            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;

            isRendering = true;
            
            onMeshBuilt?.Invoke(chunkPos, this);
        }

        public byte GetBlockAtIndex(Vector3Int pos)
        {
            if (pos.x < 0 || pos.x >= VoxelData.chunkWidth || pos.y < 0 || pos.y >= VoxelData.chunkHeight ||
                pos.z < 0 || pos.z >= VoxelData.chunkWidth)
                return VoxelData.airID;
            
            return voxelMap[pos.x, pos.y, pos.z];
        }

#region default
        public void Generate(int xPos, int yPos)
        {
            Generate(new Vector2Int(xPos, yPos));
        }
        public void Generate(Vector2Int _chunkPos)
        {
            chunkPos = _chunkPos;
            gameObject.name = $"Chunk {chunkPos.x}, {chunkPos.y}";
            
            voxelMap = ChunkGenerator.GenerateVoxelMap(chunkPos.x * chunkWidth, chunkPos.y * chunkWidth);
            var mesh = MeshGenerator.GenerateMesh(voxelMap, chunkPos);
            
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
            mesh.Optimize();
            mesh.RecalculateNormals();
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
            gameObject.name = $"Chunk {chunkPos.x}, {chunkPos.y}";
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
            MeshData meshData = MeshGenerator.GenerateMeshData(voxelMap, chunkPos);
            lock (meshDataThreadInfoQueue)
            {
                meshDataThreadInfoQueue.Enqueue(new ChunkThreadInfo<MeshData>(callback, meshData));
            }
        }
        private void OnVoxelMapReceived(byte[,,] _voxelMap)
        {
            voxelMap = _voxelMap;
            onFinishedGeneration(chunkPos, this);

            //RequestMeshData(OnMeshDataReceived);
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = meshData.vertices;
            mesh.triangles = meshData.triangles;
            mesh.uv = meshData.uvs;

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
            
            mesh.Optimize();
            mesh.RecalculateNormals();

            isRendering = true;
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

        public void DestroyMesh()
        {
            Mesh mesh = new Mesh();
            meshFilter.mesh = null;
            meshFilter.mesh = mesh;
            
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
        public void AddAdditionalData(AdditionalChunkData additionalChunkData)
        {
            foreach (var item in additionalChunkData.blockData)
            {
                voxelMap[item.x, item.y, item.z] = item.blockID;
            }
        }

        private void OnDrawGizmos()
        {
            if (!WorldLoader.drawGizmos) return;

            if (isRendering) return;
            
            Gizmos.color = new Color(0.2f,0.2f,0.2f, 0.2f);

            var pos = transform.position;
            pos.x += VoxelData.chunkWidth * 0.5f;
            pos.y += VoxelData.chunkHeight * 0.2f;
            pos.z += VoxelData.chunkWidth * 0.5f;
            var extents = new Vector3(VoxelData.chunkWidth, VoxelData.chunkHeight * 0.2f, VoxelData.chunkWidth);
            Gizmos.DrawCube(pos, extents);
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

    [CustomEditor(typeof(Chunk))]
    public class ChunkEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("RegenerateMesh"))
            {
                var t = (Chunk) target;
                t.RebuildMesh();
            }
        }
    }
}

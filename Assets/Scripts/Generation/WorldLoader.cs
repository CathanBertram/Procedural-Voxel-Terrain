using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Blocks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Voxel;


namespace Generation
{
    public class WorldLoader : MonoBehaviour
    {
        enum LoadMode
        {
            Default,
            Threaded
        }

        [SerializeField] private LoadMode loadMode;
        [SerializeField] private Material chunkMaterial;
        [SerializeField] private GameObject chunkPrefab;

        private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
        private Dictionary<Vector2Int, ChunkInfo> loadingChunkInfos = new Dictionary<Vector2Int, ChunkInfo>();

        [SerializeField] private int loadRange = 5;
        
        [SerializeField] private GameObject player;
        private Vector2Int previousPlayerPos = new Vector2Int();

        private Queue<ChunkInfo> chunkLoadQueue = new Queue<ChunkInfo>();
        private int loadingChunks = 0;
        [SerializeField] private int maxLoadingChunks = 5;
        [SerializeField] private int maxChunksToStartPerFrame = 5;
        private static WorldLoader instance;
        public static WorldLoader Instance => instance;
    
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            } else {
                instance = this;
            }
        }

        private void Start()
        {
            loadedChunks = new Dictionary<Vector2Int, Chunk>();
            loadingChunkInfos = new Dictionary<Vector2Int, ChunkInfo>();
            
            var position = player.transform.position;
            previousPlayerPos = new Vector2Int(Mathf.RoundToInt(position.x / VoxelData.chunkWidth), Mathf.RoundToInt(position.z / VoxelData.chunkWidth));

            UnloadChunks();
            LoadChunks();
            
            // for (int x = 0; x < loadRange; x++)
            // {
            //     for (int z = 0; z < loadRange; z++)
            //     {
            //         var xPos = x * VoxelData.chunkWidth;
            //         var zPos = z * VoxelData.chunkWidth;
            //         
            //         GenerateChunk(xPos, zPos);
            //     }
            // }
        }
        public byte GetBlockAtIndex(Vector2Int chunkPos, Vector3Int blockPos)
        {
            if (!TryGetChunkAtPos(chunkPos.x, chunkPos.y, out var chunk))
                return 1;

            return chunk.GetBlockAtIndex(blockPos);
        }
        public byte GetBlockAtIndex(int x, int z, Vector3Int blockPos)
        {
            if (!TryGetChunkAtPos(x, z, out var chunk))
                return 1;
            
            return chunk.GetBlockAtIndex(blockPos);
        }

        public bool CheckSolidAtIndex(Vector2Int chunkPos, Vector3Int blockPos)
        {
            return BlockDatabase.Instance.GetIsSolid(GetBlockAtIndex(chunkPos, blockPos));
        }
        public bool CheckSolidAtIndex(int x, int z, Vector3Int blockPos)
        {
            return BlockDatabase.Instance.GetIsSolid(GetBlockAtIndex(x, z, blockPos));
        }
        private void Update()
        {
            for (int i = 0; i < maxChunksToStartPerFrame; i++)
            {
                CycleChunkQueue();
            }

            if (player == null) return;

            var position = player.transform.position;
            Vector2Int playerPos = new Vector2Int(Mathf.RoundToInt(position.x / VoxelData.chunkWidth), Mathf.RoundToInt(position.z / VoxelData.chunkWidth));
            
            if (playerPos == previousPlayerPos) return;

            previousPlayerPos = playerPos;
            
            UnloadChunks();
            LoadChunks();
        }
        
        public bool TryGetChunkAtPos(int x, int z, out Chunk chunk)
        {
            chunk = null;
            var pos = new Vector2Int(x, z);
            if (loadedChunks.ContainsKey(pos))
            {
                chunk = loadedChunks[pos];
                return true;
            }

            return false;
        }
        
        private void UnloadChunks()
        {
            List<Vector2Int> toRemove = new List<Vector2Int>();
            foreach (var item in loadedChunks)
            {
                if (Vector2Int.Distance(item.Key, previousPlayerPos) > loadRange)
                {
                    Destroy(item.Value.gameObject);
                    toRemove.Add(item.Key);
                }
            }
            
            foreach (var item in toRemove)
            {
                loadedChunks.Remove(item);
            }
            
            toRemove.Clear();
            foreach (var item in loadingChunkInfos)
            {
                if (Vector2Int.Distance(item.Key, previousPlayerPos) > loadRange)
                {
                    toRemove.Add(item.Key);
                }
            }

            foreach (var item in toRemove)
            {
                chunkLoadQueue = new Queue<ChunkInfo>(chunkLoadQueue.Where(x => x.chunkPos != item));
                loadingChunkInfos.Remove(item);
            }
        }

        
        private void LoadChunks()
        {
            List<Vector2Int> chunksToLoad = new List<Vector2Int>();
            for (int x = -loadRange; x < loadRange; x++)
            {
                for (int y = -loadRange; y < loadRange; y++)
                {
                    var pos = new Vector2Int(x + previousPlayerPos.x, y + previousPlayerPos.y);
                    if (loadedChunks.ContainsKey(pos)) continue;
                    if (loadingChunkInfos.ContainsKey(pos)) continue;

                    if (!InsideCircle(pos)) continue;
                    
                    chunksToLoad.Add(pos);
                    //GenerateChunk(pos);
                }   
            }

            chunksToLoad = chunksToLoad.OrderBy(x => Vector2Int.Distance(previousPlayerPos, x)).ToList();

            foreach (var item in chunksToLoad)
            {
                GenerateChunk(item);
            }
        }
        
        private void GenerateChunk(int x, int z)
        {
            var chunk = GameObject.Instantiate(chunkPrefab, new Vector3(x, 0, z), Quaternion.identity).GetComponent<Chunk>();
            switch (loadMode)
            {
                case LoadMode.Default:
                    chunk.Generate(x, z);
                    break;
                case LoadMode.Threaded:
                    chunk.GenerateThreaded(x, z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            loadedChunks.Add(new Vector2Int(x,z), chunk);
        }
        
        private void GenerateChunk(Vector2Int chunkPos)
        {
            Vector2Int worldPos = new Vector2Int(chunkPos.x * VoxelData.chunkWidth, chunkPos.y * VoxelData.chunkWidth);
            

            switch (loadMode)
            {
                case LoadMode.Default:            
                    var chunk = GameObject.Instantiate(chunkPrefab, new Vector3(worldPos.x, 0, worldPos.y), Quaternion.identity).GetComponent<Chunk>();
                    chunk.Generate(chunkPos);
                    loadedChunks.Add(chunkPos, chunk);
                    RebuildAdjacentChunkMeshes(chunkPos);

                    break;
                case LoadMode.Threaded:
                    var chunkInfo = new ChunkInfo(chunkPos, worldPos, GenerateChunkThreaded);
                    if (!chunkLoadQueue.Contains(chunkInfo))
                    {
                        chunkLoadQueue.Enqueue(chunkInfo);
                        loadingChunkInfos.Add(chunkInfo.chunkPos, chunkInfo);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateChunkThreaded(ChunkInfo info)
        {
            var chunkPos = info.chunkPos;
            var worldPos = info.worldPos;
            
            loadingChunks++;
            var chunk = GameObject.Instantiate(chunkPrefab, new Vector3(worldPos.x, 0, worldPos.y), Quaternion.identity).GetComponent<Chunk>();
            chunk.onFinishedGeneration = OnFinishGeneration;
            chunk.GenerateThreaded(chunkPos);
            
            loadedChunks.Add(chunkPos, chunk);
        }

        private void CycleChunkQueue()
        {
            if (chunkLoadQueue.Count > 0)
            {
                if (loadingChunks < maxLoadingChunks)
                {
                    var info = chunkLoadQueue.Dequeue();
                    info.callback(info);
                }
            }
        }
        private void OnFinishGeneration(Vector2Int chunkPos, Chunk chunk)
        {
            loadingChunkInfos.Remove(chunkPos);
            loadingChunks--;

            RebuildAdjacentChunkMeshes(chunkPos);
            
        }

        public void RebuildAdjacentChunkMeshes(Vector2Int chunkPos)
        {
            Chunk c;
            
            if (TryGetChunkAtPos(chunkPos.x - 1, chunkPos.y, out c))
                c.RebuildMesh();
            if (TryGetChunkAtPos(chunkPos.x + 1, chunkPos.y, out c))
                c.RebuildMesh();
            if (TryGetChunkAtPos(chunkPos.x, chunkPos.y - 1, out c))
                c.RebuildMesh();
            if (TryGetChunkAtPos(chunkPos.x, chunkPos.y + 1, out c))
                c.RebuildMesh();
        }
        private bool InsideCircle(Vector2Int pos)
        {
            float dx = previousPlayerPos.x - pos.x;
            float dy = previousPlayerPos.y - pos.y;
            float dSquared = dx * dx + dy * dy;
            return dSquared <= loadRange * loadRange;
        }
        
        public void SaveWorldMesh()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.CombineMeshes(combine);
            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.OptimizeIndexBuffers();
            mesh.OptimizeReorderVertexBuffer();
            AssetDatabase.CreateAsset(mesh, $"Assets/GeneratedMeshes/Mesh{UnityEngine.Random.Range(0,10000)}.asset");
            AssetDatabase.SaveAssets();
        }
        
        struct ChunkInfo
        {
            public readonly Vector2Int chunkPos;
            public readonly Vector2Int worldPos;
            public readonly Action<ChunkInfo> callback;

            public ChunkInfo(Vector2Int _chunkPos, Vector2Int _worldPos, Action<ChunkInfo> _callback)
            {
                chunkPos = _chunkPos;
                worldPos = _worldPos;
                callback = _callback;
            }
        }
    }

    [CustomEditor(typeof(WorldLoader))]
    public class WorldLoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WorldLoader myTarget = (WorldLoader) target;

            if (GUILayout.Button("Save World Mesh"))
            {
                myTarget.SaveWorldMesh();
            }
        }
    }
}

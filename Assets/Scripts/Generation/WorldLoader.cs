using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
        }
        
        private void LoadChunks()
        {
            for (int x = -loadRange; x < loadRange; x++)
            {
                for (int y = -loadRange; y < loadRange; y++)
                {
                    var pos = new Vector2Int(x + previousPlayerPos.x, y + previousPlayerPos.y);
                    if (loadedChunks.ContainsKey(pos)) continue;
                    if (!InsideCircle(pos)) continue;

                    GenerateChunk(pos);
                }   
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
                    break;
                case LoadMode.Threaded:
                    var chunkInfo = new ChunkInfo(chunkPos, worldPos, GenerateChunkThreaded);
                    if(!chunkLoadQueue.Contains(chunkInfo))
                        chunkLoadQueue.Enqueue(chunkInfo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateChunkThreaded(Vector2Int chunkPos, Vector2Int worldPos)
        {
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
                    info.callback(info.chunkPos, info.worldPos);
                }
            }
        }
        private void OnFinishGeneration()
        {
            loadingChunks--;
        }
        private bool InsideCircle(Vector2Int pos)
        {
            float dx = previousPlayerPos.x - pos.x;
            float dy = previousPlayerPos.y - pos.y;
            float dSquared = dx * dx + dy * dy;
            return dSquared <= loadRange * loadRange;
        }

        struct ChunkInfo
        {
            public readonly Vector2Int chunkPos;
            public readonly Vector2Int worldPos;
            public readonly Action<Vector2Int, Vector2Int> callback;

            public ChunkInfo(Vector2Int _chunkPos, Vector2Int _worldPos, Action<Vector2Int, Vector2Int> _callback)
            {
                chunkPos = _chunkPos;
                worldPos = _worldPos;
                callback = _callback;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        public static bool drawGizmos = false;
        [SerializeField] private LoadMode loadMode;
        [SerializeField] private Material chunkMaterial;
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private bool loadAroundPlayer;
        public bool autoStart;

        private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
        private Dictionary<Vector2Int, ChunkInfo> loadingChunkInfos = new Dictionary<Vector2Int, ChunkInfo>();
        private List<Vector2Int> renderingChunks;
        private List<Vector2Int> chunksToRender;

        private Dictionary<Vector2Int, AdditionalChunkData> additionalChunkData =
            new Dictionary<Vector2Int, AdditionalChunkData>(); 

        [SerializeField] private int loadRange = 5;
        
        [SerializeField] private GameObject player;
        private Vector2Int previousPlayerPos = new Vector2Int();

        private Queue<ChunkInfo> chunkLoadQueue = new Queue<ChunkInfo>();
        private int loadingChunks = 0;
        [SerializeField] private int maxLoadingChunks = 5;
        [SerializeField] private int maxChunksToStartPerFrame = 5;
        
        private bool isInitialLoad = true;
        [SerializeField] private int additionalDataLoadRange;
        private static WorldLoader instance;
        public static WorldLoader Instance => instance;
        private bool started = false;

        public bool generateMeshes;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            } else {
                instance = this;
            }
        }

        private void OnEnable()
        {
            Statics.onStartWorld += StartWorld;
        }

        private void OnDisable()
        {
            Statics.onStartWorld -= StartWorld;
        }

        public void Start()
        {
            player.SetActive(false);
            loadedChunks = new Dictionary<Vector2Int, Chunk>();
            loadingChunkInfos = new Dictionary<Vector2Int, ChunkInfo>();
            renderingChunks = new List<Vector2Int>();
            chunksToRender = new List<Vector2Int>();
            var position = player.transform.position;
            previousPlayerPos = new Vector2Int(Mathf.RoundToInt(position.x / VoxelData.chunkWidth), Mathf.RoundToInt(position.z / VoxelData.chunkWidth));

            UnloadChunks();
            LoadChunks();

            started = true;
        }

        public void Unload()
        {
            foreach (var item in loadedChunks)
            {
                item.Value.DestroyMesh();
                DestroyImmediate(item.Value.gameObject);
            }
            loadedChunks.Clear();
            renderingChunks.Clear();
            loadingChunkInfos.Clear();
            chunkLoadQueue.Clear();
            additionalChunkData.Clear();
            isInitialLoad = true;
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

        public void AddAdditionalData(Vector2Int chunkPos, AdditionalChunkData additionalData)
        {
            lock (additionalChunkData)
            {
                if (!TryGetChunkAtPos(chunkPos.x, chunkPos.y, out var chunk))
                {
                    if (additionalChunkData.ContainsKey(chunkPos))
                        additionalChunkData[chunkPos].blockData.AddRange(additionalData.blockData);
                    else
                        additionalChunkData.Add(chunkPos, additionalData);
                }
                else
                {
                    chunk.AddAdditionalData(additionalData);
                }
            }
            
        }
        private void Update()
        {
            //if (!started) return;
            
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
            try
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
            catch (Exception e)
            {
                chunk = null;
                return false;
            }
        }
        
        private void UnloadChunks()
        {
            List<Vector2Int> toRemove = new List<Vector2Int>();
            foreach (var item in loadedChunks)
            {
                if (renderingChunks.Contains(item.Key) && !InsideCircle(item.Key, loadRange + 1))
                {
                    item.Value.DestroyMesh();
                    renderingChunks.Remove(item.Key);
                }

                if (!InsideCircle(item.Key, loadRange + additionalDataLoadRange))
                {
                    Destroy(item.Value.gameObject);
                    toRemove.Add(item.Key);
                    renderingChunks.Remove(item.Key);
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
            for (int x = -loadRange - additionalDataLoadRange; x < loadRange + additionalDataLoadRange; x++)
            {
                for (int y = -loadRange - additionalDataLoadRange; y < loadRange + additionalDataLoadRange; y++)
                {
                    var pos = new Vector2Int(x + previousPlayerPos.x, y + previousPlayerPos.y);

                    if (!loadedChunks.ContainsKey(pos) && !loadingChunkInfos.ContainsKey(pos) && InsideCircle(pos, loadRange + additionalDataLoadRange))
                        chunksToLoad.Add(pos);

                    if (InsideCircle(pos, loadRange) && !renderingChunks.Contains(pos) && !isInitialLoad)
                    {                           
                        if (!generateMeshes) continue;

                        if (TryGetChunkAtPos(pos.x, pos.y, out var chunk))
                        {
                            chunk.RebuildMesh();
                            renderingChunks.Add(chunk.ChunkPos);
                        }
                    }
                    
                    //GenerateChunk(pos);
                }   
            }

            if (isInitialLoad)
                Statics.OnBroadcastLoadingCount(chunksToLoad.Count);
            
            if(loadAroundPlayer)
                chunksToLoad = chunksToLoad.OrderBy(x => Vector2Int.Distance(previousPlayerPos, x)).ToList();

            numLoading = chunksToLoad.Count;
            foreach (var item in chunksToLoad)
            {
                GenerateChunk(item);
            }
        }

        private int numLoading;
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
                    chunk.onFinishedGeneration += OnFinishGeneration;
                    chunk.Generate(chunkPos);
                    loadedChunks.Add(chunkPos, chunk);
                    //RebuildAdjacentChunkMeshes(chunkPos);
                    TryAddAdjacentData(chunkPos);
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
            Statics.OnStartChunkGen(info.chunkPos);
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
            Statics.OnChunkGenerated(chunkPos);
            
            if (additionalChunkData.ContainsKey(chunkPos))
            {
                chunk.AddAdditionalData(additionalChunkData[chunkPos]);
                additionalChunkData.Remove(chunkPos);
            }
        
            loadingChunkInfos.Remove(chunkPos);
            loadingChunks--;
            numLoading--;
            RebuildAdjacentChunkMeshes(chunkPos);

            chunk.onFinishedGeneration = null;
            if (!isInitialLoad) return;

            if (InsideCircle(chunkPos, loadRange))
            {
                chunksToRender.Add(chunkPos);
            }
            
            if (loadingChunks == 0 && chunkLoadQueue.Count == 0 && loadMode == LoadMode.Threaded)
            {
                foreach (var additionalData in additionalChunkData)
                {
                    if (TryGetChunkAtPos(additionalData.Key.x, additionalData.Key.y, out var c))
                    {
                        c.AddAdditionalData(additionalData.Value);
                    }
                }
                additionalChunkData.Clear();
                
                if (generateMeshes)
                {
                    foreach (var item in chunksToRender)
                    {
                        if (TryGetChunkAtPos(item.x, item.y, out var c))
                        {
                            c.RebuildMesh();
                        }
                    }
                }

                chunksToRender.Clear();
                chunksToRender = null;
                isInitialLoad = false;
                Statics.OnFinishInitialGeneration(gameObject);
            }

            if (loadMode == LoadMode.Default && numLoading == 0)
            {
                Statics.OnFinishInitialGeneration(gameObject);
            }
        }

        public void TryAddAdjacentData(Vector2Int chunkPos)
        {
            Chunk c;
            // if (additionalChunkData.ContainsKey(new Vector2Int(chunkPos.x - 1, chunkPos.y)))
            //     if (TryGetChunkAtPos(chunkPos.x - 1, chunkPos.y, out c) && InsideCircle(chunkPos.x - 1, chunkPos.y))
            //         c.AddAdditionalData(additionalChunkData[new Vector2Int(chunkPos.x - 1, chunkPos.y)]);
            //
            // if (additionalChunkData.ContainsKey(new Vector2Int(chunkPos.x + 1, chunkPos.y)))
            //     if (TryGetChunkAtPos(chunkPos.x + 1, chunkPos.y, out c)  && InsideCircle(chunkPos.x + 1, chunkPos.y))
            //         c.AddAdditionalData(additionalChunkData[new Vector2Int(chunkPos.x + 1, chunkPos.y)]);
            //
            // if (additionalChunkData.ContainsKey(new Vector2Int(chunkPos.x , chunkPos.y - 1)))
            //     if (TryGetChunkAtPos(chunkPos.x, chunkPos.y - 1, out c)  && InsideCircle(chunkPos.x, chunkPos.y - 1))
            //         c.AddAdditionalData(additionalChunkData[new Vector2Int(chunkPos.x, chunkPos.y - 1)]);
            //
            // if (additionalChunkData.ContainsKey(new Vector2Int(chunkPos.x, chunkPos.y + 1)))
            //     if (TryGetChunkAtPos(chunkPos.x, chunkPos.y + 1, out c)  && InsideCircle(chunkPos.x, chunkPos.y + 1))
            //         c.AddAdditionalData(additionalChunkData[new Vector2Int(chunkPos.x, chunkPos.y + 1)]);
            //
            // if (additionalChunkData.ContainsKey(new Vector2Int(chunkPos.x - 1, chunkPos.y - 1)))
            //     if (TryGetChunkAtPos(chunkPos.x - 1, chunkPos.y - 1, out c) && InsideCircle(chunkPos.x - 1, chunkPos.y - 1))
            //         c.AddAdditionalData(additionalChunkData[new Vector2Int(chunkPos.x - 1, chunkPos.y - 1)]);
            //
            // if (additionalChunkData.ContainsKey(new Vector2Int(chunkPos.x - 1, chunkPos.y + 1)))
            //     if (TryGetChunkAtPos(chunkPos.x - 1, chunkPos.y + 1, out c)  && InsideCircle(chunkPos.x - 1, chunkPos.y + 1))
            //         c.AddAdditionalData(additionalChunkData[new Vector2Int(chunkPos.x - 1, chunkPos.y + 1)]);
            //
            // if (additionalChunkData.ContainsKey(new Vector2Int(chunkPos.x + 1, chunkPos.y - 1)))
            //     if (TryGetChunkAtPos(chunkPos.x + 1, chunkPos.y - 1, out c)  && InsideCircle(chunkPos.x + 1, chunkPos.y - 1))
            //         c.AddAdditionalData(additionalChunkData[new Vector2Int(chunkPos.x + 1, chunkPos.y - 1)]);
            //
            // if (additionalChunkData.ContainsKey(new Vector2Int(chunkPos.x + 1, chunkPos.y + 1)))
            //     if (TryGetChunkAtPos(chunkPos.x + 1, chunkPos.y + 1, out c)  && InsideCircle(chunkPos.x + 1, chunkPos.y - 1))
            //         c.AddAdditionalData(additionalChunkData[new Vector2Int(chunkPos.x + 1, chunkPos.y + 1)]);
            Vector2Int pos;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    pos = new Vector2Int(chunkPos.x + x, chunkPos.y + y);
                    if (additionalChunkData.ContainsKey(pos))
                        if (TryGetChunkAtPos(pos.x, pos.y, out c))
                        {
                            c.AddAdditionalData(additionalChunkData[pos]);
                            additionalChunkData.Remove(pos);
                        }
                }
            }
           
            //RebuildAdjacentChunkMeshes(chunkPos);
        }
        public void RebuildAdjacentChunkMeshes(Vector2Int chunkPos)
        {
            if (isInitialLoad || !generateMeshes) return;
            
            Chunk c;
            
            if (TryGetChunkAtPos(chunkPos.x - 1, chunkPos.y, out c) && InsideCircle(chunkPos.x - 1, chunkPos.y))
                c.RebuildMesh();
            if (TryGetChunkAtPos(chunkPos.x + 1, chunkPos.y, out c)  && InsideCircle(chunkPos.x + 1, chunkPos.y))
                c.RebuildMesh();
            if (TryGetChunkAtPos(chunkPos.x, chunkPos.y - 1, out c)  && InsideCircle(chunkPos.x, chunkPos.y - 1))
                c.RebuildMesh();
            if (TryGetChunkAtPos(chunkPos.x, chunkPos.y + 1, out c)  && InsideCircle(chunkPos.x, chunkPos.y + 1))
                c.RebuildMesh();
        }
        private bool InsideCircle(Vector2Int pos, int radius)
        {
            float dx = previousPlayerPos.x - pos.x;
            float dy = previousPlayerPos.y - pos.y;
            float dSquared = dx * dx + dy * dy;
            return dSquared <= radius * radius;
        }
        private bool InsideCircle(int x, int y)
        {
            return InsideCircle(new Vector2Int(x, y), loadRange);
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

        public void StartWorld()
        {
            player.SetActive(true);
        }
        
        public void ToggleGizmos()
        {
            drawGizmos = !drawGizmos;
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
            if (GUILayout.Button("Toggle Gizmos"))
            {
                myTarget.ToggleGizmos();
            }
            if (GUILayout.Button("Save World Mesh"))
            {
                myTarget.SaveWorldMesh();
            }
        }
    }
}

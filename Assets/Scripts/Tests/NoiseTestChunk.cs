using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Voxel;

namespace Tests
{
    public class NoiseTestChunk : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private float noiseThreshold;
        [SerializeField] private Texture2D blockTexture;
        private static int dimension = 256;
        
        private byte[,,] voxelMap;
        
        public FastNoise.NoiseType noiseType;
        public int seed = 1337;
        public float frequency = 0.01f;
        public FastNoise.Interp interp = FastNoise.Interp.Quintic;
        public float cellularJitter = 0.45f;
        public float gain = 0.5f;
        public float lacunarity = 2.0f;
        public int octaves = 3;
        public FastNoise.FractalType fractalType = FastNoise.FractalType.FBM;
        public int cellularDistanceIndex0 = 0;
        public int cellularDistanceIndex1 = 1;
        public FastNoise.CellularDistanceFunction cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean;
        public float gradientPerturbAmp = 1.0f;
        public FastNoise.CellularReturnType cellularReturnType = FastNoise.CellularReturnType.CellValue;

        public void Generate()
        {
            
            FastNoise fn = new FastNoise();

            fn.SetNoiseType(noiseType);
            fn.SetSeed(seed);
            fn.SetFrequency(frequency);
            fn.SetInterp(interp);
        
            //Fractal Settings
            fn.SetFractalGain(gain);
            fn.SetFractalLacunarity(lacunarity);
            fn.SetFractalOctaves(octaves);
            fn.SetFractalType(fractalType);
        
            //Cellular Settings
            fn.SetCellularDistance2Indicies(cellularDistanceIndex0, cellularDistanceIndex1);
            fn.SetCellularDistanceFunction(cellularDistanceFunction);
            fn.SetCellularJitter(cellularJitter);
            fn.SetCellularReturnType(cellularReturnType);
        
            fn.SetGradientPerturbAmp(gradientPerturbAmp);
            
            voxelMap = new byte[dimension,dimension,dimension];

            for (int x = 0; x < dimension; x++)
            {
                for (int y = 0; y < dimension; y++)
                {
                    for (int z = 0; z < dimension; z++)
                    {
                        if ((fn.GetNoise(x, y, z) + 1) * 0.5f > noiseThreshold)
                            voxelMap[x, y, z] = 1;
                        else
                            voxelMap[x, y, z] = VoxelData.airID;
                    }
                }
            }
            
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            int vertexIndex = 0;
            
            for (int x = 0; x < dimension; x++)
            {
                for (int y = 0; y < dimension; y++)
                {
                    for (int z = 0; z < dimension; z++)
                    {
                        if (voxelMap[x, y, z] != VoxelData.airID)
                        {
                            for (int faceIndex = 0; faceIndex < 6; faceIndex++)
                            {
                                var position = new Vector3Int(x, y, z);
                                if(!CheckVoxel(position + VoxelData.faceChecks[faceIndex]))
                                {
                                    vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 0]]);
                                    vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 1]]);
                                    vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 2]]);
                                    vertices.Add(position + VoxelData.voxelVertices[VoxelData.voxelTriangles[faceIndex, 3]]);
                                    
                                    uvs.Add(new Vector2(0, 0));
                                    uvs.Add(new Vector2(0, 0));
                                    uvs.Add(new Vector2(0, 0));
                                    uvs.Add(new Vector2(0, 0));

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
                    }
                }
            }
            
            var mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();
            
            meshFilter.sharedMesh = mesh;
        }
        private bool CheckVoxel(Vector3 pos)
        {
            var x = Mathf.FloorToInt(pos.x);
            var y = Mathf.FloorToInt(pos.y);
            var z = Mathf.FloorToInt(pos.z);

            if (x < 0 || x > dimension - 1 || y < 0 || y > dimension - 1 ||  z < 0 || z > dimension - 1 || voxelMap[x,y,z] == VoxelData.airID)
                return false;
            return true;
        }
    }
   
    [CustomEditor(typeof(NoiseTestChunk))]
    public class NoiseTestChunkEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            NoiseTestChunk myTarget = (NoiseTestChunk) target;

            if (GUILayout.Button("Generate"))
            {
                myTarget.Generate();
            }
        }
    }
}

using UnityEngine;

namespace Voxel
{
    public static class VoxelData
    {
        public static readonly int chunkHeight = 128;
        public static readonly int chunkWidth = 16;
        public static readonly int seaLevel = 64;
        public static readonly byte airID = 0;
        
        public static readonly int textureAtlasSizeInBlocks = 2;

        public static float normailizedBlockTextureSize => 1f / textureAtlasSizeInBlocks;

        public static readonly Vector3[] voxelVertices = new Vector3[8]
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(0.0f, 1.0f, 1.0f)
        };

        public static readonly int[,] voxelTriangles = new int[6,4]
        {
            // Back Front Top Bottom Left Right
            // 0 1 2 2 1 3
            {0,3,1,2}, //Back Face
            {5,6,4,7}, //Front Face
            {3,7,2,6}, //Top Face
            {1,5,0,4}, //Bottom Face
            {4,7,0,3}, //Left Face
            {1,2,5,6}  //Right Face
        };

        public static readonly Vector2[] voxelUVs = new Vector2[4]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
        };

        public static readonly Vector3[] faceChecks = new Vector3[6]
        {
            new Vector3(0.0f,0.0f,-1.0f), //Back Face
            new Vector3(0.0f,0.0f,1.0f), //Front Face
            new Vector3(0.0f,1.0f,0.0f), //Top Face
            new Vector3(0.0f,-1.0f,0.0f), //Bottom Face
            new Vector3(-1.0f,0.0f,0.0f), //Left Face
            new Vector3(1.0f,0.0f,0.0f), //Right Face
        };
    }
}

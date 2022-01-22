using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxel;

namespace Generation
{
    public class WorldLoader : MonoBehaviour
    {
        [SerializeField] private Material chunkMaterial;
        [SerializeField] private GameObject chunkPrefab;

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
            for (int x = 0; x < 1; x++)
            {
                for (int z = 0; z < 1; z++)
                {
                    var xPos = x * VoxelData.chunkWidth;
                    var zPos = z * VoxelData.chunkWidth;
                    
                    GameObject.Instantiate(chunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity );
                }
            }
        }
    }
}

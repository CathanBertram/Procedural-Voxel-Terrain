using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private ChunkGenerationUI chunkGenUI;

    private void Start()
    {
        chunkGenUI.onStartWorld += StartWorld;
    }

    private void StartWorld()
    {
        Statics.OnStartWorld();
        chunkGenUI.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(true);
    }
}

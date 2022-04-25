using System;
using System.Collections;
using System.Collections.Generic;
using Generation;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Object = System.Object;

public class ChunkGenerationUI : MonoBehaviour
{
    public event Action onStartWorld;
    [SerializeField] private RectTransform startButton;
    [SerializeField] private Image imagePrefab;
    [SerializeField] private TextMeshProUGUI chunkCountText;
    [SerializeField] private TextMeshProUGUI seedText;
    
    private int numberOfLoadedChunks;
    private int numberOfChunksToLoad;
    private Dictionary<Vector2Int, Image> chunkDict;

    private void Start()
    {
        startButton.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        seedText.text = $"Seed: {Noise.Seed}";
    }

    private void OnEnable()
    {
        Statics.onFinishInitialGeneration += OnFinishInitialGeneration;
        Statics.onStartChunkGen += OnStartChunkGen;
        Statics.onChunkGenerated += OnChunkGenerated;
        Statics.onBroadcastLoadingCount += GetLoadingCount;
        Statics.onReset += OnReset;
    }

    private void OnDisable()
    {
        Statics.onFinishInitialGeneration -= OnFinishInitialGeneration;
        Statics.onStartChunkGen -= OnStartChunkGen;
        Statics.onChunkGenerated -= OnChunkGenerated;
        Statics.onBroadcastLoadingCount -= GetLoadingCount;
        Statics.onReset -= OnReset;
    }

    private void OnReset()
    {
        foreach (var item in chunkDict)
        {
            Destroy(item.Value);
        }

        numberOfLoadedChunks = 0;
        chunkDict.Clear();
        startButton.gameObject.SetActive(false);

        seedText.text = $"Seed: {Noise.Seed}";
    }
    public void OnStartChunkGen(Vector2Int chunkPos)
    {
        if (chunkDict == null)
            chunkDict = new Dictionary<Vector2Int, Image>();
        
        var image = Instantiate(imagePrefab, transform);
        image.rectTransform.anchoredPosition = chunkPos * 10;
        
        chunkDict.Add(chunkPos, image);
    }

    public void GetLoadingCount(int count)
    {
        numberOfChunksToLoad = count;
        chunkCountText.text = $"Number of Chunks Loading: {numberOfLoadedChunks}/{numberOfChunksToLoad}";
    }
    public void OnChunkGenerated(Vector2Int chunkPos)
    {
        if (chunkDict == null)
            chunkDict = new Dictionary<Vector2Int, Image>();

        if (chunkDict.ContainsKey(chunkPos))
            chunkDict[chunkPos].color = Color.green;
        numberOfLoadedChunks++;
        chunkCountText.text = $"Number of Chunks Loading: {numberOfLoadedChunks}/{numberOfChunksToLoad}";

    }
    public void OnFinishInitialGeneration()
    {
        if (WorldLoader.Instance.autoStart)
            StartWorld();
        else
            startButton.gameObject.SetActive(true);
    }
    
    public void StartWorld()
    {
        onStartWorld?.Invoke();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var item in chunkDict)
        {
            Destroy(item.Value.gameObject);
        }
        chunkDict.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using Generation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class TestHandler : MonoBehaviour
{
    [SerializeField] private bool autoStart;
    [SerializeField] private int testIterations;
    private int iter;
    [SerializeField] private string filePath;
    public List<TestResult> testResults;
    public TestResult finalResult;

    [SerializeField] private bool test;

    public int seed;
    private int seedIter = 1;
    // Start is called before the first frame update
    void Start()
    {
        Statics.onAddTestResult += AddTestResult;
        testResults = new List<TestResult>();
        Statics.onFinishInitialGeneration += OnFinishGeneration;
        Noise.Seed = seed;
        //Noise.Seed = 1;
        StartLoad();
    }

    private void StartLoad()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }
    private void OnFinishGeneration(GameObject obj)
    {
        if (!test) return;

        StartCoroutine(ResetForNextIter(obj));
    }

    private void SaveSS()
    {
        var width = Screen.width;
        var height = Screen.height;
        Rect rect = new Rect(0, 0, width, height);
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        var cam = Camera.main;

        cam.targetTexture = renderTexture;
        cam.Render();
 
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();
        
        byte[] bytes = texture.EncodeToPNG();
        var path = Application.dataPath + "/../Assets/SavedTextures/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var textureName = Directory.GetFiles(path).Length.ToString();

        File.WriteAllBytes($"{path}{seed}-{seedIter}.png", bytes);
        
        cam.targetTexture = null;
        RenderTexture.active = null;
 
        Destroy(renderTexture);
        renderTexture = null;
        Destroy(texture);
        texture = null;
    }
    private IEnumerator ResetForNextIter(GameObject obj)
    {
        yield return new WaitForSeconds(1);

        SeedTestDisplay.Instance.SetDisplayText($"{seed}-{seedIter}");
        SaveSS();
        SeedTestDisplay.Instance.ClearText();
        
        yield return new WaitForSeconds(1);
        
        var op = SceneManager.UnloadSceneAsync(obj.scene);
        while (!op.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        iter++;
        seedIter++;
        
        if (iter >= testIterations)
        {
            //SaveResults();
        }
        else
        {
            Noise.Seed = seed;
            Statics.OnReset();
            StartLoad();
        }
    }
    public void SaveResults()
    {
        var path = Application.dataPath + filePath;
        path = Application.dataPath + "/../Assets/TestResults/" + filePath + ".csv";
        if (File.Exists(path))
        {
            int iteration = 0;
            while (true)
            {
                path = Application.dataPath + "/../Assets/TestResults/" + filePath + iteration + ".csv";
                if (!File.Exists(path))
                    break;
                iteration++;
            }
        }
        
        TextWriter tw = new StreamWriter(path, false);
        
        tw.WriteLine($"Total Time, {finalResult.milliseconds}");
        tw.WriteLine();

        tw.WriteLine("Seed,ElapsedTime(ms)");

        foreach (var result in testResults)
        {
            tw.WriteLine($"{result.seed},{result.milliseconds}");
        }

        tw.Close();
        
        Debug.Log("Results Saved");
    }
    
    public void SaveResults(string fileName)
    {
        var path = Application.dataPath + fileName;
        path = Application.dataPath + "/../Assets/TestResults/" + fileName + ".csv";
        if (File.Exists(path))
        {
            int iteration = 0;
            while (true)
            {
                path = Application.dataPath + "/../Assets/TestResults/" + fileName + iteration + ".csv";
                if (!File.Exists(path))
                    break;
                iteration++;
            }
        }
        
        TextWriter tw = new StreamWriter(path, false);
        
        tw.WriteLine($"Total Time, {finalResult.milliseconds}");
        tw.WriteLine();

        tw.WriteLine("Seed,ElapsedTime(ms)");

        foreach (var result in testResults)
        {
            tw.WriteLine($"{result.seed},{result.milliseconds}");
        }

        tw.Close();
        
        Debug.Log("Results Saved");
    }

    public void AddTestResult(Vector2Int pos, long milliseconds)
    {
        testResults.Add(new TestResult(Noise.Seed, milliseconds));
    }
    public void AddTestResult(int seed, long milliseconds)
    {
        if (testResults == null)
            testResults = new List<TestResult>();
        testResults.Add(new TestResult(seed, milliseconds));
    }
    public struct TestResult
    {
        public int seed;
        public long milliseconds;

        public TestResult(int _seed, long _milliseconds)
        {
            seed = _seed;
            milliseconds = _milliseconds;
        }
    }
}

[CustomEditor(typeof(TestHandler))]
public class TestHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        TestHandler myTarget = (TestHandler) target;
        if (GUILayout.Button("Save Test Results"))
        {
            myTarget.SaveResults();
        }
        
    }
}

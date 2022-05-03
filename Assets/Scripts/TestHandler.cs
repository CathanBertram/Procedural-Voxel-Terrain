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
    // Start is called before the first frame update
    void Start()
    {
        Statics.onAddTestResult += AddTestResult;
        testResults = new List<TestResult>();
        Statics.onFinishInitialGeneration += OnFinishGeneration;
        Noise.Seed = iter + 1;
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

    private IEnumerator ResetForNextIter(GameObject obj)
    {

        var op = SceneManager.UnloadSceneAsync(obj.scene);
        while (!op.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        iter++;

        if (iter >= testIterations)
        {
            SaveResults();
        }
        else
        {
            Noise.Seed = iter + 1;
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
        
        tw.WriteLine("Seed,ElapsedTime(ms),Path Length");

        foreach (var result in testResults)
        {
            tw.WriteLine($"{result.seed},{result.milliseconds}, {result.length}");
        }

        tw.Close();
        
        Debug.Log("Results Saved");
    }

    public void AddTestResult(Vector2Int pos, long milliseconds, int length)
    {
        testResults.Add(new TestResult(Noise.Seed, milliseconds, length));
    }
    public void AddTestResult(int seed, long milliseconds, int length)
    {
        if (testResults == null)
            testResults = new List<TestResult>();
        testResults.Add(new TestResult(seed, milliseconds, length));
    }

    public void Clear()
    {
        if (testResults == null)
            testResults = new List<TestResult>();
        
        testResults.Clear();
    }
    public struct TestResult
    {
        public int seed;
        public long milliseconds;
        public int length;

        public TestResult(int _seed, long _milliseconds, int _length)
        {
            seed = _seed;
            milliseconds = _milliseconds;
            length = _length;
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class ScreenshotUtility : MonoBehaviour
{
    public string ssName;
    public CinemachineVirtualCamera vCam;
    public Vector3 vCamOffset;
    public PathTester pathTester;

    public void Screenshot()
    {

        var width = 1920;
        var height = 1080;
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

        File.WriteAllBytes($"{path}{pathTester.pathType}{pathTester.seed}ac{pathTester.angleConstraint}.png", bytes);
        
        cam.targetTexture = null;
        RenderTexture.active = null;
 
        DestroyImmediate(renderTexture);
        renderTexture = null;
        DestroyImmediate(texture);
        texture = null;
    }

    private void OnValidate()
    {
        var transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = vCamOffset;
    }
}

[CustomEditor(typeof(ScreenshotUtility))]
public class ScreenshotUtilityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Screenshot"))
        {
            ScreenshotUtility t = (ScreenshotUtility) target;
            t.Screenshot();
        }
    }
}
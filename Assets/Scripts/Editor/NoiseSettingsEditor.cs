using System;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(GeneratorSettings))]

public class NoiseSettingsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
	{
		GeneratorSettings generatorSettings = ((GeneratorSettings)target);
		DrawDefaultInspector();
	}

	public override bool HasPreviewGUI()
	{
		return true;
	}

	// public override GUIContent GetPreviewTitle()
	// {
	// 	return new GUIContent("FastNoise Unity - " + ((NoiseSettings)target).noiseName);
	// }

	public override void DrawPreview(Rect previewArea)
	{
		GeneratorSettings generatorSettings = ((GeneratorSettings)target);
		FastNoise fastNoise = generatorSettings.noiseSettings2D.CreateFastNoise();

		Texture2D tex = new Texture2D((int)previewArea.width, (int)previewArea.height);
		Color32[] pixels = new Color32[tex.width * tex.height];

		float[] noiseSet = new float[pixels.Length];

		float min = Single.MaxValue;
		float max = Single.MinValue;
		int index = 0;

		for (int y = 0; y < tex.height; y++)
		{
			for (int x = 0; x < tex.width; x++)
			{
				var noise = noiseSet[index++] = (float) fastNoise.GetNoise(x, y);
				min = Mathf.Min(min, noise);
				max = Mathf.Max(max, noise);
			}
		}

		float scale = 255f / (max - min);

		for (int i = 0; i < noiseSet.Length; i++)
		{
			byte noise = (byte)Mathf.Clamp((noiseSet[i] - min) * scale, 0f, 255f);
				pixels[i] = new Color32(noise, noise, noise, 255);
		}

		tex.SetPixels32(pixels);
		tex.Apply();
		GUI.DrawTexture(previewArea, tex, ScaleMode.StretchToFill, false);
	}
}

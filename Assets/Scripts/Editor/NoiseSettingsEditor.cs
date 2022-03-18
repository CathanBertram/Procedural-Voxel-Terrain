using System;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(NoiseSettings))]

public class NoiseSettingsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
	{
		NoiseSettings noiseSettings = ((NoiseSettings)target);
		//FastNoise fastNoise = noiseSettings.fastNoise;
		FastNoise fastNoise = new FastNoise();
		//noiseSettings.noiseName = EditorGUILayout.TextField("Name", noiseSettings.noiseName);

		//noiseSettings.generalSettingsFold = EditorGUILayout.Foldout(noiseSettings.generalSettingsFold, "General Settings");
		
		fastNoise.SetNoiseType(
			noiseSettings.noiseType = (FastNoise.NoiseType)EditorGUILayout.EnumPopup("Noise Type", noiseSettings.noiseType));
		fastNoise.SetSeed(noiseSettings.seed = EditorGUILayout.IntField("Seed", noiseSettings.seed));
		fastNoise.SetFrequency(noiseSettings.frequency = EditorGUILayout.FloatField("Frequency", noiseSettings.frequency));
		fastNoise.SetInterp(
			noiseSettings.interp = (FastNoise.Interp)EditorGUILayout.EnumPopup("Interpolation", noiseSettings.interp));
		
		
		fastNoise.SetFractalType(
			noiseSettings.fractalType =
				(FastNoise.FractalType)EditorGUILayout.EnumPopup("Fractal Type", noiseSettings.fractalType));
		fastNoise.SetFractalOctaves(
			noiseSettings.octaves = EditorGUILayout.IntSlider("Octaves", noiseSettings.octaves, 2, 9));
		fastNoise.SetFractalLacunarity(
			noiseSettings.lacunarity = EditorGUILayout.FloatField("Lacunarity", noiseSettings.lacunarity));
		fastNoise.SetFractalGain(noiseSettings.gain = EditorGUILayout.FloatField("Gain", noiseSettings.gain));
		
		fastNoise.SetCellularReturnType(
			noiseSettings.cellularReturnType =
				(FastNoise.CellularReturnType)EditorGUILayout.EnumPopup("Return Type", noiseSettings.cellularReturnType));
	
		fastNoise.SetCellularDistanceFunction(
			noiseSettings.cellularDistanceFunction =
				(FastNoise.CellularDistanceFunction)
					EditorGUILayout.EnumPopup("Distance Function", noiseSettings.cellularDistanceFunction));
		noiseSettings.cellularDistanceIndex0 = EditorGUILayout.IntSlider("Distance2 Index 0", Mathf.Min(noiseSettings.cellularDistanceIndex0, noiseSettings.cellularDistanceIndex1 - 1), 0, 2);
		noiseSettings.cellularDistanceIndex1 = EditorGUILayout.IntSlider("Distance2 Index 1", noiseSettings.cellularDistanceIndex1, 1, 3);
		fastNoise.SetCellularDistance2Indicies(noiseSettings.cellularDistanceIndex0, noiseSettings.cellularDistanceIndex1);

		fastNoise.SetCellularJitter(
			noiseSettings.cellularJitter =
				EditorGUILayout.Slider("Cell Jitter", noiseSettings.cellularJitter, 0f, 1f));
		
		
		
		fastNoise.SetGradientPerturbAmp(
			noiseSettings.gradientPerturbAmp = EditorGUILayout.FloatField("Amplitude", noiseSettings.gradientPerturbAmp));

		if (GUILayout.Button("Reset"))
		{
			fastNoise.SetSeed(noiseSettings.seed = 1337);
			fastNoise.SetFrequency(noiseSettings.frequency = 0.01f);
			fastNoise.SetInterp(noiseSettings.interp = FastNoise.Interp.Quintic);
			fastNoise.SetNoiseType(noiseSettings.noiseType = FastNoise.NoiseType.Simplex);

			fastNoise.SetFractalOctaves(noiseSettings.octaves = 3);
			fastNoise.SetFractalLacunarity(noiseSettings.lacunarity = 2.0f);
			fastNoise.SetFractalGain(noiseSettings.gain = 0.5f);
			fastNoise.SetFractalType(noiseSettings.fractalType = FastNoise.FractalType.FBM);

			fastNoise.SetCellularDistanceFunction(
				noiseSettings.cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean);
			fastNoise.SetCellularReturnType(noiseSettings.cellularReturnType = FastNoise.CellularReturnType.CellValue);

			fastNoise.SetCellularDistance2Indicies(noiseSettings.cellularDistanceIndex0 = 0, noiseSettings.cellularDistanceIndex1 = 1);
			fastNoise.SetCellularJitter(noiseSettings.cellularJitter = 0.45f);

			fastNoise.SetGradientPerturbAmp(noiseSettings.gradientPerturbAmp = 1.0f);
		}
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
		NoiseSettings noiseSettings = ((NoiseSettings)target);
		FastNoise fastNoise = noiseSettings.CreateFastNoise();

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

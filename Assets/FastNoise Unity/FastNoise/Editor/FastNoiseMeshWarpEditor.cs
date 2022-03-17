using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FastNoiseMeshWarp))]
public class FastNoiseMeshWarpEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (Application.isPlaying && GUILayout.Button("Recalculate"))
		{
			FastNoiseMeshWarp fastNoiseMeshWarp = ((FastNoiseMeshWarp)target);

			fastNoiseMeshWarp.WarpAllMeshes();
		}
	}
}
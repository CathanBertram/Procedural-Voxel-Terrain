using Tests;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EasingTest))]
    public class EasingTestEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EasingTest myTarget = (EasingTest) target;

            if (GUILayout.Button("Update"))
            {
                myTarget.UpdateColours();
            }
        }
    }
}
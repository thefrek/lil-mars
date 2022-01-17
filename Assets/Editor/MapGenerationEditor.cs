using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGeneration))]
public class MapGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGeneration mapGeneration = (MapGeneration)target;

        if (GUILayout.Button("Regenerate Map"))
        {
            mapGeneration.ClearMap();
            mapGeneration.GenerateMap();
        }
    }
}

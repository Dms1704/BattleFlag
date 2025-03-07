using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(HexGridGenerator))]
public class HexGridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        
        if (GUILayout.Button("Generate Hex Grid"))
        {
            ((HexGridGenerator)target).GenerateHexGrid();
        }
        else if (GUILayout.Button("Clear Hex Grid"))
        {
            ((HexGridGenerator)target).CleanHexGrid();
        }
    }
}
#endif

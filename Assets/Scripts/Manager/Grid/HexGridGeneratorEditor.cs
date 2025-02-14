using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

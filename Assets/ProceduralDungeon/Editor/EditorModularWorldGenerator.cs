using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModularWorldGenerator))]
public class EditorModularWorldGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        ModularWorldGenerator generator = (ModularWorldGenerator)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Dungeon"))
        {
            //check if in runtime mode
            if (EditorApplication.isPlaying)
            {
                generator.GenerateDungeon();
            }
            else
            {
                Debug.LogError("You can only generate a dungeon in runtime mode");
            }
        }
    }
}

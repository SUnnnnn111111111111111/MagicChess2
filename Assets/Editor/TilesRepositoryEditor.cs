using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(TilesRepository))]
public class TilesRepositoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TilesRepository repo = (TilesRepository)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Info", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField("Total Tiles", repo.GetTiles().Count.ToString());

        EditorGUILayout.Space();
        if (GUILayout.Button("Print All Positions"))
        {
            foreach (var kvp in repo.GetTiles())
            {
                Debug.Log($"Tile at {kvp.Key}");
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(repo);
        }
    }
}
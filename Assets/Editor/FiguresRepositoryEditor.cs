using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(FiguresRepository))]
public class FiguresRepositoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FiguresRepository repo = (FiguresRepository)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Info", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField("White Figures", repo.GetWhiteFigures().Count.ToString());
        EditorGUILayout.LabelField("Black Figures", repo.GetBlackFigures().Count.ToString());

        EditorGUILayout.Space();
        if (GUILayout.Button("Update Debug Lists"))
        {
            repo.UpdateDebugLists();
        }

        if (GUILayout.Button("Clear All Figures"))
        {
            ClearAllFigures(repo.GetWhiteFigures());
            ClearAllFigures(repo.GetBlackFigures());
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(repo);
        }
    }

    private void ClearAllFigures(List<Figure> figures)
    {
        foreach (var figure in figures.ToArray())
        {
            if (figure != null)
                DestroyImmediate(figure.gameObject);
        }
    }
}
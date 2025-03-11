using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Figure))] 
public class FigureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Figure figure = (Figure)target;

        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Info", EditorStyles.boldLabel);
        
        bool isSelected = GameManager.Instance.SelectedFigure == figure;
        EditorGUILayout.LabelField("Is Selected", isSelected ? "Yes" : "No");
        
        EditorGUILayout.LabelField("Current Tile", figure.GetCurrentTilePosition());
        EditorGUILayout.LabelField("Available Moves", figure.GetAvailableMovesCount().ToString());
        EditorGUILayout.LabelField("Current Tile Is Highlighted", figure.IsHighlighted().ToString());

        
        if (GUILayout.Button("Highlight Available Moves"))
        {
            figure.HighlightAvailableToMoveTiles();
        }
        
        if (GUILayout.Button("Destroy This Figure"))
        {
            DestroyImmediate(figure.gameObject);
        }

        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(figure);
        }
    }
}
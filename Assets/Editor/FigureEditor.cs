// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(Figure))] 
// public class FigureEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();
//
//         Figure figure = (Figure)target;
//
//         if (figure == null)
//         {
//             EditorGUILayout.HelpBox("Ошибка: Объект Figure не найден!", MessageType.Error);
//             return;
//         }
//
//         EditorGUILayout.Space();
//         EditorGUILayout.LabelField("Debug info", EditorStyles.boldLabel);
//         
//         bool isSelected = SelectedFigureManager.Instance != null && SelectedFigureManager.Instance.SelectedFigure == figure;
//         EditorGUILayout.LabelField("Is selected", isSelected ? "Yes" : "No");
//         
//         EditorGUILayout.LabelField("HasMoved", figure.isFirstMove.ToString());
//
//         string currentTilePosition = figure.GetCurrentTilePosition() ?? "None";
//         EditorGUILayout.LabelField("Current tile", currentTilePosition);
//
//         bool isHighlighted = figure.CurrentTile != null && figure.IsCurrentTileHighlighted();
//         EditorGUILayout.LabelField("Current tile IsHighlighted", isHighlighted.ToString());
//
//         int availableMoves = figure.GetAvailableMovesCount();
//         EditorGUILayout.LabelField("Available moves", availableMoves.ToString());
//
//         if (GUILayout.Button("Highlight available moves for player"))
//         {
//             figure.GetComponent<FigureLogic>().HighlightAvailableToMoveTiles(true);
//         }
//
//         if (GUILayout.Button("Destroy this figure"))
//         {
//             DestroyImmediate(figure.gameObject);
//         }
//
//         if (GUI.changed)
//         {
//             EditorUtility.SetDirty(figure);
//         }
//     }
// }
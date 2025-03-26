// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(Tile))] // Говорим Unity, что этот редактор - для Tile
// public class TileEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         // Отрисовываем стандартный инспектор Unity
//         base.OnInspectorGUI();
//
//         // Получаем ссылку на объект Tile, который мы редактируем
//         Tile tile = (Tile)target;
//
//         // Добавляем раздел "Debug Info"
//         EditorGUILayout.Space();
//         EditorGUILayout.LabelField("Debug Info", EditorStyles.boldLabel);
//         EditorGUILayout.LabelField("Position", tile.Position.ToString());
//         EditorGUILayout.LabelField("Neighbors", tile.Neighbors.Count.ToString());
//         EditorGUILayout.LabelField("Occupying Figure", tile.OccupyingFigure != null ? tile.OccupyingFigure.name : "None");
//         EditorGUILayout.LabelField("Is Highlighted", tile.IsHighlighted.ToString());
//
//         // Автообновление инспектора (чтобы значения менялись без перезапуска)
//         if (GUI.changed)
//         {
//             EditorUtility.SetDirty(tile);
//         }
//     }
// }
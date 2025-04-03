using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileFactory))]
public class TileFactoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileFactory factory = (TileFactory)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("⚙️ Генерация поля", EditorStyles.boldLabel);

        if (GUILayout.Button("🔄 Сгенерировать доску"))
        {
            // factory.GenerateBoard();
        }

        if (GUILayout.Button("🧹 Очистить доску"))
        {
            // factory.ClearBoard();
        }
    }
}
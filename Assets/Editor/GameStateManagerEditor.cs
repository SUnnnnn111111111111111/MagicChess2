using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameStateManager))]
public class GameStateManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameStateManager manager = (GameStateManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Controls", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField("Current State", manager.CurrentState.ToString());
        EditorGUILayout.LabelField("Game Mode", manager.CurrentGameMode.ToString());
        EditorGUILayout.LabelField("Human Plays White", manager.humanPlaysWhite.ToString());

        EditorGUILayout.Space();
        if (GUILayout.Button("Switch Teams Turn"))
        {
            manager.SetGameState(manager.CurrentState == GameStateManager.GameState.WhitesPlaying 
                ? GameStateManager.GameState.BlacksPlaying 
                : GameStateManager.GameState.WhitesPlaying);
        }
        
        if (GUILayout.Button("Toggle Pause"))
        {
            manager.SetPaused(manager.CurrentState != GameStateManager.GameState.Paused);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
    }
}
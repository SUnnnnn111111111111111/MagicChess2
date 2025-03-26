using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeathEffectFactory))]
public class DeathEffectFactoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DeathEffectFactory factory = (DeathEffectFactory)target;

        GUILayout.Space(10f);
        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("🎬 Тест эффекта смерти"))
        {
            GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dummy.name = "DummyDeathEffectTest";
            dummy.transform.position = factory.transform.position + Vector3.forward * 2f;

            factory.gameObject.SetActive(true);
            factory.CreateDeathEffect(dummy.transform);

            Object.Destroy(dummy, 0.1f);
        }

        GUI.enabled = true;
    }
}

public class FakeFigure : Figure
{
    
}
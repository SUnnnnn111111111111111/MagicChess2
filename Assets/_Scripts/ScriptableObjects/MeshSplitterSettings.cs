using UnityEngine;

[CreateAssetMenu(fileName = "MeshSplitterSettings", menuName = "Effects/Mesh Splitter Settings")]
public class MeshSplitterSettings : ScriptableObject
{
    public float force = 5f;
    public int minParts = 5;
    public int maxParts = 15;
    public float cubeSize = 0.15f;
    public Vector3 positionOffset = Vector3.zero;
}
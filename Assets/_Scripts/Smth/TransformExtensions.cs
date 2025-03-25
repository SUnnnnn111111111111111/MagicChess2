using UnityEngine;

public static class TransformExtensions
{
    public static void SnapToGrid(this Transform transform)
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.z = Mathf.Round(pos.z);
        transform.position = pos;
    }
}
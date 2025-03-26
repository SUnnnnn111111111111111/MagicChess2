using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MeshSplitter : MonoBehaviour
{
    public float force;
    public int minParts;
    public int maxParts;
    public float cubeSize;
    public Vector3 positionOffset;

    public void SplitMeshAndExplode()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                Debug.LogWarning($"Mesh у объекта {meshFilter.gameObject.name} отсутствует.");
                continue;
            }

            List<Vector3> positions = GenerateCubePositions(mesh.vertices, Random.Range(minParts, maxParts));
            if (positions.Count == 0)
            {
                Debug.LogWarning("Нет позиций для создания кубиков.");
                continue;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                CreateExplodedCube(positions[i], i);
            }

            Destroy(meshFilter.gameObject);
        }
    }

    List<Vector3> GenerateCubePositions(Vector3[] vertices, int partsCount)
    {
        List<Vector3> positions = new List<Vector3>();
        float radius = 1.0f;

        for (int i = 0; i < partsCount; i++)
        {
            float phi = Random.Range(0f, Mathf.PI * 2);
            float theta = Random.Range(0f, Mathf.PI);

            float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
            float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = radius * Mathf.Cos(theta);

            positions.Add(new Vector3(x, y, z));
        }

        return positions;
    }

    void CreateExplodedCube(Vector3 position, int partIndex)
    {
        GameObject cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeObject.transform.SetParent(transform);
        cubeObject.transform.localScale = Vector3.one * cubeSize;
        cubeObject.transform.localPosition = position + positionOffset;

        MeshRenderer renderer = cubeObject.GetComponent<MeshRenderer>();
        MeshRenderer parentRenderer = GetComponentInChildren<MeshRenderer>();

        if (parentRenderer != null)
            renderer.material = parentRenderer.material;
        else
        {
            Debug.LogWarning("MeshRenderer не найден в дочерних объектах. Используется материал по умолчанию.");
            renderer.material = new Material(Shader.Find("Standard"));
        }

        Rigidbody rb = cubeObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(Random.onUnitSphere * force, ForceMode.Impulse);
    }
}

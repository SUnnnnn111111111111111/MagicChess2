using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MeshSplitter : MonoBehaviour
{
    public float force = 5f;  // Сила разлета
    public int minParts = 3;  // Минимальное количество частей
    public int maxParts = 10; // Максимальное количество частей
    public float cubeSize = 0.1f;  // Размер кубиков
    public Vector3 positionOffset = Vector3.zero;  // Смещение для создания кубиков

    public void SplitMeshAndExplode(Figure figure)
    {
        // Получаем все дочерние объекты с MeshFilter
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null) continue;

            // Разбиваем Mesh на части
            List<Vector3> positions = GenerateCubePositions(mesh.vertices, Random.Range(minParts, maxParts));

            // Для каждой части создаем новый куб и анимируем его
            for (int i = 0; i < positions.Count; i++)
            {
                CreateExplodedCube(positions[i], i, figure);
            }

            // Удаляем исходный Mesh или объект (если нужно)
            Destroy(meshFilter.gameObject);
        }
    }

    // Генерируем позиции для маленьких кубиков внутри Mesh
    List<Vector3> GenerateCubePositions(Vector3[] vertices, int partsCount)
    {
        List<Vector3> positions = new List<Vector3>();

        // Для каждого заданного количества частей создаем случайные позиции
        for (int i = 0; i < partsCount; i++)
        {
            // Случайная позиция в пределах вершины исходного Mesh
            Vector3 randomPosition = vertices[Random.Range(0, vertices.Length)];
            positions.Add(randomPosition);
        }

        return positions;
    }

    // Создание нового куба и анимация разлета
    void CreateExplodedCube(Vector3 position, int partIndex, Figure figure)
    {
        // Создаем новый куб (в Unity это просто небольшой Cube GameObject)
        GameObject cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeObject.transform.SetParent(transform);

        // Устанавливаем размер куба
        cubeObject.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        // Перемещаем куб в соответствующую позицию, добавляем смещение относительно локальной позиции
        Vector3 localPosition = position + positionOffset; // Теперь позиция учитывает смещение
        cubeObject.transform.localPosition = localPosition; // Устанавливаем локальную позицию

        // Попытка получить компонент MeshRenderer в дочерних объектах
        MeshRenderer renderer = cubeObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            // Если MeshRenderer нет, добавляем его и устанавливаем материал
            renderer = cubeObject.AddComponent<MeshRenderer>();

            // Применяем материал из дочернего объекта
            MeshRenderer parentRenderer = GetComponentInChildren<MeshRenderer>();
            if (parentRenderer != null)
            {
                renderer.material = parentRenderer.material;  // Используем материал из дочернего объекта
            }
            else
            {
                Debug.LogWarning("MeshRenderer не найден в дочерних объектах. Используется материал по умолчанию.");
                renderer.material = new Material(Shader.Find("Standard"));  // Используем материал по умолчанию
            }
        }
        else
        {
            // Если MeshRenderer найден, применяем материал
            MeshRenderer parentRenderer = GetComponentInChildren<MeshRenderer>();
            if (parentRenderer != null)
            {
                renderer.material = parentRenderer.material;
            }
            else
            {
                Debug.LogWarning("MeshRenderer не найден в дочерних объектах. Используется материал по умолчанию.");
                renderer.material = new Material(Shader.Find("Standard"));
            }
        }
        
        Rigidbody rb = cubeObject.AddComponent<Rigidbody>();
        rb.useGravity = true; 
        rb.AddForce(Random.onUnitSphere * force, ForceMode.Impulse); 
    }
}



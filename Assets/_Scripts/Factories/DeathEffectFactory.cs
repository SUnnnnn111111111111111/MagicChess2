using UnityEngine;
using System.Collections.Generic;

public class DeathEffectFactory : MonoBehaviour
{
    public static DeathEffectFactory Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private MeshSplitterSettings splitterSettings;

    [Header("Visual Effect Prefab (optional)")]
    [SerializeField] private GameObject effectVisualPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float soundVolume = 0.8f;

    private Queue<GameObject> effectPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    [ContextMenu("Test Death Effect")]
    public void CreateDeathEffect(Transform source)
    {
        if (source == null || splitterSettings == null)
            return;

        GameObject effectObject = GetPooledEffect();
        effectObject.transform.position = source.position;
        effectObject.transform.rotation = source.rotation;
        effectObject.SetActive(true);

        if (effectVisualPrefab != null)
        {
            GameObject visual = Instantiate(effectVisualPrefab, effectObject.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;
        }
        else
        {
            CloneMeshHierarchy(source, effectObject.transform);
        }

        MeshSplitter splitter = effectObject.AddComponent<MeshSplitter>();
        splitter.force = splitterSettings.force;
        splitter.minParts = splitterSettings.minParts;
        splitter.maxParts = splitterSettings.maxParts;
        splitter.cubeSize = splitterSettings.cubeSize;
        splitter.positionOffset = splitterSettings.positionOffset;

        splitter.SplitMeshAndExplode();

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, source.position, soundVolume);
        }

        StartCoroutine(ReturnToPool(effectObject, 2f));
    }

    private GameObject GetPooledEffect()
    {
        if (effectPool.Count > 0)
        {
            return effectPool.Dequeue();
        }

        return new GameObject("PooledDeathEffect");
    }

    private System.Collections.IEnumerator ReturnToPool(GameObject effectObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Удаляем все дочерние объекты (mesh, визуал, collider, и т.д.)
        foreach (Transform child in effectObject.transform)
        {
            Destroy(child.gameObject);
        }

        Destroy(effectObject.GetComponent<MeshSplitter>());
        effectObject.SetActive(false);
        effectPool.Enqueue(effectObject);
    }

    private void CloneMeshHierarchy(Transform source, Transform targetParent)
    {
        foreach (Transform child in source)
        {
            GameObject clone = new GameObject(child.name);
            clone.transform.SetParent(targetParent);
            clone.transform.localPosition = child.localPosition;
            clone.transform.localRotation = child.localRotation;
            clone.transform.localScale = child.localScale;

            MeshFilter originalMeshFilter = child.GetComponent<MeshFilter>();
            if (originalMeshFilter != null)
            {
                MeshFilter cloneMeshFilter = clone.AddComponent<MeshFilter>();
                cloneMeshFilter.mesh = Object.Instantiate(originalMeshFilter.sharedMesh);
            }

            MeshRenderer originalRenderer = child.GetComponent<MeshRenderer>();
            if (originalRenderer != null)
            {
                MeshRenderer cloneRenderer = clone.AddComponent<MeshRenderer>();
                cloneRenderer.materials = originalRenderer.sharedMaterials;
            }

            MeshCollider originalCollider = child.GetComponent<MeshCollider>();
            if (originalCollider != null)
            {
                MeshCollider cloneCollider = clone.AddComponent<MeshCollider>();
                cloneCollider.sharedMesh = Object.Instantiate(originalCollider.sharedMesh);
                cloneCollider.convex = true;
            }

            CloneMeshHierarchy(child, clone.transform);
        }
    }
}

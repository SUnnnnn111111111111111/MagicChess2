using UnityEngine;

public class TileFactory : MonoBehaviour
{
    [Header("Типы тайлов (рандомно выбираются)")]
    [SerializeField] private GameObject[] tilePrefabs;

    [Header("Размер доски")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private float spacing = 1f;

    [Header("Родитель")]
    [SerializeField] private Transform tileParent;

    [Header("Автогенерация")]
    [SerializeField] private bool generateOnStart = true;

    private void Start()
    {
        if (generateOnStart)
            GenerateBoard();
    }

    public void GenerateBoard()
    {
        ClearBoard();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * spacing, 0f, y * spacing);
                GameObject prefab = GetRandomTilePrefab();
                GameObject tileGO = Instantiate(prefab, position, Quaternion.identity, tileParent);
                tileGO.name = $"Tile ({x}, {y})";

                Tile tile = tileGO.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.SetPosition(position);
                }
            }
        }

        new TileNeighborUpdater().UpdateNeighbors(TilesRepository.Instance.GetTiles());
    }

    public void ClearBoard()
    {
        if (tileParent == null) return;

        foreach (Transform child in tileParent)
        {
            DestroyImmediate(child.gameObject);
        }

        TilesRepository.Instance.GetTiles().Clear();
    }

    private GameObject GetRandomTilePrefab()
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogError("[TileFactory] Нет тайл-префабов!");
            return null;
        }

        return tilePrefabs[Random.Range(0, tilePrefabs.Length)];
    }
}
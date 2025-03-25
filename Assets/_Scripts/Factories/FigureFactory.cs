using UnityEngine;

public class FigureFactory : MonoBehaviour
{
    [SerializeField] private GameObject figurePrefab;
    [SerializeField] private NeighborTilesSelectionSettings pawnSettings;
    [SerializeField] private NeighborTilesSelectionSettings knightSettings;

    public Figure CreateFigure(Vector2Int position, bool isWhite, FigureType type)
    {
        GameObject figureObj = Instantiate(figurePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        Figure figure = figureObj.GetComponent<Figure>();
        
        figure.whiteTeamAffiliation = isWhite;
        figure.neighborTilesSelectionSettings = GetSettingsByType(type);
        figure.isPawn = (type == FigureType.Pawn);

        return figure;
    }

    private NeighborTilesSelectionSettings GetSettingsByType(FigureType type)
    {
        return type switch
        {
            FigureType.Pawn => pawnSettings,
            FigureType.Knight => knightSettings,
            _ => throw new System.ArgumentException("Unknown figure type")
        };
    }
}

public enum FigureType { Pawn, Knight }
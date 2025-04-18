﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KingThreatStateCache
{
    public static KingThreatStateCache Instance { get; } = new();

    private Dictionary<Figure, KingThreatAnalyzer.Result> cache = new();
    

    public void UpdateThreats()
    {
        cache.Clear();

        var kings = FiguresRepository.Instance.AllFigures
            .Where(f => f.IsKing && f.CurrentTile != null);

        foreach (var king in kings)
        {
            var enemies = FiguresRepository.Instance.GetFiguresByTeam(king.WhiteTeamAffiliation == false);
            var allies = FiguresRepository.Instance.GetFiguresByTeam(king.WhiteTeamAffiliation)
                .Where(f => f.IsKing == false).ToList();

            var result = KingThreatAnalyzer.Analyze(king, enemies, allies);
            cache[king] = result;
        }
    }

    public KingThreatAnalyzer.Result GetThreatState(Figure king)
    {
        if (king == null || king.IsKing == false || king.CurrentTile == null)
        {
            Debug.LogWarning($"[KTS] ❌ Пропущен {king.name} — CurrentTile == null");
            return null;
        }

        cache.TryGetValue(king, out var result);
        return result;
    }
    
    public void InvalidateCache()
    {
        cache.Clear();
    }

    public bool IsKingUnderAttack(Figure king)
    {
        return GetThreatState(king)?.isUnderAttack ?? false;
    }

    public bool IsDoubleAttack(Figure king)
    {
        return GetThreatState(king)?.isDoubleCheck ?? false;
    }

    public List<Figure> GetCoveringFigures(Figure king)
    {
        return GetThreatState(king)?.coveringPieces ?? new();
    }

    public List<Tile> GetBlockableTiles(Figure king)
    {
        return GetThreatState(king)?.blockableTiles ?? new();
    }

    public List<Figure> GetAttackers(Figure king)
    {
        return GetThreatState(king)?.attackers ?? new();
    }
}
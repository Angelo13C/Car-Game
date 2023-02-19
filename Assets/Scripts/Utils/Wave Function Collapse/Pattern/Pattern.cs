using System;
using System.Collections.Generic;
using Unity.Collections;

public readonly struct PatternSet : IDisposable
{
    public readonly NativeArray<Pattern> Patterns;

    public PatternSet(NativeArray<Pattern> patterns)
    {
        patterns.Sort(new PatternSetComparer());
        Patterns = patterns;
    }

    public Cell GetCellWithAllPossibleStates()
    {
        var cell = new Cell {
            SuperPosition = 0
        };
        for(var i = 0; i < Patterns.Length; i++)
            cell.SuperPosition |= Patterns[i].ID.Value;
        return cell;
    }

    public void Dispose() => Patterns.Dispose();

    private struct PatternSetComparer : IComparer<Pattern>
    {
        public int Compare(Pattern x, Pattern y)
        {
            return x.ID.Value.CompareTo(y.ID.Value);
        }
    }
}

public readonly struct Pattern
{
    public readonly PatternId ID;
    public readonly uint Weight;
    
    // I store in a cell the superposition of the valid patterns for each side
    public readonly Cell UpValidNeighbours;
    public readonly Cell RightValidNeighbours;
    public readonly Cell DownValidNeighbours;
    public readonly Cell LeftValidNeighbours;

    public Cell GetValidNeighboursInDirection(Direction direction)
    {
        switch(direction)
        {
            case Direction.Up: return UpValidNeighbours;
            case Direction.Right: return RightValidNeighbours;
            case Direction.Down: return DownValidNeighbours;
            case Direction.Left: return LeftValidNeighbours;
        }
        return Cell.EMPTY;
    }
}

public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction)
    {
        switch(direction)
        {
            case Direction.Up: return Direction.Down;
            case Direction.Right: return Direction.Left;
            case Direction.Down: return Direction.Up;
            default: return Direction.Right;
        }
    }
}

public readonly struct PatternId
{
    public readonly uint Value;
}
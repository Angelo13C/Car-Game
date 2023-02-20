using System;
using System.Collections.Generic;
using System.Text;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public readonly struct PatternSet : IDisposable
{
    public readonly NativeArray<Pattern> Patterns;

    public PatternSet(NativeArray<Pattern> patterns)
    {
        patterns.Sort(new PatternSetComparer());
        Patterns = patterns;
    }

    [BurstCompile]
    public Cell GetCellWithAllPossibleStates()
    {
        var cell = new Cell {
            SuperPosition = 0
        };
        for(var i = 0; i < Patterns.Length; i++)
            cell.SuperPosition |= Patterns[i].ID.Value;
        return cell;
    }

    [BurstCompile]
    public void Dispose() => Patterns.Dispose();

    [BurstCompile]
    private struct PatternSetComparer : IComparer<Pattern>
    {
        [BurstCompile]
        public int Compare(Pattern x, Pattern y)
        {
            return x.ID.Value.CompareTo(y.ID.Value);
        }
    }

    public override string ToString()
    {
        var result = new StringBuilder(Patterns.Length * 200);
        foreach(var pattern in Patterns)
            result.Append(pattern).Append("\n\n");

        return result.ToString();
    }
}

[BurstCompile]
public struct Pattern
{
    public PatternId ID;
    public uint Weight;
    
    // I store in a cell the superposition of the valid patterns for each side
    public Cell UpValidNeighbours;
    public Cell RightValidNeighbours;
    public Cell DownValidNeighbours;
    public Cell LeftValidNeighbours;

    [BurstCompile]
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

    public override string ToString()
    {
        return "ID: " + ID + "\nWeight: " + Weight + "\nUp valid neighbours: " + UpValidNeighbours
             + "\nRight valid neighbours: " + RightValidNeighbours + "\nDown valid neighbours: " + DownValidNeighbours
             + "\nLeft valid neighbours: " + LeftValidNeighbours;
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
    [BurstCompile]
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

[BurstCompile]
public readonly struct PatternId : IEquatable<PatternId>
{
    public readonly uint Value;

    public PatternId(uint value)
    {
        Value = value;
    }

    [BurstCompile]
    public bool Equals(PatternId other) => Value == other.Value;

    public override string ToString() => Value.ToString();
}
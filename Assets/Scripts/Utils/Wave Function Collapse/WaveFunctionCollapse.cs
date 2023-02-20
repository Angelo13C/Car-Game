using Unity.Collections;
using Unity.Mathematics;

public struct WaveFunctionCollapse
{
    private Grid _grid;
    private PatternSet _patternSet;

    public WaveFunctionCollapse(Grid grid, PatternSet patternSet)
    {
        _grid = grid;
        _patternSet = patternSet;
    }

    private enum Status
    {
        Valid,
        Invalid
    }
    // Offset to apply to the seed if a collapse goes wrong (the actual value 123 is random)
    private const int SEED_OFFSET_INVALID_COLLAPSE = 123;

    public NativeArray<Cell> Collapse(uint seed, Allocator allocator)
    {
        var cells = new NativeArray<Cell>(_grid.Area, allocator);
        Collapse(seed, ref cells);
        return cells;
    }

    public void Collapse(uint seed, ref NativeArray<Cell> result)
    {
        Initialize(result);

        var rng = new Random(seed);
        var patternsStack = new NativeArrayStack<Pattern>(_patternSet.Patterns.Length, Allocator.Temp);
        var propagationStack = new NativeArrayStack<int>(_grid.Area, Allocator.Temp);
        while(!IsCollapsed(result))
        {
            if(Iterate(result, patternsStack, propagationStack, ref rng) == Status.Invalid)
            {
                Collapse(seed + SEED_OFFSET_INVALID_COLLAPSE, ref result);
                break;
            }
        }

        patternsStack.Dispose();
        propagationStack.Dispose();
    }

    private void Initialize(NativeArray<Cell> cells)
    {
        var highestEntropyCell = _patternSet.GetCellWithAllPossibleStates();
        for(var i = 0; i < _grid.Area; i++)
        {
            cells[i] = highestEntropyCell;
        }
    }

    private bool IsCollapsed(NativeArray<Cell> cells)
    {
        for(var i = 0; i < _grid.Area; i++)
        {
            if(!cells[i].IsCollapsed)
                return false;
        }

        return true;
    }

    private Status Iterate(NativeArray<Cell> cells, NativeArrayStack<Pattern> patternsStack, NativeArrayStack<int> propagationStack, ref Random rng)
    {
        var coords = FindMinEntropyCoords(cells);
        if(coords == -1)
            return Status.Invalid;

        CollapseCoords(coords, cells, patternsStack, ref rng);
        Propagate(coords, cells, patternsStack, propagationStack);
        return Status.Valid;
    }

    private int FindMinEntropyCoords(NativeArray<Cell> cells)
    {
        var minEntropyIndex = -1;
        var minEntropy = byte.MaxValue;

        for(var i = 0; i < cells.Length; i++)
        {
            var currentEntropy = cells[i].GetEntropy();
            if(currentEntropy != 1 && (currentEntropy < minEntropy || minEntropy == byte.MaxValue))
            {
                minEntropyIndex = i;
                minEntropy = currentEntropy;
            }
        }

        return minEntropy == 0 ? -1 : minEntropyIndex;
    }

    private void CollapseCoords(int coords, NativeArray<Cell> cells, NativeArrayStack<Pattern> possiblePatterns, ref Random rng)
    {
        var cellToCollapse = cells[coords];
        uint totalWeight = 0;
        possiblePatterns.Clear();
        for(var i = 0; i < _patternSet.Patterns.Length; i++)
        {
            if((cellToCollapse.SuperPosition & 0x01) == 1)
            {
                totalWeight += _patternSet.Patterns[i].Weight;
                possiblePatterns.Push(_patternSet.Patterns[i]);
            }
            cellToCollapse = new Cell { SuperPosition = cellToCollapse.SuperPosition >> 1 };
        }

        var randomValue = rng.NextUInt(totalWeight);
        var possiblePattern = possiblePatterns.Pop();
        while(randomValue < possiblePattern.Weight)
        {
            randomValue -= possiblePattern.Weight;
            possiblePattern = possiblePatterns.Pop();
        }

        cells[coords] = new Cell { SuperPosition = possiblePattern.ID.Value };
    }

    private void Propagate(int coords, NativeArray<Cell> cells, NativeArrayStack<Pattern> possiblePatterns, NativeArrayStack<int> propagationStack)
    {
        propagationStack.Clear();
        propagationStack.Push(coords);

        while(!propagationStack.IsEmpty())
        {
            var currentCoords = propagationStack.Pop();
            var currentCell = cells[currentCoords];

            ConstrainNeighbour(currentCoords, Direction.Right, 1, cells, possiblePatterns, ref propagationStack);
            ConstrainNeighbour(currentCoords, Direction.Left, -1, cells, possiblePatterns, ref propagationStack);
            ConstrainNeighbour(currentCoords, Direction.Down, _grid.Size.x, cells, possiblePatterns, ref propagationStack);
            ConstrainNeighbour(currentCoords, Direction.Up, -_grid.Size.x, cells, possiblePatterns, ref propagationStack);
        }
    }

    private void ConstrainNeighbour(int currentCoords, Direction direction, int directionOffset, NativeArray<Cell> cells, NativeArrayStack<Pattern> possiblePatterns, ref NativeArrayStack<int> propagationStack)
    {
        var currentCell = cells[currentCoords];
        var neighbourCoords = currentCoords + directionOffset;
        if(_grid.IsIndexValid(neighbourCoords))
        {
            var neighbourCell = cells[neighbourCoords];
            possiblePatterns.Clear();
            for(var i = 0; i < _patternSet.Patterns.Length; i++)
            {
                if((neighbourCell.SuperPosition & 0x01) == 1)
                {
                    possiblePatterns.Push(_patternSet.Patterns[i]);
                }
                neighbourCell = new Cell { SuperPosition = neighbourCell.SuperPosition >> 1 };
            }
            
            var oppositeDirection = direction.Opposite();
            while(!possiblePatterns.IsEmpty())
            {
                var possiblePattern = possiblePatterns.Pop();
                if(currentCell.Overlap(possiblePattern.GetValidNeighboursInDirection(oppositeDirection)) != Cell.EMPTY)
                    neighbourCell.SuperPosition |= possiblePattern.ID.Value;
                else
                    propagationStack.Push(neighbourCoords);
            }
            
            cells[neighbourCoords] = neighbourCell;
        }
    }
}
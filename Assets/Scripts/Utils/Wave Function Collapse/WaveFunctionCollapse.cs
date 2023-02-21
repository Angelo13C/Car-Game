using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
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

    [BurstCompile]
    public void Collapse(uint seed, Allocator allocator, out NativeArray<Cell> result)
    {
        result = new NativeArray<Cell>(_grid.Area, allocator, NativeArrayOptions.UninitializedMemory);
        Collapse(seed, ref result);
    }

    [BurstCompile]
    public void Collapse(uint seed, ref NativeArray<Cell> result)
    {
        Initialize(result);

        var rng = new Random(seed);
        var patternsStack = new NativeArrayStack<Pattern>(_patternSet.Patterns.Length, Allocator.Temp);
        var propagationStack = new NativeArrayStack<int>(_grid.Area, Allocator.Temp);
        var entropyHeap = new EntropyHeap(_grid.Area, Allocator.Temp);
        while(!IsCollapsed(result))
        {
            if(Iterate(result, ref entropyHeap, patternsStack, propagationStack, ref rng) == Status.Invalid)
            {
                Collapse(seed + SEED_OFFSET_INVALID_COLLAPSE, ref result);
                break;
            }
        }

        patternsStack.Dispose();
        propagationStack.Dispose();
        entropyHeap.Dispose();
    }

    [BurstCompile]
    private void Initialize(NativeArray<Cell> cells)
    {
        var highestEntropyCell = _patternSet.GetCellWithAllPossibleStates();
        for(var i = 0; i < _grid.Area; i++)
        {
            cells[i] = highestEntropyCell;
        }
    }

    [BurstCompile]
    private bool IsCollapsed(NativeArray<Cell> cells)
    {
        for(var i = 0; i < _grid.Area; i++)
        {
            if(!cells[i].IsCollapsed)
                return false;
        }

        return true;
    }

    [BurstCompile]
    private Status Iterate(NativeArray<Cell> cells, ref EntropyHeap entropyHeap, NativeArrayStack<Pattern> patternsStack, NativeArrayStack<int> propagationStack, ref Random rng)
    {
        var coords = FindMinEntropyCoords(cells, ref entropyHeap, ref rng);
        CollapseCoords(coords, cells, patternsStack, ref rng);
        return Propagate(coords, cells, ref entropyHeap, patternsStack, propagationStack);
    }

    [BurstCompile]
    private int FindMinEntropyCoords(NativeArray<Cell> cells, ref EntropyHeap entropyHeap, ref Random rng)
    {
        var coords = entropyHeap.GetMinEntropyCellCoords();
        if(coords == -1)
        {
            do
            {
                coords = rng.NextInt(cells.Length);
            } while(cells[coords].IsCollapsed);
        }

        return coords;
    }

    [BurstCompile]
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
        while(randomValue >= possiblePattern.Weight)
        {
            randomValue -= possiblePattern.Weight;
            possiblePattern = possiblePatterns.Pop();
        }

        cells[coords] = new Cell { SuperPosition = possiblePattern.ID.Value };
    }

    [BurstCompile]
    private Status Propagate(int coords, NativeArray<Cell> cells, ref EntropyHeap entropyHeap, NativeArrayStack<Pattern> possiblePatterns, NativeArrayStack<int> propagationStack)
    {
        propagationStack.Clear();
        propagationStack.Push(coords);

        while(!propagationStack.IsEmpty())
        {
            var currentCoords = propagationStack.Pop();
            var currentCell = cells[currentCoords];

            if(ConstrainNeighbour(currentCoords, Direction.Right, 1, cells, ref entropyHeap, possiblePatterns, ref propagationStack) == Status.Invalid)
                return Status.Invalid;
            if(ConstrainNeighbour(currentCoords, Direction.Left, -1, cells, ref entropyHeap, possiblePatterns, ref propagationStack) == Status.Invalid)
                return Status.Invalid;
            if(ConstrainNeighbour(currentCoords, Direction.Down, _grid.Size.x, cells, ref entropyHeap, possiblePatterns, ref propagationStack) == Status.Invalid)
                return Status.Invalid;
            if(ConstrainNeighbour(currentCoords, Direction.Up, -_grid.Size.x, cells, ref entropyHeap, possiblePatterns, ref propagationStack) == Status.Invalid)
                return Status.Invalid;
        }
        return Status.Valid;
    }

    [BurstCompile]
    private Status ConstrainNeighbour(int currentCoords, Direction direction, int directionOffset, NativeArray<Cell> cells, ref EntropyHeap entropyHeap, NativeArrayStack<Pattern> possiblePatterns, ref NativeArrayStack<int> propagationStack)
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
            var changed = false;
            while(!possiblePatterns.IsEmpty())
            {
                var possiblePattern = possiblePatterns.Pop();
                if(currentCell.Overlap(possiblePattern.GetValidNeighboursInDirection(oppositeDirection)) != Cell.EMPTY)
                    neighbourCell = neighbourCell.Union(new Cell { SuperPosition = possiblePattern.ID.Value });
                else
                    changed = true;
            }
            
            if(changed)
            {
                propagationStack.Push(neighbourCoords);
                cells[neighbourCoords] = neighbourCell;
                entropyHeap.UpdateCoordsWithValue(neighbourCoords, neighbourCell);
                if(neighbourCell == Cell.EMPTY)
                    return Status.Invalid;
            }
        }
        return Status.Valid;
    }
}
using Unity.Mathematics;

public struct Grid
{
    public int2 GridSize;
    public int Area => GridSize.x * GridSize.y;
    
    public int GridPositionToIndex(int2 gridPosition)
    {
        return gridPosition.x + gridPosition.y * GridSize.x;
    }
    public int2 IndexToGridPosition(int index)
    {
        return new int2(index % GridSize.x, index / GridSize.x);
    }

    public bool IsGridPositionValid(int2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < GridSize.x && gridPosition.y >= 0 && gridPosition.y < GridSize.y;
    }
    public bool IsIndexValid(int index)
    {
        return index >= 0 && index < Area;
    }
}
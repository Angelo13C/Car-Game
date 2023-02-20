using Unity.Mathematics;

public struct Grid
{
    public int2 Size;
    public int Area => Size.x * Size.y;

    public Grid(int width, int height)
    {
        Size = new int2(width, height);
    }
    
    public int GridPositionToIndex(int2 gridPosition)
    {
        return gridPosition.x + gridPosition.y * Size.x;
    }
    public int2 IndexToGridPosition(int index)
    {
        return new int2(index % Size.x, index / Size.x);
    }

    public bool IsGridPositionValid(int2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < Size.x && gridPosition.y >= 0 && gridPosition.y < Size.y;
    }
    public bool IsIndexValid(int index)
    {
        return index >= 0 && index < Area;
    }
}
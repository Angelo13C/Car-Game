using Unity.Entities;
using Unity.Mathematics;

public struct StreetNetwork : IComponentData
{
	public Grid Grid;
	public float2 StreetTileSize;
	public float LaneWidth;

	public int2 PositionToGridPosition(float2 position) => new int2((int) (position.x / StreetTileSize.x), (int) (position.y / StreetTileSize.y));
	public int PositionToIndex(float2 position) => Grid.GridPositionToIndex(PositionToGridPosition(position));

	public bool IsStreet(int2 gridPosition, DynamicBuffer<StreetTile> streetTiles)
	{
		return Grid.IsGridPositionValid(gridPosition) && streetTiles[Grid.GridPositionToIndex(gridPosition)].IsStreet;
	}

	public Adjacency GetAdjacencyStreets(int2 gridPosition, DynamicBuffer<StreetTile> streetTiles)
	{
		var adjacency = Adjacency.Empty;
		if(IsStreet(gridPosition + new int2(1, 0), streetTiles))
			adjacency |= Adjacency.Right;
		if(IsStreet(gridPosition + new int2(-1, 0), streetTiles))
			adjacency |= Adjacency.Left;
		if(IsStreet(gridPosition + new int2(0, 1), streetTiles))
			adjacency |= Adjacency.Up;
		if(IsStreet(gridPosition + new int2(0, -1), streetTiles))
			adjacency |= Adjacency.Down;
		return adjacency;
	}
}

public enum Adjacency
{
    Empty = 0,
    Right = (1 << 0),
    Up = (1 << 1),
    Left = (1 << 2),
    Down = (1 << 3),
}


[InternalBufferCapacity(0)]
public struct StreetTile : IBufferElementData
{
	public bool IsStreet;
}

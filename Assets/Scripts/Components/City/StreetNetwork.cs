using Unity.Entities;
using Unity.Mathematics;

public struct StreetNetwork : IComponentData
{
	public Grid Grid;
	public float2 StreetTileSize;

	public bool IsStreet(int2 gridPosition, DynamicBuffer<StreetTile> streetTiles)
	{
		return Grid.IsGridPositionValid(gridPosition) && streetTiles[Grid.GridPositionToIndex(gridPosition)].IsStreet;
	}
}

[InternalBufferCapacity(0)]
public struct StreetTile : IBufferElementData
{
	public bool IsStreet;
}

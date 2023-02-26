using Unity.Entities;
using Unity.Mathematics;

public struct StreetNetwork : IComponentData
{
	public Grid Grid;
	public float2 StreetTileSize;
}

[InternalBufferCapacity(0)]
public struct StreetTile : IBufferElementData
{
	public bool IsStreet;
}

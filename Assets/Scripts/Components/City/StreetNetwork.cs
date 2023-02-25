using Unity.Collections;
using Unity.Entities;

public struct StreetNetwork : IComponentData
{
	public NativeArray<bool> Streets;
}

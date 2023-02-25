using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CityGenerator : IComponentData
{
    public Texture2D InputImage;
    
	public float2 CellSize;
	public int2 CitySize;
	public uint Seed;
	public int N;

	public Color32 RoadColor;

    public bool Generate;
}

[InternalBufferCapacity(30)]
public struct CityPlaceableObject : IBufferElementData
{
	public Entity Prefab;
	public int2 Size;
	public CityObjectType Type;
}

public enum CityObjectType
{
	StraightRoad,
	Building
}